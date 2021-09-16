using DAL.Model;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using OB_BE_dotnet.Dress.DTO;

namespace OB_BE_dotnet
{
    class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            CreateMap<DressModel, DressDTO>();
            CreateMap<DressDTO, DressModel>();
        }
    }
}

