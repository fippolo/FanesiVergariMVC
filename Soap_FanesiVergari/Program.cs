using Soap_FanesiVergari.WSSoap;
using SoapCore;

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

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            

            app.UseRouting();

            app.UseAuthorization();
            

            // non utiliaziamo endpoint per codice Warning ASP0014
            //  app.UseEndpoints(endpoints =>
            //  {
            //      endpoints.UseSoapEndpoint<IServizioAutoveloxItalia>("/ServizioAutoveloxItalia.wsdl", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
            //  });

            app.MapGet("/", () => "ServizioAutoveloxItalia.wsdl");


            app.MapControllers();

            app.Run();
        }
    }
}