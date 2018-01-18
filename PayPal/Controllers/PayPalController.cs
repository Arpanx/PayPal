using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PayPal.Payments;
using PayPalNew.Models;
using PayPalNew.Services;

namespace PayPalNew.Controllers
{
    public class PayPalController : Controller
    {
        private readonly IPayPalServices _payPalServices;

        public PayPalController(IPayPalServices payPalServices)
        {
            _payPalServices = payPalServices;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync()
        {
            var data = await _payPalServices.CreatePayment("sale", "");

           return new OkObjectResult(data);
        }

        [HttpPost]
        public async Task<IActionResult> ExecuteAsync([FromForm] PayVM content)
        {
            var data = await _payPalServices.ExecutePaymentAsync(content);

            return new OkObjectResult(data);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
