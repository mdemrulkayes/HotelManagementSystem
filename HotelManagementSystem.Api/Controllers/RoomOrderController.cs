using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Core;
using Business.Models;
using DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoomOrderController : ControllerBase
    {
        private readonly IRoomOrderDetailsRepository _repository;

        public RoomOrderController(IRoomOrderDetailsRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoomOrderDetails details)
        {
            if (ModelState.IsValid)
            {
                var result = await _repository.Create(details);
                return Ok(result);
            }
            else
            {
                return BadRequest(new ErrorModel()
                {
                    ErrorMessage = "Please fill all the fields."
                });
            }
        }

        [HttpPost("mark_payment_successful")]
        public async Task<IActionResult> PaymentSuccessful([FromBody] RoomOrderDetails details)
        {
            var result = await _repository.MarkAsPaymentSuccessful(details.Id);
            if (result == null)
            {
                return BadRequest(new ErrorModel()
                {
                    ErrorMessage = "Can not mark payment as successful"
                });
            }
            return Ok(result);
        }
    }
}
