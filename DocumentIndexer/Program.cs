using DocumentIndexer.Services;
using Nest;
using System.Configuration;

namespace DocumentIndexer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddSingleton(_ =>
            {
                var esHost = builder.Configuration.GetSection("ElasticSearch")["Host"];

                if (esHost == null)              
                    throw new ConfigurationErrorsException("В конфигурационном файле не указан адрес кластера ES.");

                var connectionSettings = new ConnectionSettings(new Uri(esHost));
                connectionSettings.DisableDirectStreaming();

                return new ElasticClient(connectionSettings);
            });
            builder.Services.AddTransient<DocumentService>();
            builder.Services.AddTransient<ElasticService>();

            var app = builder.Build();

            app.MapControllers();

            app.Run();
        }
    }
}
