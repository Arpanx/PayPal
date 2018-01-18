using BraintreeHttp;
using PayPal.Payments;
using PayPalNew.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayPalNew.Services
{
    public interface IPayPalServices
    {
        Task<Payment> CreatePayment(string intent, string paymentMethod = "credit_card", string invoiceNumber = null);
        Task<Payment> ExecutePaymentAsync(PayVM payVM);
    }
}
