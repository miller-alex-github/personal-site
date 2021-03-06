﻿using FluentAssertions;
using Ma.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
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
                .Should().BeAssignableTo<PaginatedItems<Appointment>>().Subject.Data
                .Should().BeEquivalentTo(factory.FakeAppointments);
        }

        [Fact(DisplayName = "Get a part of paginated appointments")]
        public async Task Get_PaginatedAppointments_OK()
        {
            using var factory = new AppointmentsControllerFactory()
                .SetFakeAppointmentsTo(14);

            var sorted = factory.FakeAppointments.OrderBy(c => c.Title).ToList();
            var page0 = sorted.GetRange(0, 5);
            var page1 = sorted.GetRange(5, 5);
            var page2 = sorted.GetRange(10, 4); // Attention: only 4 items!

            var result0 = (await factory.Service.Get(pageSize: 5, pageIndex: 0))
                .Should().BeOfType<OkObjectResult>().Subject.Value
                .Should().BeAssignableTo<PaginatedItems<Appointment>>().Subject;
            result0.PageIndex.Should().Be(0);
            result0.PageSize.Should().Be(5);
            result0.TotalCount.Should().Be(14);
            result0.Data.Should().BeEquivalentTo(page0);

            var result1 = (await factory.Service.Get(pageSize: 5, pageIndex: 1))
                .Should().BeOfType<OkObjectResult>().Subject.Value
                .Should().BeAssignableTo<PaginatedItems<Appointment>>().Subject;
            result1.PageIndex.Should().Be(1);
            result1.PageSize.Should().Be(5);
            result1.TotalCount.Should().Be(14);
            result1.Data.Should().BeEquivalentTo(page1);

            var result2 = (await factory.Service.Get(pageSize: 5, pageIndex: 2))
                .Should().BeOfType<OkObjectResult>().Subject.Value
                .Should().BeAssignableTo<PaginatedItems<Appointment>>().Subject;
            result2.PageIndex.Should().Be(2);
            result2.PageSize.Should().Be(5);
            result2.TotalCount.Should().Be(14);
            result2.Data.Should().BeEquivalentTo(page2);
        }

        [Fact(DisplayName = "Find upcoming paginated appointments")]
        public async Task FindUpcomingAppointments_PaginatedAppointments_OK()
        {
            using var factory = new AppointmentsControllerFactory()
                .SetFakeAppointmentsTo(14);

            var sorted = factory.FakeAppointments.OrderBy(c => c.Date).ToList();
            var page0 = sorted.GetRange(0, 5);
            var page1 = sorted.GetRange(5, 5);
            var page2 = sorted.GetRange(10, 4); // Attention: only 4 items!

            var result0 = (await factory.Service.FindUpcomingAppointments(period: 1, pageSize: 5, pageIndex: 0))
                .Should().BeOfType<OkObjectResult>().Subject.Value
                .Should().BeAssignableTo<PaginatedItems<Appointment>>().Subject;
            result0.PageIndex.Should().Be(0);
            result0.PageSize.Should().Be(5);
            result0.TotalCount.Should().Be(14);
            result0.Data.Should().BeEquivalentTo(page0);

            var result1 = (await factory.Service.FindUpcomingAppointments(period: 1, pageSize: 5, pageIndex: 1))
                .Should().BeOfType<OkObjectResult>().Subject.Value
                .Should().BeAssignableTo<PaginatedItems<Appointment>>().Subject;
            result1.PageIndex.Should().Be(1);
            result1.PageSize.Should().Be(5);
            result1.TotalCount.Should().Be(14);
            result1.Data.Should().BeEquivalentTo(page1);

            var result2 = (await factory.Service.FindUpcomingAppointments(period: 1, pageSize: 5, pageIndex: 2))
                .Should().BeOfType<OkObjectResult>().Subject.Value
                .Should().BeAssignableTo<PaginatedItems<Appointment>>().Subject;
            result2.PageIndex.Should().Be(2);
            result2.PageSize.Should().Be(5);
            result2.TotalCount.Should().Be(14);
            result2.Data.Should().BeEquivalentTo(page2);
        }

        [Fact(DisplayName = "Get appointment by id")]
        public async Task GetById_Appointment_OK()
        {
            using var factory = new AppointmentsControllerFactory()
                .SetFakeAppointmentsTo(3);

            (await factory.Service.GetById(factory.FakeAppointments.LastOrDefault().Id))
                .Should().BeOfType<OkObjectResult>().Subject.Value
                .Should().BeEquivalentTo(factory.FakeAppointments.LastOrDefault());
        }

        [Fact(DisplayName = "Get appointment by invalid id")]
        public async Task GetById_AppointmentForEmptyId_BadRequest()
        {
            using var factory = new AppointmentsControllerFactory()
                .SetFakeAppointmentsTo(1);

            (await factory.Service.GetById(Guid.Empty))
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
            
            (await factory.Service.CreateAsync(A))
                .Should().BeOfType<OkObjectResult>();

            A.Title = nameof(Update_Appointment_OK);

            (await factory.Service.UpdateAsync(A))
                 .Should().BeOfType<OkObjectResult>();

            (await factory.Context.Appointments.FirstAsync())
                .Should().BeEquivalentTo(A);
        }

        [Fact(DisplayName = "Delete an appointment")]
        public async Task Delete_Appointment_OK()
        {           
            using var factory = new AppointmentsControllerFactory()
                .SetFakeAppointmentsTo(1);

            (await factory.Service.DeleteAsync(factory.FakeAppointments[0].Id))
                .Should().BeOfType<OkObjectResult>();
        }

        [Fact(DisplayName = "Import file of appointments")]
        public async Task Import_File_OK()
        {
            var content = "01.01.1900 A\r\n01.01.1900 A, B\r\n01.01.1901 A, B";            
            using var factory = new AppointmentsControllerFactory();
           
            (await factory.Service.ImportAsync(factory.FakeUserID, content))
                .Should().BeOfType<OkObjectResult>();

            var first = await factory.Context.Appointments.FirstAsync();
            first.Title.Should().Be("A");
            first.Date.Should().Be(DateTimeOffset.Parse("01.01.1900"));

            var last = await factory.Context.Appointments.LastAsync();
            last.Title.Should().Be("A, B");
            last.Date.Should().Be(DateTimeOffset.Parse("01.01.1901"));

            (await factory.Service.ImportAsync(Guid.Empty, null))
                .Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
