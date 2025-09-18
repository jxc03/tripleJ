using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp1.Components.Services
{
    public class EmailSettings
    {
        /// SMTP server address (e.g., smtp.gmail.com for Gmail)
        public string SmtpServer { get; set; } = string.Empty;
        
        /// SMTP port number (587 for TLS, 465 for SSL)
        public int SmtpPort { get; set; }
        
        /// Display name for the sender
        public string SenderName { get; set; } = string.Empty;
        
        /// Email address that will appear as the sender
        public string SenderEmail { get; set; } = string.Empty;
        
        /// Gmail account username (usually same as email)
        public string Username { get; set; } = string.Empty;
        
        /// App specific password generated from Google Account
        public string Password { get; set; } = string.Empty;
    }
}