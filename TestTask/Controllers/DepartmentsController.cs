using Microsoft.AspNetCore.Mvc;
using TestTask.Models;
using TestTask.Services;

namespace TestTask.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly DepartmentsService _departmentsService;

        public DepartmentsController(IConfiguration config)
        {
            var connString = config.GetConnectionString("DefaultConnection");
            _departmentsService = new DepartmentsService(connString);
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _departmentsService.GetAllDepartments();
            departments = Department.GetHierarchy(departments);

            return View(departments);
        }

        public async Task<IActionResult> Details(string id)
        {
            var department = await _departmentsService.GetDepartment(Guid.Parse(id));
            if (department == null) return NotFound();

            department.ChildDepartments = await _departmentsService.GetChildDepartments(Guid.Parse(id));
            department.Employees = await _departmentsService.GetDepartmentEmployees(Guid.Parse(id));

            return View(department);
        }

        public async Task<IActionResult> Create(string? parentId)
        {
            var department = new Department();

            if (parentId != null)
            {
                var parentDepartment = await _departmentsService.GetDepartment(Guid.Parse(parentId));
                if (parentDepartment == null) return NotFound();

                department.ParentDepartmentID = parentDepartment.ID;
                department.ParentDepartment = parentDepartment;
            }

            return View(department);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Department department)
        {
            if (!ModelState.IsValid) return View(department);

            await _departmentsService.CreateDepartment(department);

            if (department.ParentDepartmentID == null)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(Details), new { id = department.ParentDepartmentID });
            }
        }


        public async Task<IActionResult> Update(string id)
        {
            var department = await _departmentsService.GetDepartment(Guid.Parse(id));
            if (department == null) return NotFound();

            return View(department);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Department department)
        {
            if (!ModelState.IsValid) return View(department);

            await _departmentsService.UpdateDepartment(department);
            return RedirectToAction(nameof(Details), new { id = department.ID });
        }


        public async Task<IActionResult> Delete(string id)
        {
            var guidId = Guid.Parse(id);

            var department = await _departmentsService.GetDepartment(guidId);

            ViewBag.CheckChild = await _departmentsService.CheckChildDepartments(guidId);
            ViewBag.CheckEmployees = await _departmentsService.CheckDepartmentEmployees(guidId);
            return View(department);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePost(string id)
        {
            var guidId = Guid.Parse(id);

            var checkChild = await _departmentsService.CheckChildDepartments(guidId);
            if (checkChild) return BadRequest();

            var checkEmployees = await _departmentsService.CheckDepartmentEmployees(guidId);
            if (checkEmployees) return BadRequest();

            await _departmentsService.DeleteDepartment(guidId);
            return RedirectToAction(nameof(Index));
        }
    }
}