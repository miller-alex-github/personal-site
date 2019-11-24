using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ma.Services.Appointments.UnitTests
{
    internal class AppointmentsControllerFactory : IDisposable
    {
        internal readonly ApplicationDbContext Context;
        internal readonly AppointmentsController Service;

        internal Guid FakeUserID = Guid.NewGuid();
        internal List<Appointment> FakeAppointments;

        internal AppointmentsControllerFactory()
        {
            // Context
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            Context = new ApplicationDbContext(options);            
            Context.Database.EnsureDeleted(); // Ensures we have a new database with no data in it.
            Context.Database.EnsureCreated();

            // Mapper
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            var mapper = config.CreateMapper();
            
            // Logger
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();
            var factory = serviceProvider.GetService<ILoggerFactory>();
            var logger = factory.CreateLogger<AppointmentsController>();
           
            // Service of appointments
            Service = new AppointmentsController(Context, mapper, logger);
        }

        public void Dispose()
        {
            if (Context != null)            
                Context.Dispose();
        }

        internal AppointmentsControllerFactory SetFakeAppointmentsTo(int count)
        {
            if (count < 1)
                throw new ArgumentException(nameof(count));

            FakeAppointments = Enumerable.Range(0, count).Select(x => CreateFakeAppointment(x.ToString())).ToList();                  
            Context.Appointments.AddRangeAsync(FakeAppointments);
            Context.SaveChangesAsync();

            return this;
        }

        internal Appointment CreateFakeAppointment(string title = "Test1")
        {           
            return new Appointment
            {
                Id = Guid.NewGuid(),
                UserId = FakeUserID,
                Date = DateTime.UtcNow,
                Title = title
            };
        }
    }
}
