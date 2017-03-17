using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Threading.Tasks;
using newdot.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;

namespace Cinnamon.Identity
{
    public class ProfileService : IProfileService
    {
        private Context db { get; }

        public ProfileService(Context ctx)
        {
            db = ctx;
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            return Task.Factory.StartNew(async () =>
            {
                var filter = new BsonDocument();
                var user = await db.Users.Find(t => t.Username == context.Subject.GetSubjectId()).FirstOrDefaultAsync();
                if (user != null)
                {
                    var claims = new HashSet<Claim>(new ClaimComparer());
                    claims.Add(new Claim(ClaimTypes.Role,"user"));
                    context.IssuedClaims.AddRange(claims);
                }
            });
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.Factory.StartNew(async () =>
            {
                context.IsActive = false;
                var filter = new BsonDocument();
                var user = await db.Users.Find(t => t.Username == context.Subject.GetSubjectId()).FirstOrDefaultAsync();
                if (user != null)
                {
                    context.IsActive = user.IsActive;
                }
            });
        }
    }
}