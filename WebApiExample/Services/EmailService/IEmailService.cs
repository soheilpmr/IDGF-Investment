﻿namespace IDGFAuth.Services.EmailService
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string htmlBody);
    }
}
