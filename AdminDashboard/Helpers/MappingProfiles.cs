using AdminDashboard.Models.Application;
using AdminDashboard.Models.Auth.RoleViewModels;
using AdminDashboard.Models.Auth.UserViewModels;
using AutoMapper;
using ECommerce.Core.Entities.IdentityModule;
using ECommerce.Core.Entities.ProductModule;
using Microsoft.AspNetCore.Identity;

namespace AdminDashboard.Helpers
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			CreateMap<IdentityRole, RoleVM>();
			CreateMap<ApplicationUser, UserVM>();
			CreateMap<Product, ProductVM>()
				.ForMember(dest => dest.Brand, opts => opts.MapFrom(src => src.Brand.Name))
				.ForMember(dest => dest.Category, opts => opts.MapFrom(src => src.Category.Name));
			CreateMap<CreateOrEditProductVM, Product>().ReverseMap();
		}
	}
}
