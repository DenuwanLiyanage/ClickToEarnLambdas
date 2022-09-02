using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace HelloWorld;

public interface IEmailFunctions
{
}

public class EmailFunctions : IEmailFunctions
{
    private readonly AmazonSimpleEmailServiceClient _emailClient;

    public EmailFunctions(AmazonSimpleEmailServiceClient emailClient)
    {
        _emailClient = emailClient;
    }


    public async Task<string> SendAnEmailAsync(string emailAddress, string subject, string body)
    {
        var sendRequest = new SendEmailRequest
        {
            Source = "denuwan.metaroon@gmail.com",
            Destination = new Destination
            {
                ToAddresses =
                    new List<string> { emailAddress }
            },
            Message = new Message
            {
                Subject = new Content(subject),
                Body = new Body
                {
                    Text = new Content
                    {
                        Charset = "UTF-8",
                        Data = body
                    }
                }
            },
        };
        try
        {
            Console.WriteLine("Sending email using Amazon SES...");
            var response = await _emailClient.SendEmailAsync(sendRequest);

            Console.WriteLine("The email was sent successfully." + response.HttpStatusCode);
            return "Success";
        }
        catch (Exception ex)
        {
            Console.WriteLine("The email was not sent.");
            Console.WriteLine("Error message: " + ex.Message);
            return "Failed";

        }
    }
}