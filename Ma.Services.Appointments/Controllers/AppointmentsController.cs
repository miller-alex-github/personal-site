using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public AppointmentsController(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        /// <summary>
        /// Get appointments by unique user id.
        /// </summary>      
        /// <remarks>GET: api/v1/appointments?userId=1234567789</remarks>
        /// <param name="userId">Unique user id.</param>
        /// <returns>List of appointments.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Required parameter <paramref name="userId"/> missing or invalid.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [ApiVersion("1.0")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [SwaggerResponse(500)]
        public async Task<IActionResult> Get([FromQuery(Name = "userId")] string userId)
        {

            if (string.IsNullOrEmpty(userId))
                return BadRequest($"{nameof(userId)}  must not be null or empty.");

            try
            {
                return Ok(await dbContext.Appointments
                                .Where(p => p.UserId == userId)
                                .ProjectTo<DTO.AppointmentItem>(mapper.ConfigurationProvider).ToListAsync());                                          
            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exc.Message); // TODO: Not secure, better to use logger.
            }
        }
    }
}
