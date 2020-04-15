using HRM.Services;
using HRM.Services.ServiceImp;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Text;
using System.DirectoryServices.AccountManagement;

namespace HRM.Models
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private IAccount account;
        public AuthorizationServerProvider()
        {
            account = new AccountImp();
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();

        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            // context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            //try
            //{
            //    var adContext = new PrincipalContext(ContextType.Domain, "phuhunglife.com", "1234567");
            //    adContext.ValidateCredentials(context.UserName, context.Password);

            //}
            //catch (Exception ex)
            //{
            //    var a = ex;

            //}


            var user = account.ValidateUser(context.UserName, context.Password);
            if (user == null)
            {
                context.SetError("invalid_grant", "Provided username and password is incorrect");
                return;
            }

            var role = Newtonsoft.Json.JsonConvert.SerializeObject(account.GetUserFunction(user.UserID).ToList());
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserID));
            identity.AddClaim(new Claim(ClaimTypes.Actor, user.BusinessEntityID));
            identity.AddClaim(new Claim(ClaimTypes.Role, role));

            var authProp = new AuthenticationProperties(new Dictionary<string, string>()
{
            { "userId", user.UserID },
            { "userName", user.UserName },

            { "userFuntion",role },

            });
            var ticket = new AuthenticationTicket(identity, authProp);
            context.Validated(ticket);


        } 
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
    }
}
