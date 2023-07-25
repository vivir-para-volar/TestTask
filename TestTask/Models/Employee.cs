using System.ComponentModel.DataAnnotations;

namespace TestTask.Models
{
    public class Employee
    {
        [Key]
        [Display(Name = "Идентификатор сотрудника")]
        public int ID { get; set; }

        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = "Поле обязательное для заполнения")]
        [StringLength(50, ErrorMessage = "Должно быть короче 50 символов")]
        public string SurName { get; set; }

        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Поле обязательное для заполнения")]
        [StringLength(50, ErrorMessage = "Должно быть короче 50 символов")]
        public string FirstName { get; set; }

        [Display(Name = "Отчество")]
        [StringLength(50, ErrorMessage = "Должно быть короче 50 символов")]
        public string? Patronymic { get; set; }

        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Поле обязательное для заполнения")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Серия документа")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Должно быть длиной 4 символов")]
        [RegularExpression(@"\d+$", ErrorMessage = "Допустимы только цифры")]
        public string? DocSeries { get; set; }

        [Display(Name = "Номер документа")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Должно быть длиной 6 символов")]
        [RegularExpression(@"\d+$", ErrorMessage = "Допустимы только цифры")]
        public string? DocNumber { get; set; }

        [Display(Name = "Должность")]
        [Required(ErrorMessage = "Поле обязательное для заполнения")]
        [StringLength(50, ErrorMessage = "Должно быть короче 50 символов")]
        public string Position { get; set; }


        [Display(Name = "Идентификатор отдела")]
        public Guid DepartmentID { get; set; }


        [Display(Name = "Отдел")]
        public Department? Department { get; set; }



        public int CalculateAge()
        {
            var today = DateTime.Today;

            int age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.AddYears(age) > today)
            {
                age--;
            }
            return age;
        }
    }
}
