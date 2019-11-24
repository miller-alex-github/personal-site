using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
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
        private readonly IMapper mapper;
        private readonly ILogger<AppointmentsController> logger;

        /// <summary>
        /// Creates a new <see cref="AppointmentsController"/>.
        /// </summary>
        /// <param name="context">The <see cref="ApplicationDbContext"/> database store.</param>
        /// <param name="mapper">The <see cref="IMapper"/> to map objects to DTOs.</param>
        /// <param name="logger">The <see cref="ILogger"/> to log the workflow.</param>
        public AppointmentsController(ApplicationDbContext context, IMapper mapper, ILogger<AppointmentsController> logger)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get list of all appointments.
        /// </summary>           
        /// <returns>List of appointments.</returns>
        /// <response code="200">Success.</response>       
        /// <response code="401">Access denied.</response>          
        /// <response code="500">Internal server error.</response>
        [HttpGet()]
        [ApiVersion("1.0")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [SwaggerResponse(500)]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Get()
        {            
            try
            {
                return Ok(await context.Appointments.AsNoTracking().ToListAsync());
            }
            catch (Exception exc)
            {
                logger.LogError("Failed to get list of appointments.", exc);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Get list of appointments by user id.
        /// </summary>   
        /// <param name="userId">Unique user id.</param>
        /// <returns>List of appointments.</returns>
        /// <response code="200">Success.</response>      
        /// <response code="400">Invalid input parameter.</response>  
        /// <response code="401">Access denied.</response>    
        /// <response code="500">Internal server error.</response>
        [HttpGet("{userId}")]
        [ApiVersion("1.0")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [SwaggerResponse(500)]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                logger.LogError("UserID object sent from client is empty.");
                return BadRequest("UserID object is empty");
            }

            try
            {
                return Ok(await context.Appointments
                                .Where(p => p.UserId == userId).AsNoTracking()
                                .ProjectTo<DTO.Appointment>(mapper.ConfigurationProvider).ToListAsync());                                          
            }
            catch (Exception exc)
            {
                logger.LogError($"Failed to get list of appointments with user id: {userId}.", exc);
                return StatusCode(StatusCodes.Status500InternalServerError);
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
        /// <response code="500">Internal server error.</response>
        [HttpPost()]
        [ApiVersion("1.0")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [SwaggerResponse(500)]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(Appointment appointment)
        {
            if (appointment == null)
            {
                logger.LogError("Appointment object sent from client is null.");
                return BadRequest("Appointment object is null");
            }

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(appointment.Title))
            {
                logger.LogError("Invalid appointment object sent from client.");
                return BadRequest("Invalid model object");
            }

            try
            {                
                var entity = await context.Appointments.AddAsync(appointment);
                var ok = await context.SaveChangesAsync() == 1;
                entity.State = EntityState.Detached;
                
                return Ok(ok);
            }
            catch (Exception exc)
            {
                logger.LogError("Failed to add new appointments", exc);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update an appointment.
        /// </summary>   
        /// <param name="id">The unique id of the appointment.</param>
        /// <param name="appointment">The appointment to update.</param>
        /// <returns>True if successful.</returns>
        /// <response code="200">Success.</response>    
        /// <response code="400">Invalid input parameter.</response> 
        /// <response code="401">Access denied.</response>    
        /// <response code="500">Internal server error.</response>
        [HttpPut("{id}")]
        [ApiVersion("1.0")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [SwaggerResponse(500)]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody]Appointment appointment)
        {
            try
            {
                if (appointment == null)
                {
                    logger.LogError("Appointment object sent from client is null.");
                    return BadRequest("Appointment object is null");
                }

                appointment.Id = id;

                if (!ModelState.IsValid || string.IsNullOrWhiteSpace(appointment.Title))
                {
                    logger.LogError("Invalid appointment object sent from client.");
                    return BadRequest("Invalid model object");
                }

                if (!context.Appointments.Any(x => x.Id == id))
                {
                    logger.LogError($"Appointment with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                var entity = context.Appointments.Update(appointment);
                var ok = await context.SaveChangesAsync() == 1;
                entity.State = EntityState.Detached;
                return Ok(ok);
            }
            catch (Exception exc)
            {
                logger.LogError($"Failed to update an appointments with id: {id}", exc);
                return StatusCode(StatusCodes.Status500InternalServerError);
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
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [SwaggerResponse(500)]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            try
            {
                var appointment = await context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    logger.LogError($"Appointment with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                var entity = context.Appointments.Remove(appointment);
                var ok = await context.SaveChangesAsync() == 1;
                entity.State = EntityState.Detached;

                return Ok(ok);
            }
            catch (Exception exc)
            {
                logger.LogError($"Failed to delete an appointment with id: {id}", exc);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
