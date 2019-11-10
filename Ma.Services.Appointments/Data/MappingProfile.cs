using AutoMapper;

namespace Ma.Services.Appointments
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AppointmentItem, DTO.AppointmentItem>();
        }
    }
}
