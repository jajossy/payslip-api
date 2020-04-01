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
            string status = context.Parameters["status"];

            //You can pass if you need the parameter to your GrantResourceOwnerCredentials
            context.OwinContext.Set<string>("as:status", status);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            NominalDataEntities dbContext = new NominalDataEntities();
            var status = context.OwinContext.Get<string>("as:status");

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);

            if(status == "admin")
            {
                //var password = EncryptDecrypt.Encrypt(context.Password);
                User user = dbContext.Users.Where(u => u.Username.ToLower() == context.UserName && u.Password.ToLower() == context.Password).FirstOrDefault();
                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
                else
                {
                    if(user.Role != null) identity.AddClaim(new Claim(ClaimTypes.Role, user.Role.ToString()));
                    identity.AddClaim(new Claim("ippis", user.UserId.ToString()));
                    identity.AddClaim(new Claim("fullname", user.Username.ToString()));
                    identity.AddClaim(new Claim("surname", user.Username.ToString()));
                    //identity.AddClaim(new Claim("email", staff.Email.ToString()));
                    //identity.AddClaim(new Claim("email", staff.Email.ToString()));


                    context.Validated(identity);

                }

                
            }
            else
            {
                //var password = EncryptDecrypt.Encrypt(context.Password);
                //StaffDataLatest staff = dbContext.StaffDataLatests.Where(u => u.Surname == context.UserName && u.Ippis.ToLower() == context.Password && u.Active == true).FirstOrDefault();
                // UCH uses only ippis or Pin no, others may include surname
                StaffDataLatest staff = dbContext.StaffDataLatests.Where(u => u.Ippis.ToLower() == context.Password && u.Active == true).FirstOrDefault();
                if (staff == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
                else
                {
                    //if(staff.role != null) identity.AddClaim(new Claim(ClaimTypes.Role, staff.role.ToString()));
                    identity.AddClaim(new Claim("ippis", staff.Ippis.ToString()));
                    identity.AddClaim(new Claim("fullname", staff.Fullname.ToString()));
                    identity.AddClaim(new Claim("email", staff.Email.ToString()));
                    if(staff.Surname != null) identity.AddClaim(new Claim("surname", staff.Surname.ToString()));
                    if (staff.Firstname != null) identity.AddClaim(new Claim("firstname", staff.Firstname.ToString()));                    

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
}