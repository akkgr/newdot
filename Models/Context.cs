using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using Cinnamon.Identity;

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

            Users = database.GetCollection<User>("users");
        }

        public IMongoCollection<User> Users { get; set; }

        public void EnsureIndexes()
        {
            var options = new CreateIndexOptions();
            options.Unique = true;           
            Users.Indexes.CreateOneAsync(Builders<User>.IndexKeys.Ascending(d => d.Username), options);
        }

        public void EnsureAdminAccount()
        {
            var admin = Users.Find(t=>t.Username == "admin").FirstOrDefault();
            if (admin == null)
            {
                admin = new User();
                admin.Username = "admin";
                admin.PasswordHash = PasswordHasher.HashPassword("admin");
                admin.IsActive = true;
                Users.InsertOne(admin);
            }
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
        }
    }
}