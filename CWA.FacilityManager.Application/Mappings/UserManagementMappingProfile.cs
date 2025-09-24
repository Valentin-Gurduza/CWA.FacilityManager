using AutoMapper;
using CWA.FacilityManager.Application.DTOs.UserManagement;
using CWA.FacilityManager.Domain.Models;

namespace CWA.FacilityManager.Application.Mappings
{
    public class UserManagementMappingProfile : Profile
    {
        public UserManagementMappingProfile()
        {
            // User mappings
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => 
                    string.IsNullOrEmpty($"{src.FirstName} {src.LastName}".Trim()) 
                        ? src.Email ?? src.UserName ?? "Unknown User" 
                        : $"{src.FirstName} {src.LastName}".Trim()))
                .ForMember(dest => dest.Roles, opt => opt.Ignore())
                .ForMember(dest => dest.Permissions, opt => opt.Ignore());

            CreateMap<CreateUserDto, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore());

            CreateMap<UpdateUserDto, ApplicationUser>()
                .ForMember(dest => dest.LastModifiedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore());

            // Role mappings
            CreateMap<ApplicationRole, RoleDto>()
                .ForMember(dest => dest.UserCount, opt => opt.MapFrom(src => src.UserRoles.Count(ur => ur.IsActive)))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => 
                    src.RolePermissions.Where(rp => rp.IsActive).Select(rp => rp.Permission)));

            CreateMap<CreateRoleDto, ApplicationRole>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.IsSystemRole, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.NormalizedName, opt => opt.Ignore())
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore())
                .ForMember(dest => dest.RolePermissions, opt => opt.Ignore());

            CreateMap<UpdateRoleDto, ApplicationRole>()
                .ForMember(dest => dest.LastModifiedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsSystemRole, opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedName, opt => opt.Ignore())
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore())
                .ForMember(dest => dest.RolePermissions, opt => opt.Ignore());

            // Permission mappings
            CreateMap<Permission, PermissionDto>()
                .ForMember(dest => dest.PolicyName, opt => opt.MapFrom(src => src.PolicyName))
                .ForMember(dest => dest.IsGranted, opt => opt.Ignore());

            CreateMap<CreatePermissionDto, Permission>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsSystemPermission, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.RolePermissions, opt => opt.Ignore());

            // UserRole mappings
            CreateMap<UserRole, UserRoleAssignmentDto>()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

            CreateMap<UserRoleAssignmentDto, UserRole>()
                .ForMember(dest => dest.AssignedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            // RolePermission mappings
            CreateMap<RolePermission, RolePermissionDto>()
                .ForMember(dest => dest.IsGranted, opt => opt.MapFrom(src => src.IsActive));

            CreateMap<RolePermissionDto, RolePermission>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.GrantedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsGranted))
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.Permission, opt => opt.Ignore());
        }
    }
}