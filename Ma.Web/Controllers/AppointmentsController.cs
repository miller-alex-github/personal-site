using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ma.Web.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentsAPI appointmentsAPI;

        public AppointmentsController(IAppointmentsAPI appointmentsAPI)
        {
            this.appointmentsAPI = appointmentsAPI;
        }

        public async Task<IActionResult> Index()
        {
            var appointmentItems = await appointmentsAPI.GetAsync();
            var model = new AppointmentViewModel { AppointmentItems = appointmentItems };

            return View(model);
        }

        [HttpGet]
        public IActionResult New()
        {            
            return View(model: new Appointment());
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromForm]Appointment newItem)
        {
            if (ModelState.IsValid)
            {                
                newItem.UserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var successful = await appointmentsAPI.AddAsync(newItem);
                if (!successful)
                    return BadRequest("Could not add item.");

                return RedirectToAction("Index");
            }
            else
            {
                return View("New", model: newItem);
            }
        }
    }
}