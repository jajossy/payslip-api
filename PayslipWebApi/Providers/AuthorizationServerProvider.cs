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
            NominalDataEntities dbContext = new NominalDataEntities();

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            //var password = EncryptDecrypt.Encrypt(context.Password);
            StaffDataLatest staff = dbContext.StaffDataLatests.Where(u => u.Ippis.ToLower() == context.Password).FirstOrDefault();
            if (staff == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }
            else
            {         
                if(staff.role != null) identity.AddClaim(new Claim(ClaimTypes.Role, staff.role.ToString()));
                identity.AddClaim(new Claim("ippis", staff.Ippis.ToString()));
                identity.AddClaim(new Claim("fullname", staff.Fullname.ToString()));
                identity.AddClaim(new Claim("email", staff.Email.ToString()));

                context.Validated(identity);
                
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