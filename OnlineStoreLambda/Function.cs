using System.Net;
using System.Net.Mail;
using System.Text.Json;
using Amazon.Lambda.Core;
using OnlineStoreLambda.DTOs;
using SendGrid;
using SendGrid.Helpers.Mail;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace OnlineStoreLambda;

public class Function
{
    
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<string> FunctionHandler(SenderData senderData, ILambdaContext context)
    {
        //SenderData senderData = JsonSerializer.Deserialize<SenderData>(input); // In case the first parameter is string input
        var mailingList = new MailingList();
        var personalEmail = Environment.GetEnvironmentVariable("PERSONAL_EMAIL");
        mailingList.List = new List<Recipient>()
        {
            new Recipient() { Email = personalEmail, Name = "Bernardo Mondragon Brozon"}
        };
        string res = await SendMessageFromVisitor(mailingList.List, senderData);
        return res;
    }

    private async Task<string> SendMessageFromVisitor(List<Recipient> recipients, SenderData senderData)
    {
        string result = "";
        try
        {
            foreach (var recipient in recipients)
            {
                var personalEmail = Environment.GetEnvironmentVariable("PERSONAL_EMAIL");
                Console.WriteLine($"\nSending email from: {personalEmail}");
                Console.WriteLine($"\nSending email to: {recipient.Name} :: {recipient.Email}");
                var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(personalEmail, "Bernardo Mondragon Brozon");
                var subject = "Message from visitor";
                var to = new EmailAddress(recipient.Email, recipient.Name);
                var plainTextContent = "";
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
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                Response? response = await client.SendEmailAsync(msg).ConfigureAwait(false);
                Console.WriteLine($"SendGrid reponse status: " + response.StatusCode);
                string res = response.IsSuccessStatusCode 
                    ? "Email sent successfully to " + recipient.Email 
                    : "Failed to send email to " + recipient.Email;
                Console.WriteLine(res);
                result = res;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("{0} Exception caught.", e);
            result = "Error sending email: " + e.Message;
            throw;
        }
        return result;
    }
}
