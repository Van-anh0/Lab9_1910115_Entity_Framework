using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lab9_1910115_Entity_Framework.Models;

namespace Lab9_1910115_Entity_Framework
{
    public partial class UpdateFoodForm : Form
    {
        private RestaurantContext _dbContext;
        private int _foodId;

        public UpdateFoodForm(int? foodId = null)
        {
            InitializeComponent();
            _dbContext = new RestaurantContext();
            _foodId = foodId ?? 0;
        }

        private void LoadCategoriesToCombobox()
        {
            //lấy tất cả danh mục thức ăn, sắp tăng theo tên
            var categories = _dbContext.Categories
                .OrderBy(x => x.Name).ToList();

            //nạp danh mục vào combobox, hiển thị tên cho người dùng xem nhưng khi được chọn thì lấy giá trị là ID
            cbbFoodCategory.DisplayMember = "Name";
            cbbFoodCategory.ValueMember = "Id";
            cbbFoodCategory.DataSource = categories;
        }

        private void ShowFoodInformation()
        {
            //tìm món ăn theo mã số đã được truyền vào form
            var food = GetFoodById(_foodId);

            //nếu không tìm thấy, không cần làm gì cả
            if (food == null) return;

            //ngược lại, hiển thị thông tin món ăn trên form
            txtFoodId.Text = food.Id.ToString();
            txtFoodName.Text = food.Name;
            cbbFoodCategory.SelectedValue = food.FoodCategoryId;
            txtFoodUnit.Text = food.Unit;
            nudFoodPrice.Value = food.Price;
            txtFoodNotes.Text = food.Notes;
        }
        private void UpdateFoodForm_Load(object sender, EventArgs e)
        {
            //nạp danh sách nhóm thức ăn vào combobox
            LoadCategoriesToCombobox();

            //hiển thị thông tin món ăn lên form
            ShowFoodInformation();
            
        }

        private Food GetFoodById(int foodId)
        {
            //tìm món ăn theo mã số
            return foodId > 0 ? _dbContext.Foods.Find(foodId) : null;
        }

        private bool ValidateUserInput()
        {
            //kiểm tra tên món ăn đã được nhập hay chưa
            if (string.IsNullOrWhiteSpace(txtFoodName.Text))
            {
                MessageBox.Show("Tên món ăn, đồ uống không được để trống", "Thông báo");
                return false;
            }

            //kiểm tra đơn vị tính đã được nhập hay chưa
            if (string.IsNullOrWhiteSpace(txtFoodUnit.Text))
            {
                MessageBox.Show("Đơn vị tính không được để trống", "Thông báo");
                return false;
            }

            //kiểm tra giá món ăn đã được nhập hay chưa
            if (nudFoodPrice.Value.Equals(0))
            {
                MessageBox.Show("Giá của thức ăn phải lớn hơn 0", "Thông báo");
                return false;

            }

            //kiểm tra nhóm món ăn đã được chọn hay chưa
            if (cbbFoodCategory.SelectedIndex < 0)
            {
                MessageBox.Show("Bạn chưa chọn nhóm thức ăn", "Thông báo");
                return false;
            }
            return true;
        }
        private Food GetUpdatedFood()
        {
            //tạo đối tượng food với thông tin được lấy từ các điều kiển trên form
            var food = new Food()
            {
                Name = txtFoodName.Text.Trim(),
                FoodCategoryId = (int)cbbFoodCategory.SelectedValue,
                Unit = txtFoodUnit.Text,
                Price = (int)nudFoodPrice.Value,
                Notes = txtFoodNotes.Text
            };

            //gán giá trị của Id ban đầu (nếu đang cập nhật
            if (_foodId > 0)
            {
                food.Id = _foodId;
            }
            return food;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //kiểm tra nếu dữ liệu nhập vào là hợp lệ
            if (ValidateUserInput())
            {
                //thì lấy thông tin người dùng nhập vào
                var newFood = GetUpdatedFood();

                //và thử tìm xem đã có món ăn trong CSDL chưa
                var oldFood = GetFoodById(_foodId);

                //nếu chưa có (chưa tồn tại)
                if (oldFood == null)
                {
                    //thì thêm món ăn mới
                    _dbContext.Foods.Add(newFood);
                }
                else
                {
                    //ngược lại, cập nhật thông tin món ăn
                    oldFood.Name = newFood.Name;
                    oldFood.FoodCategoryId = newFood.FoodCategoryId;
                    oldFood.Unit = newFood.Unit;
                    oldFood.Price = newFood.Price;
                    oldFood.Notes = newFood.Notes;
                }

                //lưu các thay đổi xuống Csdl
                _dbContext.SaveChanges();

                //đóng hộp thoại
                DialogResult = DialogResult.OK;
            }
        }
    }
}
