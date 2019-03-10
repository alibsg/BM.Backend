using AutoMapper;
using BM.BackEnd.Models.Entities;
using BM.BackEnd.DTOs;

public class AutoMapperProfile: Profile{
    public AutoMapperProfile(){
        CreateMap<User,UserDTO>();
        CreateMap<UserDTO,User>();
    }
}


