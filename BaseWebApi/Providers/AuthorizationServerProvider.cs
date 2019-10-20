using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Security.Claims;
using BaseWebApi.Models;
using BaseWebApi.Providers;

namespace BaseWebApi.Providers
{
    public class AuthorizationServerProvider: OAuthAuthorizationServerProvider
    {        
        
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        //public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated(); //
            //return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            SuitrohDBEntities dbContext = new SuitrohDBEntities();

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            var password = EncryptDecrypt.Encrypt(context.Password);
            User user = dbContext.Users.Where(u => u.username == context.UserName && u.password == password).FirstOrDefault();
            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }
            else
            {                
                //if (context.UserName == "admin" && context.Password == "admin")
                //{
                    identity.AddClaim(new Claim(ClaimTypes.Role, user.role.ToString()));
                    identity.AddClaim(new Claim("firstName", user.firstName.ToString()));
                    identity.AddClaim(new Claim("lastName", user.lastName.ToString()));
                    identity.AddClaim(new Claim("username", user.username.ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.lastName.ToString() + " " + user.firstName.ToString()));
                    identity.AddClaim(new Claim("UserId", user.id.ToString()));
                    identity.AddClaim(new Claim("EmployeeId", user.UserId.ToString()));
                    context.Validated(identity);
                //}
            }
            
            /*if(context.UserName == "admin" && context.Password == "admin")
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));
                identity.AddClaim(new Claim("username", "admin"));
                identity.AddClaim(new Claim(ClaimTypes.Name, "Jolasho Joseph"));
                context.Validated(identity);
            }
            else if (context.UserName == "user" && context.Password == "user")
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
                identity.AddClaim(new Claim("username", "user"));
                identity.AddClaim(new Claim(ClaimTypes.Name, "Bingo Beans"));
                context.Validated(identity);
            }
            else
            {
                context.SetError("invalid_grant", "Username and Password Provided is incorrect");
                return;
            }*/
        }
    }
}