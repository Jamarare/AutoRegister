﻿using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace AutoRegister.Services
{
    public class NoOpEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Console.WriteLine($"FAKE EMAIL: To={email}, Subject={subject}");
            return Task.CompletedTask;
        }
    }
}
