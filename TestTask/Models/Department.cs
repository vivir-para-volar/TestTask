using System.ComponentModel.DataAnnotations;

namespace TestTask.Models
{
    public class Department
    {
        [Key]
        [Display(Name = "Идентификатор отдела")]
        public Guid ID { get; set; }

        [Display(Name = "Мнемокод")]
        [StringLength(10, ErrorMessage = "Должно быть короче 10 символов")]
        public string? Code { get; set; }

        [Display(Name = "Наименование")]
        [Required(ErrorMessage = "Поле обязательное для заполнения")]
        [StringLength(50, ErrorMessage = "Должно быть короче 50 символов")]
        public string Name { get; set; }


        [Display(Name = "Идентификатор головного отдела")]
        public Guid? ParentDepartmentID { get; set; }

        [Display(Name = "Головной отдел")]
        public Department? ParentDepartment { get; set; }

        public ICollection<Employee> Employees { get; set; }
        public ICollection<Department> ChildDepartments { get; set; }

        public Department()
        {
            Employees = new List<Employee>();
            ChildDepartments = new List<Department>();
        }


        public static List<Department> GetHierarchy(List<Department> departments)
        {
            foreach (var department in departments)
            {
                if (department.ParentDepartmentID != null)
                {
                    departments.Find(item => item.ID == department.ParentDepartmentID)?.ChildDepartments.Add(department);
                }
            }

            for (var i = 0; i < departments.Count; i++)
            {
                if (departments[i].ParentDepartmentID != null)
                {
                    departments.Remove(departments[i]);
                    i--;
                }
            }

            return departments;
        }
    }
}
