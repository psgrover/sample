using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using GroLife.InvoiceEngine.Interfaces.Service;
using GroLife.InvoiceEngine.DataTransferObjects;

namespace GroLife.InvoiceEngine.Api.Controllers
{
    /// <summary>
    /// Payments Controller for Payments Micro Services API.
    /// </summary>
    [Route("api/[controller]")]
    public class PaymentsController : Controller
    {
        private readonly IPaymentService _entityService;
        private readonly ILogger<PaymentsController> _logger;

        /// <summary>
        /// Constructor for PaymentsController.
        /// </summary>
        /// <param name="entityService">IPaymentService object parameter</param>
        /// <param name="logger">ILogger&lt;PaymentsController&gt; logger object</param>
        public PaymentsController(IPaymentService entityService, ILogger<PaymentsController> logger)
        {
            _entityService = entityService;
            _logger = logger;
        }

        /// <summary>
        /// GET list of Payments: api/payments
        /// </summary>
        /// <returns>List of Payment DTO objects</returns>
        [HttpGet]
        public IServiceResponse<IEnumerable<PaymentDTO>> Get()
        {
            _logger.LogDebug($"{nameof(PaymentsController)}.{nameof(Get)}");
            return _entityService.Get();
        }

        /// <summary>
        /// GET Payment by ID: api/payments/1
        /// </summary>
        /// <param name="id">ID key of the Payment</param>
        /// <returns>Payment DTO</returns>
        [HttpGet("{id}")]
        public IServiceResponse<PaymentDTO> Get(int id)
        {
            _logger.LogDebug($"{nameof(PaymentsController)}.{nameof(Get)} - id: '{id}'");
            return _entityService.GetById(id);
        }

        /// <summary>
        /// POST Payment api/payments
        /// </summary>
        [HttpPost]
        public IServiceResponse<PaymentDTO> Post([FromBody]PaymentDTO entity)
        {
            _logger.LogDebug($"{nameof(PaymentsController)}.{nameof(Post)} - entity: '{entity}'");
            return _entityService.Add(entity);
        }

        /// <summary>
        /// PUT Payment value for ID: api/payments/
        /// </summary>
        [HttpPut]
        public IServiceResponse<PaymentDTO> Put([FromBody]PaymentDTO entity)
        {
            _logger.LogDebug($"{nameof(PaymentsController)}.{nameof(Put)} - entity: '{entity}'");
            return _entityService.Update(entity);
        }

        /// <summary>
        /// POST Payment for invoice: api/payments/ReceivePaymentsForInvoices/
        /// </summary>
        /// <param name="entity">ProcessPaymentDTO object</param>
        /// <returns>list of ProcessInvoicePaymentDTO</returns>
        [HttpPost("ReceivePaymentsForInvoices")]
        public IServiceResponse<IEnumerable<ProcessInvoicePaymentDTO>> ReceivePaymentsForInvoices([FromBody]ProcessPaymentDTO entity)
        {
            _logger.LogDebug($"{nameof(PaymentsController)}.{nameof(ReceivePaymentsForInvoices)} - entity: '{entity}'");
            return _entityService.ReceivePaymentsForInvoices(entity, isUpdate: false);
        }

        /// <summary>
        /// PUT Payment for invoice: api/payments/UpdatePaymentsForInvoices/
        /// </summary>
        /// <param name="entity">ProcessPaymentDTO object</param>
        /// <returns>list of ProcessInvoicePaymentDTO</returns>
        [HttpPut("UpdatePaymentsForInvoices")]
        public IServiceResponse<IEnumerable<ProcessInvoicePaymentDTO>> UpdatePaymentsForInvoices([FromBody]ProcessPaymentDTO entity)
        {
            _logger.LogDebug($"{nameof(PaymentsController)}.{nameof(UpdatePaymentsForInvoices)} - entity: '{entity}'");
            return _entityService.ReceivePaymentsForInvoices(entity, isUpdate: true);
        }

        /// <summary>
        /// Reverses the Payment and associated invoice payments based on given payment id.
        /// POST api/Payments/ReversalById/1
        /// </summary>
        /// <param name="id">PaymentId key for Payment Reversal</param>
        /// <returns>IActionResult response.</returns>
        [HttpPost("ReversalById/{id}")]
        [Produces(typeof(IServiceResponse<PaymentDTO>))]
        public IActionResult ReversalById(int id)
        {
            _logger.LogDebug($"{nameof(PaymentsController)}.{nameof(ReversalById)} - reversalPaymentId: '{id}'");

            //Call Reverse payment service method instead of delete.
            var response = _entityService.ReversePayment(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Reverses the Payment and associated invoice payments based on given payment reference.
        /// POST api/Payments/ReversalByReference/abc
        /// </summary>
        /// <param name="key">Payment Reference key for Payment Reversal</param>
        /// <returns>IActionResult response.</returns>
        [HttpPost("ReversalByReference/{key}")]
        [Produces(typeof(IServiceResponse<PaymentDTO>))]
        public IActionResult ReversalByReference(string key)
        {
            _logger.LogDebug($"{nameof(PaymentsController)}.{nameof(ReversalByReference)} - reversalPaymentReferenceKey: '{key}'");

            //Call Reverse payment service method instead of delete.
            var response = _entityService.ReversePayment(key);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }


        /// <summary>
        /// Use Credit for Payment of invoice(s) and Adjust remaining amount. 
        /// </summary>
        /// <param name="processPaymentDto">ProcessPaymentDTO object</param>
        /// <returns>List of ProcessInvoicePaymentDTO in service reponse</returns>
        [HttpPost("UseCreditForPayment")]
        public IServiceResponse<IEnumerable<ProcessInvoicePaymentDTO>> UseCreditForPayment([FromBody]ProcessPaymentDTO processPaymentDto)
        {
            _logger.LogDebug($"{nameof(PaymentsController)}.{nameof(UseCreditForPayment)} - entity: '{processPaymentDto}'");
            return _entityService.UseCreditForPayment(processPaymentDto);
        }


    }
}