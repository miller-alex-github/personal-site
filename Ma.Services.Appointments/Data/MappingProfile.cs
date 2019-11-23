using AutoMapper;

namespace Ma.Services.Appointments
{
    /// <summary>
    /// Represents the mapper from object to DTO (Date Transfer Object)
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Creates a new <see cref="MappingProfile"/>.
        /// </summary>        
        public MappingProfile()
        {
            CreateMap<Appointment, DTO.Appointment>();
        }
    }
}
