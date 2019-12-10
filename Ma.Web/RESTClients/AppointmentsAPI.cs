using Ma.Shared;
using Ma.Web.Services;
using Microsoft.Extensions.Configuration;
using Refit;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Ma.Web
{
    public interface IAppointmentsAPI
    {
        [Get("/Appointments?pageSize={pageSize}&pageIndex={pageIndex}")]
        Task<PaginatedItems<Appointment>> GetAsync(int pageSize = 10, int pageIndex = 0);

        [Get("/Appointments/{id}")]
        Task<Appointment> GetByIdAsync(Guid id);

        [Post("/Appointments")]
        Task<bool> CreateAsync(Appointment appointment);

        [Put("/Appointments")]
        Task<bool> UpdateAsync(Appointment appointment);

        [Delete("/Appointments/{id}")]
        Task<bool> DeleteAsync(Guid id);
               
        [Post("/Appointments/{userId}")]
        Task<bool> ImportAsync(Guid userId, string text);
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

        public async Task<PaginatedItems<Appointment>> GetAsync(int pageSize = 10, int pageIndex = 0) 
            => await client.GetAsync(pageSize, pageIndex);

        public async Task<Appointment> GetByIdAsync(Guid id)
            => await client.GetByIdAsync(id);

        public async Task<bool> CreateAsync(Appointment appointment) 
            => await client.CreateAsync(appointment);

        public async Task<bool> UpdateAsync(Appointment appointment)
            => await client.UpdateAsync(appointment);

        public async Task<bool> DeleteAsync(Guid id)
            => await client.DeleteAsync(id);

        public async Task<bool> ImportAsync(Guid userId, string text)
            => await client.ImportAsync(userId, text);
    }
}
