using FluentResults;
using SmartHouse.Core.Model;
using SmartHouse.Infrastructure.DTOS;

namespace SmartHouse.Core.Interfaces.Services
{
    public interface ISmartPropertyService
    {
        Task<Result<List<CityDTO>>> GetAllCities();
        Task<Result<SinglePropertyDTO>> GetProperty(Guid id);
        Task<Result<PropertyDTO>> RegisterSmartProperty(RegisterPropertyDTO newProperty, Guid userId);
        Task<Result<PagedList<AdminProperties>>> GetPropertiesForAdmin(Guid adminId, Page page);

    }
}
