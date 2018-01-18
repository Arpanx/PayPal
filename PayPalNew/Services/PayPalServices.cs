using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BraintreeHttp;
using PayPal.Core;
using PayPal.PaymentExperience;
using PayPal.Payments;
using PayPal.Invoices;
using PayPalNew.Models;
using Microsoft.Extensions.Configuration;

namespace PayPalNew.Services
{
    public class PayPalServices : IPayPalServices
    {
        public PayPalServices(
            IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected static string _skypeLogin;
        private readonly IConfiguration configuration;

        public async Task<Payment> CreatePayment(string intent, string paymentMethod = "credit_card", string invoiceNumber = null)
        {
            string ClientID = this.configuration.GetSection("ClientID").Get<string>();
            string Secret = this.configuration.GetSection("Secret").Get<string>();
            var environment = new SandboxEnvironment(ClientID, Secret);

            var client = new PayPalHttpClient(environment);
            var request = new PaymentCreateRequest();
            request.RequestBody(BuildRequestBody(intent, paymentMethod, invoiceNumber));
            
            try
            {
                HttpResponse response = await client.Execute(request);
                var statusCode = response.StatusCode;
                Payment result = response.Result<Payment>();
                return result;
            }
            catch (HttpException httpException)
            {
                var statusCode = httpException.StatusCode;
                var debugId = httpException.Headers.GetValues("PayPal-Debug-Id").FirstOrDefault();
                return new Payment();
            }
        }

        public static Payment BuildRequestBody(string intent, string paymentMethod = "credit_card", string invoiceNumber = null)
        {
            var body = new Payment()
            {
                Intent = intent,
                Transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Amount = new Amount()
                        {
                            Total = "10",
                            Currency = "USD"
                        }
                    }
                },
                RedirectUrls = new RedirectUrls()
                {
                    CancelUrl = "http://paypal.com/cancel",
                    ReturnUrl = "http://paypal.com/return"
                }
            };

            if (invoiceNumber != null)
            {
                body.Transactions[0].InvoiceNumber = invoiceNumber;
            }

            if (paymentMethod.Equals("credit_card"))
            {
                body.Payer = new Payer()
                {
                    PaymentMethod = "credit_card",
                    FundingInstruments = new List<FundingInstrument>()
                    {
                        new FundingInstrument()
                        {
                            CreditCard = new CreditCard()
                            {
                                BillingAddress = new PayPal.Payments.Address()
                                {
                                    Line1 = "123 Townsend st",
                                    Line2 = "Suite 600",
                                    City = "San Francisco",
                                    State = "CA",
                                    CountryCode = "US",
                                    PostalCode = "94117"
                                },
                                Cvv2 = "111",
                                ExpireMonth = 1,
                                ExpireYear = 2020,
                                FirstName = "Joe",
                                LastName = "Shopper",
                                Type = "visa",
                                Number = "4422009910903049"
                            }
                        }
                    }
                };
            }
            else
            {
                body.Payer = new Payer()
                {
                    PaymentMethod = "paypal"
                };
            }

            return body;
        }

        public async Task<Payment> ExecutePaymentAsync(PayVM payVM)
        {
            var paymentId = payVM.PaymentID;
            
            PaymentExecuteRequest request = new PaymentExecuteRequest(paymentId);

            var body =  new PaymentExecution()
            {
                PayerId = payVM.PayerID
            };

            request.RequestBody(body);

            string ClientID = this.configuration.GetSection("ClientID").Get<string>();
            string Secret = this.configuration.GetSection("Secret").Get<string>();
            var environment = new SandboxEnvironment(ClientID, Secret);

            var client = new PayPalHttpClient(environment);
            try
            {
                HttpResponse response = await client.Execute(request);
                var statusCode = response.StatusCode;
                Payment result = response.Result<Payment>();
                return result;
            }
            catch (HttpException httpException)
            {
                var statusCode = httpException.StatusCode;
                var debugId = httpException.Headers.GetValues("PayPal-Debug-Id").FirstOrDefault();
                return new Payment();
            }

        }
    }
}
