# tripleJ

# Contact Form
- Email Implementation
- - Email settings to appsettings.json added to store data
- - Email settings configuration model .cs added for storing email settings data from appsettings.json
- - Email service .cs to implement the connection between MailKit and Gmail SMTP
- - Register the services in program.cs to allow it to be injected
- - Update HandleValidSubmit to use the email service
- - User secrets then leave empty the secret 