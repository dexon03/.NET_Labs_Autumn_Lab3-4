using AutoMapper;
using Lab4.Domain.Dtos;
using Lab4.Domain.Entities;

namespace Lab4.Application.Mapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<AccountDto, Account>();
        CreateMap<CashFlowDto, CashFlow>();
        CreateMap<TransactionDto, Transaction>();
    }
}