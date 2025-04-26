using AutoMapper;
using HuynhDuyKhanh.DTO;
using HuynhDuyKhanh.Model;

namespace HuynhDuyKhanh.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Ánh xạ từ ProductDTO sang Product
            CreateMap<ProductDTO, Product>();

            // Ánh xạ từ Product sang ProductDTO
            CreateMap<Product, ProductDTO>();

            // Ánh xạ từ OrderDTO sang Order
            CreateMap<OrderDTO, Order>();

            // Ánh xạ từ Order sang OrderDTO
            CreateMap<Order, OrderDTO>();

            // Ánh xạ từ UserDTO sang User
            CreateMap<UserDTO, User>();

            // Ánh xạ từ User sang UserDTO
            CreateMap<User, UserDTO>();
        }
    }
}
