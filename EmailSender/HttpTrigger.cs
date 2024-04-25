using System.IO;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;

public static class SendEmailFunction
{
	[FunctionName("SendEmail")]
	public static async Task<IActionResult> Run(
		[HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
		ILogger log)
	{
		log.LogInformation("C# HTTP trigger function processed a request.");

		string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
		log.LogInformation($"Received data: {requestBody}");

		dynamic data = JsonConvert.DeserializeObject(requestBody);

		string toEmail = data?.toEmail;
		string subject = data?.subject;
		string content = data?.content;

		if (string.IsNullOrEmpty(toEmail) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(content))
		{
			log.LogError("Invalid request data.");
			return new BadRequestObjectResult("Please provide toEmail, subject, and content in the request body.");
		}

		// Gmail SMTP settings
		var smtpServer = "smtp.gmail.com";
		var smtpPort = 587; // Gmail SMTP port
		var smtpUsername = "fbms.1223@gmail.com"; // Replace with your Gmail email address
		var smtpPassword = "gtic uuke ggei yhnm"; // Replace with your Gmail App Password

		try
		{
			using (var client = new SmtpClient(smtpServer, smtpPort))
			{
				client.UseDefaultCredentials = false;
				client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
				client.EnableSsl = true;
				var mailMessage = new MailMessage
				{
					From = new MailAddress(smtpUsername),
					Subject = "Salam",
					Body = "<h1><b style='color:red;'>Salam2</b></h1>",
					IsBodyHtml = true
				};

				mailMessage.To.Add(toEmail);

				client.Send(mailMessage);
				log.LogInformation($"Email sent to {toEmail}");
				return new OkObjectResult(new { status = "success" });
			}
		}
		catch (Exception ex)
		{
			log.LogError($"Error sending email: {ex.Message}");
			return new BadRequestObjectResult($"Error sending email: {ex.Message}");
		}
	}
}
