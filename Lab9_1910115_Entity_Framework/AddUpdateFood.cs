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
    public partial class UpdateCategoryForm : Form
    {
        private RestaurantContext _dbContext;
        private int _categoryId;
        public UpdateCategoryForm(int? categoryId = null)
        {
            InitializeComponent();
            _dbContext = new RestaurantContext();
            _categoryId = categoryId ?? 0;
        }

        private void UpdateCategoryForm_Load(object sender, EventArgs e)
        {
            //hiển thị thông tin nhóm thức ăn lên form
            ShowCategory();
        }

        private Category GetCategoryByID(int categoryId)
        {
            //nếu id được truyền vào là hợp lệ, ta tìm thông tin theo id
            //ngược lại, chỉ đơn giản trả về null, cho biết không thấy
            return categoryId > 0 ? _dbContext.Categories.Find(categoryId) : null;
        }

        private void ShowCategory()
        {
            //lấy thông tin của nhóm thức ăn
            var category = GetCategoryByID(_categoryId);

            //nếu ko tìm thấy thông tin, ko cần làm gì cả
            if (category == null) return;

            //ngược lại, nếu tìm thấy, hiển thị lên form
            txtCategoryID.Text = category.Id.ToString();
            txtCategoryName.Text = category.Name;
            cbbCategoryType.SelectedIndex = (int)category.Type;
        }

        private Category GetUpdatedCategory()
        {
            //tạo đối tượng Category với thông tin đã nhập
            var category = new Category()
            {
                Name = txtCategoryName.Text.Trim(),
                Type = (CategoryType)cbbCategoryType.SelectedIndex
            };

            //gán giá trị của id ban đầu (nếu đang cập nhật)
            if (_categoryId > 0)
            {
                category.Id = _categoryId;
            }

            return category;
        }

        private bool ValidateUserInput()
        {
            //kiểm tra tên nhóm thức ăn đã được nhập hay chưa
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show("Tên nhóm thức ăn không được để trống", "Thông báo");
                return false;
            }
            //kiểm tra loại nhóm thức ăn đã được chọn hay chưa
            if (cbbCategoryType.SelectedIndex < 0)
            {
                MessageBox.Show("Bạn chưa chọn loại nhóm thức ăn", "thông báo");
                return false;
            }
            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //kiểm tra nếu dữ liệu nhập vào là hợp lệ
            if (ValidateUserInput())
            {
                //thì lấy thông tin của người dùng nhập vào
                var newCategory = GetUpdatedCategory();

                // và tìm thử xem đã có nhóm thức ăn trong csdl chưa
                var oldCategory = GetCategoryByID(_categoryId);

                //nếu chưa có 
                if (oldCategory == null)
                {
                    //thì thêm nhóm thức ăn mới
                    _dbContext.Categories.Add(newCategory);
                }else
                {
                    //ngược lại, ta chỉ cần cập nhật thông tin cần thiết
                    oldCategory.Name = newCategory.Name;
                    oldCategory.Type = newCategory.Type;
                }

                //lưu các thay đổi xuống csdl
                _dbContext.SaveChanges();

                //đóng hộp thoại
                DialogResult = DialogResult.OK;
            }
        }
    }
}
