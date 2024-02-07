using AutoMapper;
using FluentResults;
using SmartHouse.Core.Interfaces.Services;
using SmartHouse.Core.Messages;
using SmartHouse.Core.Model;
using SmartHouse.Extensions;
using SmartHouse.Infrastructure.DTOS;
using SmartHouse.Infrastructure.Interfaces.Repositories;

namespace SmartHouse.Services
{
    public class SmartPropertyService : ISmartPropertyService
    {

        private readonly ISmartPropertyRepository _smartPropertyRepository;
        private readonly IRepositoryBase<User> _userRepository;
        private readonly IRepositoryBase<City> _cityRepository;
        private readonly IRepositoryBase<Country> _countryRepository;
        private readonly IMapper _mapper;


        public SmartPropertyService(ISmartPropertyRepository smartProprtyRepository, IRepositoryBase<User> userRepository,
            IRepositoryBase<City> cityRepository, IRepositoryBase<Country> countryRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _smartPropertyRepository = smartProprtyRepository;
            _cityRepository = cityRepository;
            _countryRepository = countryRepository;
            _mapper = mapper;


        }
        public async Task<Result<PropertyDTO>> RegisterSmartProperty(RegisterPropertyDTO newProperty, Guid userId)
        {
            var user = await _userRepository.FindById(userId);
            if (user == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("user", "id"));
            }

            var property = new SmartProperty
            {
                Name = newProperty.Name,
                Address = newProperty.Address,
                City = newProperty.City,
                Country = newProperty.Country,
                Quadrature = newProperty.Quadrature,
                NumberOfFloors = newProperty.NumberOfFloors,
                Longitude = newProperty.Longitude,
                Latitude = newProperty.Latitude,
                TypeOfProperty = newProperty.TypeOfProperty,
                IsAccepted = Activation.Pending
            };
            _smartPropertyRepository.Create(property);

            var imageFileName = "images/properties/" + user.UserName + "_" + property.Id + "." + newProperty.ImageType;
            property.PathToImage = imageFileName;

            user.Properties.Add(property);
            await _userRepository.SaveChanges();

            byte[] imageBytes = Convert.FromBase64String(newProperty.Image);


            await File.WriteAllBytesAsync(imageFileName, imageBytes);
            var returnProperty = new PropertyDTO(property);


            return Result.Ok(returnProperty);

        }

        public async Task<Result<List<CityDTO>>> GetAllCities()
        {
            var cities = await _cityRepository.FindAll();
            List<CityDTO> citiesDTO = new List<CityDTO>();
            foreach (var city in cities)
            {
                var country = _countryRepository.FindById(city.CountryId);
                CityDTO cityDTO = new CityDTO();
                cityDTO.Name = city.Name;
                cityDTO.Country = country.Result.Name;
                cityDTO.Longitude = city.Longitude;
                cityDTO.Latitude = city.Latitude;
                citiesDTO.Add(cityDTO);

            }
            return Result.Ok(citiesDTO);
        }

        public async Task<Result<SinglePropertyDTO>> GetProperty(Guid id)
        {
            var property = await _smartPropertyRepository.FindById(id);
            if (property == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFound("property", "id")); }
            SinglePropertyDTO result = new SinglePropertyDTO();
            result.Longitude = property.Longitude;
            result.Latitude = property.Latitude;
            result.Name = property.Name;
            result.Country = property.Country;
            result.City = property.City;
            result.Quadrature = property.Quadrature;
            result.NumberOfFloors = property.NumberOfFloors;
            result.Address = property.Address;
            result.Devices = property.Devices;
            result.TypeOfProperty = property.TypeOfProperty;
            result.IsAccepted = property.IsAccepted;

            return Result.Ok(result);
        }

        public async Task<Result<PagedList<AdminProperties>>> GetPropertiesForAdmin(Guid adminId, Page page)
        {
            var admin = await _userRepository.FindById(adminId);
            if (admin == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFoundUserId());
            }
            if (admin.Role != Role.SUPERADMIN && admin.Role != Role.ADMIN)
            {
                return ResultExtensions.FailBadRequest("Unauthorized for users.");

            }

            var properties = await _smartPropertyRepository.FindAllWithUser(page);
            var propertiesDTO = _mapper.Map<PagedList<AdminProperties>>(properties);
            for (int i = 0; i < properties.Count; i++)
            {
                propertiesDTO[i].OwnerName = properties[i].User.Name + " " + properties[i].User.LastName;
            }
            return Result.Ok(propertiesDTO);
        }
    }
}
