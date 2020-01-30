using Ma.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ma.Services.Appointments
{
    /// <summary>
    /// Represents an appointment controller.
    /// </summary>
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]    
    public class AppointmentsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        
        /// <summary>
        /// Creates a new <see cref="AppointmentsController"/>.
        /// </summary>
        /// <param name="context">The <see cref="ApplicationDbContext"/> database store.</param>       
        public AppointmentsController(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        /// <summary>
        /// Get list of all appointments.
        /// </summary>          
        /// <param name="pageSize">Amount of items to retrieve. Defaults to 10. Must be valid 
        /// integer between 0 and 100.</param>
        /// <param name="pageIndex">The index of paginated page.</param>
        /// <returns>List of appointments.</returns>
        /// <response code="200">Success.</response>       
        /// <response code="401">Access denied.</response>          
        /// <response code="500">Internal server error.</response>
        [HttpGet()]
        [ApiVersion("1.0")]
        [ProducesResponseType(typeof(PaginatedItems<Appointment>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get([FromQuery]int pageSize = 10,
                                             [FromQuery]int pageIndex = 0)
        {            
            if (pageSize < 0 || pageSize > 100)
                return BadRequest("The size of the page must be a valid integer between 0 and 100!");

            if (pageIndex < 0 || pageIndex > int.MaxValue)
                return BadRequest($"The index of the page must be a valid integer between 0 and {int.MaxValue}!");

            try
            {
                var totalItems = await context.Appointments.LongCountAsync();

                var itemsOnPage = await context.Appointments
                    .AsNoTracking()
                    .OrderBy(c => c.Title)
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .ToListAsync();

                var model = new PaginatedItems<Appointment>(
                    pageIndex, pageSize, totalItems, itemsOnPage);

                return Ok(model);
            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exc.Message);
            }
        }

        /// <summary>
        /// Find upcoming appointments.
        /// </summary>          
        /// <param name="pageSize">Amount of items to retrieve. Defaults to 10. Must be valid.
        /// integer between 0 and 100.</param>
        /// <param name="period">Integer used to define the period for search (days).</param>
        /// <param name="pageIndex">The index of paginated page.</param>
        /// <returns>List of upcoming appointments.</returns>
        /// <response code="200">Success.</response>       
        /// <response code="401">Access denied.</response>          
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [Route("Upcoming")]
        [ApiVersion("1.0")]
        [ProducesResponseType(typeof(PaginatedItems<Appointment>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<IActionResult> FindUpcomingAppointments(
            [FromQuery] int period = 5,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0)
        {
            if (period < 1 || period > 530)
                return BadRequest("The period must be a valid integer between 1 and 530 days!");
            
            if (pageSize < 0 || pageSize > 100)
                return BadRequest("The size of the page must be a valid integer between 0 and 100!");

            if (pageIndex < 0 || pageIndex > int.MaxValue)
                return BadRequest($"The index of the page must be a valid integer between 0 and {int.MaxValue}!");

            try
            {
                var now = DateTimeOffset.UtcNow;
                var end = DateTimeOffset.UtcNow.AddDays(period);
                var upcomingAppointments = await context.Appointments
                    .Where(a => a.Date.AddYears(now.Year - a.Date.Year) >= now && a.Date.AddYears(now.Year - a.Date.Year) <= end)
                    .ToListAsync();

                var totalItems = upcomingAppointments.Count;

                var itemsOnPage = upcomingAppointments                    
                    .OrderBy(c => c.Date)
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .ToList();

                var model = new PaginatedItems<Appointment>(
                    pageIndex, pageSize, totalItems, itemsOnPage);

                return Ok(model);
            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exc.Message);
            }
        }

        /// <summary>
        /// Get appointment by id.
        /// </summary>   
        /// <param name="id">Unique id of appointment.</param>
        /// <returns>An appointment.</returns>
        /// <response code="200">Success.</response>      
        /// <response code="400">Invalid input parameter.</response>  
        /// <response code="401">Access denied.</response>    
        /// <response code="404">Not found.</response>    
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id}")]
        [ApiVersion("1.0")]
        [ProducesResponseType(typeof(Appointment), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("ID object is empty");
            
            try
            {
                var appointment = await context.Appointments.FirstOrDefaultAsync(x => x.Id == id);
                if (appointment == null)
                    return NotFound();

                return Ok(appointment);                                          
            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exc.Message);
            }
        }

        /// <summary>
        /// Create new appointment.
        /// </summary>   
        /// <param name="appointment">The appointment to create.</param>
        /// <returns>True if successful.</returns>
        /// <response code="200">Success.</response>    
        /// <response code="400">Invalid input parameter.</response> 
        /// <response code="401">Access denied.</response>    
        /// <response code="404">Not found.</response>    
        /// <response code="500">Internal server error.</response>
        [HttpPost()]
        [ApiVersion("1.0")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync(Appointment appointment)
        {
            if (appointment == null)
                return BadRequest("Appointment object is null");

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(appointment.Title))
                return BadRequest("Invalid model object");

            try
            {                
                var entity = await context.Appointments.AddAsync(appointment);
                var ok = await context.SaveChangesAsync() == 1;
                entity.State = EntityState.Detached;
                
                return Ok(ok);
            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    $"Failed to create new appointment. {exc.Message}");
            }
        }

        /// <summary>
        /// Update an appointment.
        /// </summary>          
        /// <param name="appointment">The appointment to update.</param>
        /// <returns>True if successful.</returns>
        /// <response code="200">Success.</response>    
        /// <response code="400">Invalid input parameter.</response> 
        /// <response code="401">Access denied.</response>   
        /// <response code="404">Not found.</response>    
        /// <response code="500">Internal server error.</response>
        [HttpPut()]
        [ApiVersion("1.0")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync(Appointment appointment)
        {
            try
            {
                if (appointment == null)
                    return BadRequest("Appointment object is null");

                if (!ModelState.IsValid || string.IsNullOrWhiteSpace(appointment.Title))
                    return BadRequest("Invalid model object");

                if (!context.Appointments.Any(x => x.Id == appointment.Id))
                    return NotFound();

                var entity = context.Appointments.Update(appointment);
                var ok = await context.SaveChangesAsync() == 1;
                entity.State = EntityState.Detached;
                return Ok(ok);
            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Failed to update an appointments with id: {appointment.Id}. {exc.Message}");
            }
        }

        /// <summary>
        /// Delete an appointment from database.
        /// </summary>   
        /// <param name="id">The unique id of the appointment.</param>
        /// <returns>Returns true if item was successful deleted from database.</returns>
        /// <response code="200">Success.</response>    
        /// <response code="401">Access denied.</response>    
        /// <response code="404">Appointment hasn't been found in database.</response>    
        /// <response code="500">Internal server error.</response>
        [HttpDelete("{id}")]
        [ApiVersion("1.0")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            try
            {
                var appointment = await context.Appointments.FindAsync(id);
                if (appointment == null)
                    return NotFound();

                var entity = context.Appointments.Remove(appointment);
                var ok = await context.SaveChangesAsync() == 1;
                entity.State = EntityState.Detached;

                return Ok(ok);
            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Failed to delete an appointment with id: {id}. {exc.Message}");
            }
        }

        /// <summary>
        /// Import a file with appointments.
        /// </summary>   
        /// <param name="userId">The user id who creates the appointment.</param>
        /// <param name="text">The text with appointments.</param>
        /// <returns>True if successful.</returns>
        /// <response code="200">Success.</response>    
        /// <response code="400">Invalid input parameter.</response> 
        /// <response code="401">Access denied.</response>    
        /// <response code="404">Not found.</response>    
        /// <response code="500">Internal server error.</response>
        [HttpPost("{userId}")]
        [ApiVersion("1.0")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ImportAsync(Guid userId, string text)
        {         
            try
            {
                var appointments = AppointmentHandler.ParseFile(userId, text);
               
                var count = 0;
                foreach (var appointment in appointments)
                {
                    var appointmentDB = await context.Appointments.FirstOrDefaultAsync(x => 
                    x.Title == appointment.Title &&
                    x.Date == appointment.Date);

                    if (appointmentDB == null)
                    {
                        await context.Appointments.AddAsync(appointment);
                        count++;
                    }
                }
                                
                var ok = await context.SaveChangesAsync() == count;                
                return Ok(ok);
            }
            catch (ArgumentNullException exc)
            {
                return BadRequest(exc.Message);
            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Failed to create new appointment. {exc.Message}");
            }
        }
    }
}