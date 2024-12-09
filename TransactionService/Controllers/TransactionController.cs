using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TransactionService.Data;
using TransactionService.Dtos;
using TransactionService.Models;
using TransactionService.RabbitMQ;

namespace TransactionService.Controllers
{
    [Route("v1/transaction/")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepo _transactionRepo;
        private readonly IMapper _mapper;
        private readonly IRabbitMqService _mqService;

        public TransactionController(ITransactionRepo transactionRepo, IMapper mapper, IRabbitMqService mqService)
        {
            _transactionRepo = transactionRepo;
            _mapper = mapper;
            _mqService = mqService;
        }

        [Route("[action]/{message}")]
        [HttpGet]
        public IActionResult SendMessage(string message)
        {
            _mqService.SendMessage(message);

            return Ok("Сообщение отправлено");
        }

        [HttpGet("{ClientId}")]
        public ActionResult<IEnumerable<ReadTransactionDto>> GetTransactionByClientId(int ClientId)
        {
            var transaction = _transactionRepo.GetTransactionByClientId(ClientId);
           if (transaction == null)
                return NotFound();
            return Ok(_mapper.Map<IEnumerable<ReadTransactionDto>>(transaction));
        }

        [HttpPost]
        public ActionResult CreateTransaction(CreateTransactionDto createTransaction)
        {
            _transactionRepo.CreateTransaction(createTransaction);
            _transactionRepo.SaveChange();
            return Ok();
        }
    }
}
