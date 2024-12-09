using AutoMapper;
using TransactionService.Models;
using TransactionService.Dtos;

namespace TransactionService.Profiles
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<Transaction, ReadTransactionDto>();
            CreateMap<CreateTransactionDto, Transaction>();
        }
    }
}
