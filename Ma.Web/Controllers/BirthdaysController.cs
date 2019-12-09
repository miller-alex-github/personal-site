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
            var appointmentItems = await appointmentsAPI.GetAsync(pageSize:50);
            var model = new AppointmentViewModel { AppointmentItems = appointmentItems };

            return View(model);
        }
               
        public IActionResult Add()
        {            
            return View(model: new Appointment());
        }
                
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await appointmentsAPI.DeleteAsync(id);
            }
            catch
            {
                // ignore
            }

            return RedirectToAction("Index");
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