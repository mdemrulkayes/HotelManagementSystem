using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.DataModels;

namespace HotelManagementSystem.BlazorWasm.Core
{
    public interface IStripePaymentService
    {
        public Task<SuccessModel> Checkout(StripePaymentDTO model);
    }
}
