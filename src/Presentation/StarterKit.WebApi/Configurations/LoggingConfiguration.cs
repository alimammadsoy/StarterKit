using Serilog;
using Serilog.Sinks.PostgreSQL;
using Serilog.Core;

namespace StarterKit.WebApi.Configurations
{
    public static class LoggingConfiguration
    {
        public static void ConfigureLogger(this WebApplicationBuilder builder)
        {
            var columnWriters = new Dictionary<string, ColumnWriterBase>
            {
                { "Message", new RenderedMessageColumnWriter() },
                //{ "MessageTemplate", new MessageTemplateColumnWriter() },
                { "Level", new LevelColumnWriter(true, NpgsqlTypes.NpgsqlDbType.Varchar) },
                { "TimeStamp", new TimestampColumnWriter() },
                //{ "Exception", new ExceptionColumnWriter() },
                //{ "Properties", new PropertiesColumnWriter(NpgsqlTypes.NpgsqlDbType.Jsonb) },
                { "IPAddress", new SinglePropertyColumnWriter("IPAddress", PropertyWriteMethod.ToString , NpgsqlTypes.NpgsqlDbType.Varchar) }
            };

            Logger logger = new LoggerConfiguration()
                .WriteTo.PostgreSQL(
                    connectionString: builder.Configuration.GetConnectionString("LogDb"),
                    tableName: "Logs",
                    columnWriters,
                    needAutoCreateTable: true
                )
                .Enrich.FromLogContext()
                .MinimumLevel.Information()
                .Filter.ByIncludingOnly(log => log.Properties.ContainsKey("SourceContext")
                                   && log.Properties["SourceContext"].ToString().Contains("Controller"))
                .WriteTo.File("logs/log.txt")
                .CreateLogger();

            builder.Host.UseSerilog(logger);
        }
    }
}