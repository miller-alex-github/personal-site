using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ma.Web.Controllers
{
    [Authorize]
    public class BirthdaysController : Controller
    {
        private readonly IAppointmentsAPI appointmentsAPI;

        public BirthdaysController(IAppointmentsAPI appointmentsAPI)
        {
            this.appointmentsAPI = appointmentsAPI;
        }

        public async Task<IActionResult> Index()
        {
            var appointmentItems = await appointmentsAPI.GetAsync(pageSize:100);
            var model = new AppointmentViewModel { AppointmentItems = appointmentItems };

            return View(model);
        }

        [HttpGet]
        public IActionResult New()
        {            
            return View(model: new Appointment());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm]Appointment newItem)
        {
            if (ModelState.IsValid)
            {                
                newItem.UserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var successful = await appointmentsAPI.CreateAsync(newItem);
                if (!successful)
                    return BadRequest("Could not add item.");

                return RedirectToAction("Index");
            }
            else
            {
                return View("New", model: newItem);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("File is not selected!");

            using (var reader = new StreamReader(file.OpenReadStream(), System.Text.Encoding.UTF8))
            {
                var text = await reader.ReadToEndAsync();
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var successful = await appointmentsAPI.ImportAsync(userId, text);
                if (!successful)
                    return BadRequest("Could not add item.");

                return RedirectToAction("Index");
            }            
        }
    }
}