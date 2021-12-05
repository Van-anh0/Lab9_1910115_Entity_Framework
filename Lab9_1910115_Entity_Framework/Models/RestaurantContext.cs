using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab9_1910115_Entity_Framework.Models
{
    class RestaurantContext : DbContext
    {
        // tham chiếu tới các nhóm món ăn trong bảng Category
        public DbSet<Category> Categories { get; set; }

        //tham chiếu tới các món ăn, đồ uống trong bảng Food
        public DbSet<Food> Foods { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // lúc này, thuộc tính Categories ánh xạ tới bảng Category trong db và thuộc tính Foods tương ứng với bảng Food trong csdl.
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // định nghĩa mối quan hệ một nhiều giữa 2 bảng Category và Food
            modelBuilder.Entity<Food>()
                .HasRequired(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.FoodCategoryId)
                .WillCascadeOnDelete(true);
            
        }
    }
}
