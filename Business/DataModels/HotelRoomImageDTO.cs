﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Business.DataModels
{
    public class HotelRoomImageDTO
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string RoomImageUrl { get; set; }
    }
}