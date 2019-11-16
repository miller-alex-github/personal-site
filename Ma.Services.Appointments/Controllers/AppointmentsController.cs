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
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<AppointmentsController> logger;

        public AppointmentsController(ApplicationDbContext dbContext, IMapper mapper, ILogger<AppointmentsController> logger)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <summary>
        /// Get list of all appointments.
        /// </summary>           
        /// <returns>List of appointments.</returns>
        /// <response code="200">Success.</response>                
        /// <response code="500">Internal server error.</response>
        [HttpGet()]
        [ApiVersion("1.0")]
        [ProducesResponseType(200)]
        [SwaggerResponse(500)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get()
        {            
            try
            {
                return Ok(await dbContext.Appointments.ToListAsync());
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
        /// <response code="500">Internal server error.</response>
        [HttpGet("{userId}")]
        [ApiVersion("1.0")]
        [ProducesResponseType(200)]
        [SwaggerResponse(500)]
        [Authorize]
        public async Task<IActionResult> Get(string userId)
        {
            try
            {
                return Ok(await dbContext.Appointments
                                .Where(p => p.UserId == userId)
                                .ProjectTo<DTO.AppointmentItem>(mapper.ConfigurationProvider).ToListAsync());                                          
            }
            catch (Exception exc)
            {
                logger.LogError("Failed to get list of appointments by user id.", exc);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
