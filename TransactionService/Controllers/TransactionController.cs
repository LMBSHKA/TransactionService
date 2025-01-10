using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TransactionService.Data;
using TransactionService.Dtos;

namespace TransactionService.Controllers
{
    [Route("v1/transaction/")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepo _transactionRepo;
        private readonly IMapper _mapper;

        public TransactionController(ITransactionRepo transactionRepo, IMapper mapper)
        {
            _transactionRepo = transactionRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Получение всех транзакций
        /// </summary>
        /// <param name="model">транзакции</param>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Ошибка API(скоре всего неправильные данные)</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet]
        public ActionResult<IEnumerable<ReadTransactionDto>> GetAllTransactions()
        {
            var transactions = _transactionRepo.GetAllTransactions();
            return Ok(_mapper.Map<IEnumerable<ReadTransactionDto>>(transactions));
        }

        /// <summary>
        /// Получение транзакций по id-абрнента
        /// </summary>
        /// <param name="model">транзакции</param>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Ошибка API(скоре всего неправильные данные)</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("{ClientId}")]
        public ActionResult<IEnumerable<ReadTransactionDto>> GetTransactionByClientId(int ClientId)
        {
            var transaction = _transactionRepo.GetTransactionByClientId(ClientId);
           if (transaction == null)
                return NotFound();
            return Ok(_mapper.Map<IEnumerable<ReadTransactionDto>>(transaction));
        }

        /// <summary>
        /// Создание транзакции
        /// </summary>
        /// <remarks>
        /// Пример запроса (в теле запроса передать json):
        ///
        ///     POST /Todo
        ///     {
        ///        "ClientId" : 1,
        ///        "Amount" : 120,
        ///        "PaymentMethod" : "card",
        ///     }
        ///
        /// </remarks>
        /// <param name="model">транзакция</param>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Ошибка API(скоре всего неправильные данные)</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost]
        public ActionResult CreateTransaction(CreateTransactionDto createTransaction)
        {
            _transactionRepo.CreateTransaction(createTransaction);
            return Ok();
        }
    }
}
