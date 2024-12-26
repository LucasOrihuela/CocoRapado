using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

public class EmailSender
{
    private readonly string _apiKey;
    private const string ResendEndpoint = "https://api.resend.com/emails";

    // Constructor para inicializar la API Key desde appsettings.json
    public EmailSender(IConfiguration configuration)
    {
        _apiKey = configuration["Resend:ApiKey"];
        if (string.IsNullOrEmpty(_apiKey))
        {
            throw new Exception("API Key de Resend no configurada en appsettings.json");
        }
    }

    // Método para enviar correos electrónicos
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var emailData = new
            {
                from = "CocoRapado <lucasorihuela18@gmail.com>",
                to,
                subject,
                html = body // También puedes usar "text" para texto plano
            };

            var jsonData = JsonConvert.SerializeObject(emailData);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(ResendEndpoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error enviando email: {errorContent}");
            }

            Console.WriteLine("Correo enviado exitosamente.");
        }
    }
}
