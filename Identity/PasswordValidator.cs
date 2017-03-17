
using System.Threading.Tasks;
using Cinnamon.Identity;
using IdentityModel;
using IdentityServer4.Validation;
using MongoDB.Bson;
using MongoDB.Driver;
using newdot.Models;

namespace Cinnamon.Identity
{
    public class PasswordValidator : IResourceOwnerPasswordValidator
    {
        private Context db { get; }

        public PasswordValidator(Context ctx)
        {
            db = ctx;
        }

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            return Task.Factory.StartNew(async () =>
            {
                var filter = new BsonDocument();
                var user = await db.Users.Find(t => t.Username == context.UserName).FirstOrDefaultAsync();
                if (user != null)
                {
                    if(PasswordHasher.VerifyHashedPassword(user.PasswordHash, context.Password))
                    {
                        context.Result = new GrantValidationResult(user.Username, OidcConstants.AuthenticationMethods.Password);
                    }
                }
            });
        }
    }
}