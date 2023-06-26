# ASP-201
After cloning rename file ```appsettings.example.json``` to ```appsettings.json```
and change connection strings


{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "MsDb": "******",
    "MySqlDb": "*******"
  },
  "Smtp": {
    "Gmail": {
      "Host": "smtp.gmail.com",
      "Port": 587,
      "Email": "********",
      "Password": "******",
      "Ssl": true
    }
  }
}

^^^ paste this code into appsettings.json