namespace Incident.Infrastructure.Persistence.Mongo
{
    public class MongoSettings
    {
        public required string ConnectionString { get; set; }

        public required string Database { get; set; }

        public required string Collection { get; set; }
    }
}
