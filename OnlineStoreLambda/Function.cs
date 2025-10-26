using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OnlineStoreLambda.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Logging.Console;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace OnlineStoreLambda;

public class ApiGatewayRequest
{
    public string Body { get; set; } = string.Empty;
}

public class Function
{
    private readonly MailService _mailService;
    private readonly ILogger<Function> _logger;

    public Function()
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        _mailService = serviceProvider.GetRequiredService<MailService>();
        _logger = serviceProvider.GetRequiredService<ILogger<Function>>();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        var configBuilder = new ConfigurationBuilder()
            .AddEnvironmentVariables();

        var configuration = configBuilder.Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
        });
        services.AddSingleton<MailService>();
    }

    /// <summary>
    /// Function that processes the incoming sender data and sends an email to a predefined mailing list.
    /// </summary>
    /// <param name="senderData"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<string> FunctionHandler(Stream inputStream, ILambdaContext context)
    {
        using var reader = new StreamReader(inputStream);
        var rawRequest = await reader.ReadToEndAsync();
        var apiRequest = JsonConvert.DeserializeObject<ApiGatewayRequest>(rawRequest);
        SenderData senderData = JsonConvert.DeserializeObject<SenderData>(apiRequest?.Body ?? "");
        var mailingList = new MailingList();
        var personalEmail = Environment.GetEnvironmentVariable("PERSONAL_EMAIL");
        mailingList.List = new List<Recipient>()
        {
            new Recipient() { Email = personalEmail, Name = "Bernardo Mondragon Brozon"}
        };
        string res = await SendMessageFromVisitor(mailingList.List, senderData);
        return res;
    }

    private Task<string> SendMessageFromVisitor(List<Recipient> recipients, SenderData senderData)
    {
        string result = "";
        try
        {
            foreach (var recipient in recipients)
            {
                var personalEmail = Environment.GetEnvironmentVariable("PERSONAL_EMAIL");
                _logger.LogInformation("Sending email from: {FromEmail}", personalEmail);
                _logger.LogInformation("Sending email to: {RecipientName} :: {RecipientEmail}", recipient.Name, recipient.Email);

                var subject = "Message from visitor";
                var plainTextContent = $"Sender: {senderData.SenderFirstName} {senderData.SenderLastName}\nEmail: {senderData.SenderEmailAddress}\nPhone: {senderData.SenderPhoneNumber}\nMessage: {senderData.SenderMessage}";
                var htmlContent = $@"<!DOCTYPE html>
                    <html lang=""en"">
                        <head>
                            <link href=""https://fonts.googleapis.com/css2?family=Nunito:wght@200;400;700;800;900&display=swap"" rel=""stylesheet"">
                            <style>
                                body {{
                                    margin: 0px;
                                    font-family: 'Montserrat', 'open sans';
                                }}
                                -webkit-any-link {{
                                    color: white;
                                }}
                                a:-webkit-any-link {{
                                    color: white;
                                }}
                                .main-container {{
                                    max-width: 1280px;
                                    margin: auto;
                                }}
                                .card-container {{
                                    margin: 30px 15px;
                                    margin-bottom: 60px;
                                    padding: 20px;
                                    border-radius: 30px;
                                    box-shadow: 0px 0px 10px -5px rgba(0,0,0,0.75);
                                }}
                                .image-container {{
                                    text-align: center;
                                }}
                                .image {{
                                    max-width: 200px;
                                }}
                                .title {{
                                    margin: auto;
                                    color: #8e8f90;
                                    text-align: center;
                                    font-weight: 600;
                                    font-size: 24px;
                                    margin-top: 30px;
                                }}
                                .email-letter {{
                                    max-width: 800px;
                                    margin: auto;
                                    margin-top: 60px;
                                    margin-bottom: 60px;
                                }}
                                .salutation {{
                                    margin-bottom: 30px;
                                    font-weight: 800;
                                    font-size: 28px;
                                    color: #31445a;
                                }}
                                .letter-body {{
                                    font-style: normal;
                                    font-weight: 500;
                                    font-size: 18px;
                                    align-items: center;
                                    color: #6D7278;
                                }}
                                .closing {{
                                    text-align: end;
                                    margin-top: 30px;
                                }}
                                .sender-name {{
                                    font-size: 18px;
                                    font-weight: bold;
                                    color: #31445a;
                                }}
                                .sender-title {{
                                    color: #31445a;
                                }}
                                .sender-phone {{
                                    color: #6D7278;
                                }}
                                .sender-website {{
                                    color: white !important;
                                    background-color: #31445a;
                                    padding-top: 2px;
                                    padding-bottom: 2px;
                                    padding-right: 5px;
                                    padding-left: 5px;
                                    display: inline-block;
                                }}
                                .divider {{
                                    width: 100%;
                                    height: 1px;
                                    background-color: gainsboro;
                                    margin-bottom: 30px;
                                }}
                                .terms-container {{
                                    max-width: 800px;
                                    margin: auto;
                                    margin-top: 30px;
                                    margin-bottom: 30px;
                                }}
                                .terms-and-conditions {{
                                    color: #8e8f90;
                                    font-style: normal;
                                    font-weight: 500;
                                    font-size: 12px;
                                    align-items: center;
                                }}
                                .small-letters {{
                                    margin-top: 30px;
                                }}
                            </style>
                        </head>
                        <body style=""margin: 0px; font-family: 'Montserrat', 'open sans';"">
                            <div style=""max-width: 1280px; margin: auto;"" class=""main-container"">
                                <div style=""margin: 30px 15px; margin-bottom: 60px; padding: 20px; border-radius: 30px; box-shadow: 0px 0px 10px -5px rgba(0,0,0,0.75);"" class=""card-container"">
                                    <div style=""margin: auto; color: #8e8f90; text-align: center; font-weight: 600; font-size: 24px; margin-top: 30px;"" class=""title"">Message from visitor</div>
                                    <div style=""max-width: 800px; margin: auto; margin-top: 60px; margin-bottom: 60px;"" class=""email-letter"">
                                        <div style=""margin-bottom: 30px; font-weight: 800; font-size: 28px; color: #31445a;"" class=""salutation"">Estimado {recipient.Name},</div>
                                        <div style=""font-style: normal; font-weight: 500; font-size: 18px; align-items: center; color: #6D7278;"" class=""letter-body"">
                                            <p><b>Sender First name: {senderData.SenderFirstName}</b></p>
                                            <p><b>Sender First lastname: {senderData.SenderLastName}</b></p>
                                            <p><b>Sender email address: {senderData.SenderEmailAddress}</b></p>
                                            <p><b>Sender phone number: {senderData.SenderPhoneNumber}</b></p>
                                            <p><b>Sender message:</b></p>
                                            <p>{senderData.SenderMessage}</p>
                                        </div>
                                    </div>
                                    <div style=""width: 100%; height: 1px; background-color: gainsboro; margin-bottom: 30px;"" class=""divider""></div>
                                </div>
                            </div>
                        </body>
                    </html>
                ";

                _mailService.SendEmail(
                    fromAddress: personalEmail,
                    fromDisplay: "Bernardo Mondragon Brozon",
                    to: recipient.Email,
                    cc: "",
                    subject: subject,
                    plainTextBody: plainTextContent,
                    htmlBody: htmlContent
                );

                result = "Email sent successfully to " + recipient.Email;
                _logger.LogInformation(result);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception caught while sending email");
            result = "Error sending email: " + e.Message;
            throw;
        }
        return Task.FromResult(result);
    }
}
