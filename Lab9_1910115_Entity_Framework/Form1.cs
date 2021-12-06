using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Lab9_1910115_Entity_Framework.Models;

namespace Lab9_1910115_Entity_Framework
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ShowCategories();
        }

        private void btnReloadCategory_Click(object sender, EventArgs e)
        {
            ShowCategories();

        }

        private List<Category> GetCategories()
        {
            //khởi tạo đối tượng context
            var dbContext = new RestaurantContext();

            //lấy danh sách tất cả nhóm thức ăn, sắp xếp theo tên
            return dbContext.Categories.OrderBy(x => x.Name).ToList();

        }

        private void ShowCategories()
        {
            //xóa tất cả các nút hiện có trên cây
            tvCategory.Nodes.Clear();

            //tạo danh sách loại nhóm thức ăn, đồ uống
            //tên của các loại này được hiển thị trên các nút mức 2
            var cateMap = new Dictionary<CategoryType, string>()
            {
                [CategoryType.Food] = "Đồ ăn",
                [CategoryType.Drink] = "Thức uống"
            };

            //tạo nút gốc của cây
            var rootNode = tvCategory.Nodes.Add("Tất cả");

            //lấy danh sách nhóm đồ ăn, thức uống
            var categories = GetCategories();

            //duyệt qua các loại nhóm thức ăn
            foreach (var cateType in cateMap)
            {
                //tạo các nút tương ứng với loại nhóm thức ăn
                var childNode = rootNode.Nodes.Add(cateType.Key.ToString(), cateType.Value);
                childNode.Tag = cateType.Key;

                //duyệt qua các nhoms thức ăn
                foreach (var category in categories)
                {
                    //nếu nhóm đang xét không cùng loại thì bỏ qua
                    if (category.Type != cateType.Key) continue;

                    //ngược lại, tạo các nút tương ứng trên cây
                    var grantChildNode = childNode.Nodes.Add(category.Id.ToString(), category.Name);
                    grantChildNode.Tag = category;
                }
                
            }

            //mở rộng các nhánh của cây để thấy hết tất cả các nhóm thức ăn
            tvCategory.ExpandAll();

            //đánh dấu nút gốc đang được chọn
            tvCategory.SelectedNode = rootNode;
        }

        private List<FoodModel> GetFoodByCategory(int? categoryId)
        {
            //khởi tạo đối tượng context
            var dbContext = new RestaurantContext();
            //tạo truy vấn lấy danh sách món ăn
            var foods = dbContext.Foods.AsQueryable();

            //nếu mã nhóm món ăn khác null và hợp lệ
            if (categoryId != null && categoryId > 0)
            {
                //thì tìm theo mã số nhóm thức ăn
                foods = foods.Where(x => x.FoodCategoryId == categoryId);

            }

            //sắp xếp đồ ăn, thức uống theo tên và trả về
            //danh sách chứa đầy đủ thông tin về món ăn
            return foods
                .OrderBy(x => x.Name)
                .Select(x => new FoodModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Unit = x.Unit,
                    Price = x.Price,
                    Notes = x.Notes,
                    CategoryName = x.Category.Name
                })
                .ToList();
        }

        private  List<FoodModel> GetFoodByCategoryType(CategoryType cateType)
        {
            var dbContext = new RestaurantContext();
            //tìm các món ăn theo loại nhóm thức ăn (Category type).
            //sắp xếp đồ ăn, thức uongs theo tên và trả về ds chứa đầy đủ thông tin về món

            return dbContext.Foods
                .Where(x => x.Category.Type == cateType)
                .OrderBy(x => x.Name)
                .Select(x => new FoodModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Unit = x.Unit,
                    Price = x.Price,
                    Notes = x.Notes,
                    CategoryName = x.Category.Name
                })
                .ToList();
        }

        private void ShowFoodsForNode(TreeNode node)
        {
            //xóa danh sách thực đơn hiện tại khỏi listview
            lvFood.Items.Clear();

            //nếu node == null, không cần xử lý gì thêm
            if (node == null) return;

            //tạo danh sách để chứa danh sách các món ăn tìm được
            List<FoodModel> foods = null;

            //nếu nút được chọn trên TreeView tương ứng với loại nhóm thức ăn
            //(Category type) (mức thứ 2 trên cây)
            if (node.Level == 1)
            {
                //thì lấy danh sách món ăn theo loại nhóm
                var categoryType = (CategoryType)node.Tag;
                foods = GetFoodByCategoryType(categoryType);

            }else
            {
                //ngược lại, lấy đanh sách món ăn theo thể loại
                //nếu nút được chọn là 'tất cả' thì lấy hết
                var category = node.Tag as Category;
                foods = GetFoodByCategory(category?.Id);
            }
            //gọi hàm để hiển thị các món ăn lên lv
            ShowFoodsOnListView(foods);
        }

        private void ShowFoodsOnListView(List<FoodModel> foods)
        {
            //duyệt qua từng phần tử của danh sách food
            foreach (var foodItem in foods)
            {
                //tạo item tương ứng trên listView
                var item = lvFood.Items.Add(foodItem.Id.ToString());

                //và hiển thị các thông tin của món ăn
                item.SubItems.Add(foodItem.Name);
                item.SubItems.Add(foodItem.Unit);
                item.SubItems.Add(foodItem.Price.ToString("##,###"));
                item.SubItems.Add(foodItem.CategoryName);
                item.SubItems.Add(foodItem.Notes);
            }
        }

        private void tvCategory_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ShowFoodsForNode(e.Node);
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            var dialog = new UpdateCategoryForm();
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                ShowCategories();
            }
        }

        private void tvCategory_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if(e.Node == null || e.Node.Level < 2 || e.Node.Tag == null) return;
            var category = e.Node.Tag as Category;
            var dialog = new UpdateCategoryForm(category?.Id);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                ShowCategories();
            }
        }

        private void btnReloadFood_Click(object sender, EventArgs e)
        {
            ShowFoodsForNode(tvCategory.SelectedNode);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //nếu không có món ăn nào được chọn, không cần làm gì cả
            if (lvFood.SelectedItems.Count == 0) return;

            //ngược lại, lấy mã số của món ăn được chọn
            var dbContext = new RestaurantContext();
            var selectedFoodId = int.Parse(lvFood.SelectedItems[0].Text);

            //truy vấn để lấy thông tin của món ăn đó
            var selectedFood = dbContext.Foods.Find(selectedFoodId);

            //nếu tìm hấy thông tin món ăn
            if (selectedFood != null)
            {
                //thì xóa nó khỏi csdl
                dbContext.Foods.Remove(selectedFood);
                dbContext.SaveChanges();

                //và đồng thời xóa khỏi listview
                lvFood.Items.Remove(lvFood.SelectedItems[0]);
            }
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            var dialog = new UpdateFoodForm();
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                ShowFoodsForNode(tvCategory.SelectedNode);
            }
        }

        private void lvFood_DoubleClick(object sender, EventArgs e)
        {
            if (lvFood.SelectedItems.Count == 0) return;
            var foodId = int.Parse(lvFood.SelectedItems[0].Text);
            var dialog = new UpdateFoodForm(foodId);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                ShowFoodsForNode(tvCategory.SelectedNode);
            }
        }
    }
}
