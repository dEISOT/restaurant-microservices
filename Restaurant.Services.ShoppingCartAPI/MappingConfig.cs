using AutoMapper;

namespace Restaurant.Services.ShoppingCartAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMap()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                //config.CreateMap<ProductDto, Product>();
                //config.CreateMap<Product, ProductDto>();

            });
            return mappingConfig;
        }
    }
}
