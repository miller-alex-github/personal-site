using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        /// <summary>
        /// Creates a new <see cref="AppointmentsController"/>.
        /// </summary>
        /// <param name="context">The <see cref="ApplicationDbContext"/> database store.</param>
        /// <param name="mapper">The <see cref="IMapper"/> to map objects to DTOs.</param>
        public AppointmentsController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        /// <summary>
        /// Get list of all appointments.
        /// </summary>          
        /// <param name="pageSize">Page size.</param>
        /// <param name="pageIndex">Page index.</param>
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
        public async Task<IActionResult> Get([FromQuery]int pageSize = 10,
                                             [FromQuery]int pageIndex = 0)
        {            
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
                return BadRequest("UserID object is empty");
            
            try
            {
                return Ok(await context.Appointments
                        .Where(p => p.UserId == userId).AsNoTracking()
                        .ProjectTo<DTO.Appointment>(mapper.ConfigurationProvider).ToListAsync());                                          
            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Failed to get list of appointments with user id: {userId}. {exc.Message}");
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
                    return BadRequest("Appointment object is null");

                appointment.Id = id;

                if (!ModelState.IsValid || string.IsNullOrWhiteSpace(appointment.Title))
                    return BadRequest("Invalid model object");

                if (!context.Appointments.Any(x => x.Id == id))
                    return NotFound();

                var entity = context.Appointments.Update(appointment);
                var ok = await context.SaveChangesAsync() == 1;
                entity.State = EntityState.Detached;
                return Ok(ok);
            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Failed to update an appointments with id: {id}. {exc.Message}");
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
    }
}
