using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Startapp.Shared;
using Startapp.Shared.Core;
using Startapp.Shared.Models;
using Startapp.Shared.ViewModels;

namespace Shop.Server
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AppUser, UserViewModel>()
                   .ForMember(d => d.Roles, map => map.Ignore());
            CreateMap<UserViewModel, AppUser>()
                .ForMember(d => d.Roles, map => map.Ignore())
                .ForMember(d => d.Id, map => map.Condition(src => src.Id != null));

            CreateMap<AppUser, UserEditViewModel>()
                .ForMember(d => d.Roles, map => map.Ignore());
            CreateMap<UserEditViewModel, AppUser>()
                .ForMember(d => d.Roles, map => map.Ignore())
                .ForMember(d => d.Id, map => map.Condition(src => src.Id != null));

            CreateMap<AppUser, RegisterViewModel>().ReverseMap()
                     .ForMember(d => d.Id, map => map.Condition(src => src.Id != null));
            CreateMap<AppUser, ProfileVM>().ReverseMap()
                   .ForMember(d => d.Id, map => map.Condition(src => src.UserId != null));
            CreateMap<Customer, ProfileVM>().ReverseMap()
                     .ForMember(d => d.Id, map => map.Condition(src => src.UserId != null));

            CreateMap<AppUser, LoginViewModel>().ReverseMap();
            CreateMap<AppUser, LoginVM>().ReverseMap();
            //CreateMap<AppUser, RegisterVM>().ReverseMap();
            //.ForMember(d => d.Id, map => map.Condition(src => src.Id != null));

            CreateMap<AppUser, UserDTO>().ReverseMap();

            CreateMap<AppUser, RegisterViewModel>()
                .ReverseMap()
                .ForMember(d => d.Id, map => map.Condition(src => src.Id != null));

            CreateMap<AppUser, UserPatchViewModel>()
                .ReverseMap();

            CreateMap<AppRole, RoleViewModel>()
                .ForMember(d => d.Permissions, map => map.MapFrom(s => s.Claims))
                .ForMember(d => d.UsersCount, map => map.MapFrom(s => s.Users != null ? s.Users.Count : 0))
                .ReverseMap();
            CreateMap<RoleViewModel, AppRole>()
                .ForMember(d => d.Id, map => map.Condition(src => src.Id != null));

            CreateMap<IdentityRoleClaim<string>, ClaimViewModel>()
                .ForMember(d => d.Type, map => map.MapFrom(s => s.ClaimType))
                .ForMember(d => d.Value, map => map.MapFrom(s => s.ClaimValue))
                .ReverseMap();

            CreateMap<ApplicationPermission, PermissionViewModel>()
                .ReverseMap();
            CreateMap<ApplicationPermission, PermissionVM>()
               .ReverseMap();
            CreateMap<PermissionVM, PermissionViewModel>()
              .ReverseMap();

            CreateMap<IdentityRoleClaim<string>, PermissionViewModel>()
                .ConvertUsing(s => (PermissionViewModel)ApplicationPermissions.GetPermissionByValue(s.ClaimValue));

            CreateMap<Customer, CustomerViewModel>().ReverseMap();
            CreateMap<Customer, Customer>().ReverseMap();
            CreateMap<Customer, ClientInfo>().ReverseMap();

            CreateMap<Language, Language>().ReverseMap();

            CreateMap<Article, ArticleViewModel>().ReverseMap();
            CreateMap<Article, CartItem>().ReverseMap();
            CreateMap<CartVM, CartItem>().ReverseMap();
            CreateMap<CartItem, CartItemVM>()
                  .ForMember(dest => dest.Name, act => act.MapFrom(src => src.Title))
                  .ForMember(dest => dest.UnitAmount, act => act.MapFrom(src => src.SellingPrice));

                   


            CreateMap<Article, Article>().ReverseMap();

            CreateMap<Order, OrderViewModel>().ReverseMap();
        }
    }
}
