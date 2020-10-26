using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Core;
using Business.DataModels;
using Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HotelRoomsController : ControllerBase
    {
        private readonly IHotelRepository _hotelRepository;
        public HotelRoomsController(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetHotelRooms()
        {
            var allRooms = await _hotelRepository.GetAllHotelRooms();
            return Ok(allRooms);
        }

        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetHotelRoom(int? roomId)
        {
            if (roomId == null)
            {
                return BadRequest(
                    new ErrorModel()
                    {
                        Title = "",
                        ErrorMessage = "Invalid room Id",
                        StatusCode = StatusCodes.Status400BadRequest
                    }
                );
            }

            var roomDetails = await _hotelRepository.GetHotelRoom(roomId.Value);
            if (roomDetails == null)
            {
                return NotFound(
                    new ErrorModel()
                    {
                        Title = "",
                        ErrorMessage = "Room details not found",
                        StatusCode = StatusCodes.Status404NotFound
                    }
                );
            }

            return Ok(roomDetails);
        }

        [HttpPost("mark_as_booked")]
        public async Task<IActionResult> MarkAsRoomBooked([FromBody] HotelRoomDTO room)
        {
            var result = await _hotelRepository.MarkAsBooked(room.Id);
            if (result == true)
            {
                return Ok(new SuccessModel()
                {
                    SuccessMessage = "Room booked successfully"
                });
            }
            else
            {
                return BadRequest(new ErrorModel()
                {
                    ErrorMessage = "Room can not booked. Something wrong. please contact with administrator"
                });
            }
        }
    }
}
