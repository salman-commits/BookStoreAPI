using BookStore.Repository.Context;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStore.Repository.Provider
{
    public class OAuthProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            await Task.Run(() => context.Validated());
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                using (var db = new ApplicationDbContext())
                {
                    if (db != null)
                    {
                        var user = db.User.Where(x => x.Email == context.UserName).FirstOrDefault();
                        if (user == null)
                        {
                            context.SetError("Login Error", "provided email address not found, sign up to create an account");
                            return;
                        }

                        var validUser = (from u in db.User
                                         join r in db.Role on u.RoleId equals r.RoleId
                                         where u.Email == context.UserName && u.Password == context.Password
                                         select new { r.RoleName, u.Email }).FirstOrDefault();

                        if (validUser != null)
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, validUser.RoleName));
                            identity.AddClaim(new Claim(ClaimTypes.Name, validUser.Email));
                            identity.AddClaim(new Claim("LoggedOn", DateTime.Now.ToString()));
                            await Task.Run(() => context.Validated(identity));
                        }
                        else
                        {
                            context.SetError("Login Error", "Provided username or password are incorrect");
                        }
                    }
                    else
                    {
                        context.SetError("API Error", "API services are down, please contact yur administrator.");
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
    }
}