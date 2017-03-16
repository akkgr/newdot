using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;

namespace newdot.Models
{
    public class Context
    {
        public Context(string connectionString)
        {
            var url = new MongoUrl(connectionString);
			var client = new MongoClient(url);
			if (url.DatabaseName == null)
			{
				throw new ArgumentException("Your connection string must contain a database name", connectionString);
			}
			var database = client.GetDatabase(url.DatabaseName);

            Users = database.GetCollection<User>("AccommodationCenters");
        }

        public IMongoCollection<User> Users { get; set; }

        private void EnsureIndexes()
        {
            var options = new CreateIndexOptions();
            options.Unique = true;           
        }

        public static void Init()
        {
            BsonClassMap.RegisterClassMap<BaseModel>(cm =>
            {
                cm.AutoMap();
                cm.SetIdMember(cm.GetMemberMap(c => c.Id)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId))
                    .SetIdGenerator(StringObjectIdGenerator.Instance));
            });

            BsonClassMap.RegisterClassMap<User>(cm =>
            {
                cm.AutoMap();
            });

            BsonClassMap.RegisterClassMap<AccommodationPeriod>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(c => c.StartDate).SetSerializer(new DateTimeSerializer(DateTimeKind.Local));
                cm.MapMember(c => c.EndDate).SetSerializer(new DateTimeSerializer(DateTimeKind.Local));
            });

            BsonClassMap.RegisterClassMap<AccommodationType>(cm =>
            {
                cm.AutoMap();
            });

            BsonClassMap.RegisterClassMap<Person>(cm =>
            {
                cm.AutoMap();
                cm.UnmapMember(c => c.Spouse);
            });

            BsonClassMap.RegisterClassMap<ResortApplication>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(c => c.DateSubmitted).SetSerializer(new DateTimeSerializer(DateTimeKind.Local));
                cm.UnmapMember(c => c.Person);
                cm.UnmapMember(c => c.AccommodationCenter);
                cm.UnmapMember(c => c.AccommodationType);
                cm.UnmapMember(c => c.FirstAccommodationPeriod);
                cm.UnmapMember(c => c.SecondAccommodationPeriod);
                cm.UnmapMember(c => c.AccommodationTypes);
                cm.UnmapMember(c => c.AccommodationCenters);
                cm.UnmapMember(c => c.AccommodationPeriods);
            });

            BsonClassMap.RegisterClassMap<SpayFak>(cm =>
            {
                cm.AutoMap();
            });

            BsonClassMap.RegisterClassMap<CpayFak>(cm =>
            {
                cm.AutoMap();
            });
        }
    }
}