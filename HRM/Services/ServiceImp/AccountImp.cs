using HRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRM.Services.ServiceImp
{
    public class AccountImp : IAccount, IDisposable
    {
        private readonly ApplicationContext application = new ApplicationContext();
        public void Dispose()
        {
            application.Dispose();
        }

        public IEnumerable<string> GetUserFunction(string userId)
        {

            var funtion = application.UserFunction.Where(f => f.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase)).Select(f => f.FunctionID);
            return funtion;
        }

        public User ValidateUser(string userName, string password)
        {
            var user = application.users.Where(u => u.UserID.Equals(userName, StringComparison.OrdinalIgnoreCase)).SingleOrDefault() ?? null;
            var hashPassword = Helper.validPassword(password, user == null ? null : user.Password);
            return application.users.Where(u => u.UserID.Equals(userName, StringComparison.OrdinalIgnoreCase) && hashPassword).FirstOrDefault();
        }
        public async Task<bool> AddRefreshToken(RefreshToken token)
        {
            var existingToken = application.RefreshTokens.FirstOrDefault(r => r.UserName == token.UserName
                            && r.ClientID == token.ClientID);
            if (existingToken != null)
            {
                var result = await RemoveRefreshToken(existingToken);
            }
            application.RefreshTokens.Add(token);
            return await application.SaveChangesAsync() > 0;
        }
        //Remove the Refesh Token by id
        public async Task<bool> RemoveRefreshTokenByID(string refreshTokenId)
        {
            var refreshToken = await application.RefreshTokens.FindAsync(refreshTokenId);
            if (refreshToken != null)
            {
                application.RefreshTokens.Remove(refreshToken);
                return await application.SaveChangesAsync() > 0;
            }
            return false;
        }
        //Remove the Refresh Token
        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            application.RefreshTokens.Remove(refreshToken);
            return await application.SaveChangesAsync() > 0;
        }
        //Find the Refresh Token by token ID
        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await application.RefreshTokens.FindAsync(refreshTokenId);
            return refreshToken;
        }
        //Get All Refresh Tokens
        public List<RefreshToken> GetAllRefreshTokens()
        {
            return application.RefreshTokens.ToList();
        }

    }
}