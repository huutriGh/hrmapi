using HRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HRM.Services
{
    public interface IAccount
    {
        User ValidateUser(string user, string password);
        IEnumerable<string> GetUserFunction(string userId);
        Task<bool> AddRefreshToken(RefreshToken token);
        Task<bool> RemoveRefreshTokenByID(string refreshTokenId);
        Task<bool> RemoveRefreshToken(RefreshToken refreshToken);
        Task<RefreshToken> FindRefreshToken(string refreshTokenId);
        List<RefreshToken> GetAllRefreshTokens();
    }
}