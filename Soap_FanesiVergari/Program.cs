using Soap_FanesiVergari.WSSoap;
using SoapCore;
using System.Text;


namespace Soap_FanesiVergari
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSoapCore();
            builder.Services.AddScoped<IServizioAutoveloxItalia, ServizioAutoveloxItalia>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseRouting();

            // Redirect root to the WSDL endpoint
            app.MapGet("/", () => Results.Redirect("/ServizioAutoveloxItalia.wsdl"));

            
            var encoderOptions = new SoapEncoderOptions
            {
                MessageVersion = System.ServiceModel.Channels.MessageVersion.Soap12WSAddressing10, // SOAP 1.2 versione codificatore corretto per envelope
                WriteEncoding = Encoding.UTF8,
                ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max
            };
            app.UseEndpoints(endpoints =>
            {
                endpoints.UseSoapEndpoint<IServizioAutoveloxItalia>("/ServizioAutoveloxItalia.wsdl", encoderOptions, SoapSerializer.XmlSerializer);
            });

            app.Run();
        }
    }
}