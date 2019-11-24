using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Ma.Services.Appointments.UnitTests
{
    public class AppointmentsControllerTests
    {
        [Fact(DisplayName = "Get all appointments")]
        public async Task Get_Appointments_OK()
        {
            using var factory = new AppointmentsControllerFactory()
                .SetFakeAppointmentsTo(3);
            
            (await factory.Service.Get())
                .Should().BeOfType<OkObjectResult>().Subject.Value
                .Should().BeEquivalentTo(factory.FakeAppointments);
        }

        [Fact(DisplayName = "Get all appointments by user id")]
        public async Task GetByUserId_Appointments_OK()
        {
            using var factory = new AppointmentsControllerFactory()
                .SetFakeAppointmentsTo(3);

            (await factory.Service.GetByUserId(factory.FakeUserID))
                .Should().BeOfType<OkObjectResult>().Subject.Value
                .Should().BeEquivalentTo(factory.FakeAppointments, options =>
                    options.ExcludingMissingMembers()); // because DTO has less propertys as original
        }

        [Fact(DisplayName = "Get all appointments by invalid user id")]
        public async Task GetByUserId_AppointmentsForEmptyUserId_BadRequest()
        {
            using var factory = new AppointmentsControllerFactory()
                .SetFakeAppointmentsTo(1);

            (await factory.Service.GetByUserId(Guid.Empty))
                .Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact(DisplayName = "Create an appointment")]
        public async Task Create_Appointment_OK()
        {
            using var factory = new AppointmentsControllerFactory();
            var fakeAppointment = factory.CreateFakeAppointment();

            await factory.Service.CreateAsync(fakeAppointment);

            (await factory.Context.Appointments.FirstAsync())
                .Should().BeEquivalentTo(fakeAppointment);
        }

        [Fact(DisplayName = "Create an invalid appointment")]
        public async Task Create_InvalidAppointment_BadRequest()
        {
            using var factory = new AppointmentsControllerFactory();
            var fakeAppointment = factory.CreateFakeAppointment();

            fakeAppointment.Title = string.Empty; 
            (await factory.Service.CreateAsync(fakeAppointment))
                .Should().BeOfType<BadRequestObjectResult>();

            fakeAppointment.Title = null;
            (await factory.Service.CreateAsync(fakeAppointment))
                .Should().BeOfType<BadRequestObjectResult>();

            (await factory.Service.CreateAsync(null))
                .Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact(DisplayName = "Update an appointment")]
        public async Task Update_Appointment_OK()
        {
            using var factory = new AppointmentsControllerFactory();
            var A = factory.CreateFakeAppointment("A");
            var B = factory.CreateFakeAppointment("B");
            
            (await factory.Service.CreateAsync(A))
                .Should().BeOfType<OkObjectResult>();

            (await factory.Service.UpdateAsync(A.Id, B))
                 .Should().BeOfType<OkObjectResult>();

            (await factory.Context.Appointments.FirstAsync())
                .Should().BeEquivalentTo(B);
        }

        [Fact(DisplayName = "Delete an appointment")]
        public async Task Delete_Appointment_OK()
        {           
            using var factory = new AppointmentsControllerFactory()
                .SetFakeAppointmentsTo(1);

            (await factory.Service.DeleteAsync(factory.FakeAppointments[0].Id))
                .Should().BeOfType<OkObjectResult>();
        }
    }
}
