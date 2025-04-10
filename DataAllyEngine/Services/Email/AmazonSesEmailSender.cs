using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using DataAllyEngine.Configuration;

namespace DataAllyEngine.Services.Email;

public class AmazonSesEmailSender: IEmailSender
{
	private readonly ILogger<AmazonSesEmailSender> logger;
	
	public AmazonSesEmailSender(ILogger<AmazonSesEmailSender> logger)
	{        
		this.logger = logger;
	}

	public async Task SendMail(string sender, string to, string subject, string content, bool isHtml = false)
	{
		await SendMailToMultipleRecipients(sender, new List<string> { to }, subject, content, isHtml); 
	}

	public async Task SendMailToMultipleRecipients(string sender, List<string> to, string subject, string content, bool isHtml = false)
	{
		using (var client = new AmazonSimpleEmailServiceClient(RegionEndpoint.USEast1))
		{
			var sendRequest = new SendEmailRequest
			{
				Source = sender,
				Destination = new Destination { ToAddresses = to },
				Message = new Message
				{
					Subject = new Content(subject),
					Body = new Body { Text = new Content(content) }
				}
			};

			try
			{
				await client.SendEmailAsync(sendRequest);
			}
			catch (Exception ex)
			{
				logger.LogError($"Sending mail had an error: {ex}");
			}
		}
	}
}