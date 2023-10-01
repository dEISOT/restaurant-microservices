using Restaurant.Services.ProductAPI.Models.DTO;
using Restaurant.Web.Models;

namespace Restaurant.Web.Services.Contracts
{
    public interface IBaseService : IDisposable
    {
        ResponseDto ResponseDto { get; set; }
        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
