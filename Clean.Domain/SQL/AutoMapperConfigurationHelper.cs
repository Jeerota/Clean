using AutoMapper;

namespace Clean.Domain.SQL
{
    public static class AutoMapperConfigurationHelper
    {
        public static MapperConfiguration GetAutoMapperConfiguration()
        {
            return new MapperConfiguration(cfg =>
            {
                //EntityToDTOMaps
                cfg.CreateMap<object, object>().ReverseMap();
            });
        }
    }
}