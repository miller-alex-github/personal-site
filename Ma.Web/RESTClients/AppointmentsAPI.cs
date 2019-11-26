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
        
        [Post("/Appointments")]
        Task<bool> AddAsync(Appointment newItem);
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

        public async Task<bool> AddAsync(Appointment newItem)
        {
            return await client.AddAsync(newItem);
        }

        public async Task<PaginatedItems<Appointment>> GetAsync(int pageSize = 10, int pageIndex = 0)
        {
            return await client.GetAsync(pageSize, pageIndex);
        }
    }
}
