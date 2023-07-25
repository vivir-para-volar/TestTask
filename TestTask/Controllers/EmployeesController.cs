using Microsoft.AspNetCore.Mvc;
using TestTask.Models;
using TestTask.Services;

namespace TestTask.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeesService _employeesService;
        private readonly DepartmentsService _departmentsService;

        public EmployeesController(IConfiguration config)
        {
            var connString = config.GetConnectionString("DefaultConnection");

            _employeesService = new EmployeesService(connString);
            _departmentsService = new DepartmentsService(connString);
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _employeesService.GetAllEmployees();
            return View(employees);
        }

        public async Task<IActionResult> Details(int id)
        {
            var employee = await _employeesService.GetEmployee(id);
            if (employee == null) return NotFound();

            return View(employee);
        }


        public async Task<IActionResult> Create(string departmentId)
        {
            var department = await _departmentsService.GetDepartment(Guid.Parse(departmentId));
            if (department == null) return NotFound();

            var employee = new Employee
            {
                DateOfBirth = DateTime.Now,

                DepartmentID = department.ID,
                Department = department
            };

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (!ModelState.IsValid) return View(employee);

            await _employeesService.CreateEmployee(employee);
            return RedirectToAction(nameof(Details), "Departments", new { id = employee.DepartmentID });
        }


        public async Task<IActionResult> Update(int id)
        {
            var employee = await _employeesService.GetEmployee(id);
            if (employee == null) return NotFound();

            var departments = await _departmentsService.GetAllDepartments();
            departments = Department.GetHierarchy(departments);

            ViewBag.Departments = departments;
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                var departments = await _departmentsService.GetAllDepartments();
                departments = Department.GetHierarchy(departments);

                ViewBag.Departments = departments;
                return View(employee);
            }

            await _employeesService.UpdateEmployee(employee);
            return RedirectToAction(nameof(Details), new { id = employee.ID });
        }


        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeesService.GetEmployee(id);
            if (employee == null) return NotFound();

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int id)
        {
            await _employeesService.DeleteEmployee(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
