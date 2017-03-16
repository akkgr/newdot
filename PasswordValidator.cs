
using System.Threading.Tasks;
using IdentityServer4.Validation;

namespace newdot
{
    public class PasswordValidator : IResourceOwnerPasswordValidator
    {
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                if (context.UserName.Equals("alice") && context.Password.Equals("password"))
                {
                    context.Result = new GrantValidationResult("1", "password", new List<Claim>
                    {
                    new Claim(Common.CLAIM_ID, "1"),
                    new Claim(Common.CLAIM_EMAIL, "alice@cherry.loc"),
                    new Claim(Common.CLAIM_FIRSTNAME, "Alice"),
                    new Claim(Common.CLAIM_LASTNAME, "Liddell"),
                    new Claim(Common.CLAIM_NAME, "Alice"),

                    new Claim("cherry_sample", "hello")
                    });
                }
            });
        }
    }
}