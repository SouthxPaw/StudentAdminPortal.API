using AutoMapper;
using StudentAdminPortal.API.DataModels;
using StudentAdminPortal.API.DomainModels;

namespace StudentAdminPortal.API.Profiles
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() 
        {
            CreateMap<Student, StudentDTO>()
                .ReverseMap();

            CreateMap<Gender, GenderDTO>()
                .ReverseMap();

            CreateMap<Address, AddressDTO>()
                .ReverseMap();
        } 
    }
}
