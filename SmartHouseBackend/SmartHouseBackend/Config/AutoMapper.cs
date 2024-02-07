using AutoMapper;
using SmartHouse.Core.Model;
using SmartHouse.Core.Model.ElectromagneticDevices;
using SmartHouse.Infrastructure.DTOS;

namespace SmartHouse.Config
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<SmartProperty, UserPropertyDTO>();
            CreateMap<SmartDevice, SmartDeviceDTO>();
            CreateMap<HouseBattery, BatteryDTO>();
            CreateMap<Panel, PanelDTO>();
            CreateMap<SmartProperty, AdminProperties>();
            CreateMap(typeof(PagedList<>), typeof(PagedList<>)).ConvertUsing(typeof(PagedListConverter<,>));
        }
    }
    public class PagedListConverter<TSource, TDestination> : ITypeConverter<PagedList<TSource>, PagedList<TDestination>>
    {
        public PagedList<TDestination> Convert(PagedList<TSource> source, PagedList<TDestination> destination, ResolutionContext context)
        {
            var mappedList = context.Mapper.Map<List<TSource>, List<TDestination>>(source);
            return new PagedList<TDestination>(mappedList, source.TotalCount, source.CurrentPage, source.PageSize);
        }
    }
}
