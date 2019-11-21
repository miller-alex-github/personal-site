using Ma.Web.Services;
using Microsoft.Extensions.Configuration;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Ma.Web
{
    public interface IAppointmentsAPI
    {
        [Get("/Appointments")]
        Task<IEnumerable<Appointment>> GetAppointments();

        [Get("/Appointments/{userId}")]
        Task<IEnumerable<Appointment>> GetAppointmentsByUserId([AliasAs("userId")] string userId);
    }

    public class AppointmentsAPI : IAppointmentsAPI
    {
        private readonly IAppointmentsAPI client;

        public AppointmentsAPI(IConfiguration config, HttpClient httpClient, IJsonWebTokenService jsonWebTokenService)
        {
            var token = jsonWebTokenService.Create();
            httpClient.BaseAddress = new Uri(config["ApiGateway"]);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client = RestService.For<IAppointmentsAPI>(httpClient);
        }

        public async Task<IEnumerable<Appointment>> GetAppointments()
        {
            return await client.GetAppointments();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByUserId([AliasAs("userId")] string userId)
        {
            return await client.GetAppointmentsByUserId(userId);
        }
    }
}
