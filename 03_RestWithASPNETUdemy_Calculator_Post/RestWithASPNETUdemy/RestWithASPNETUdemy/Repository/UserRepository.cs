using RestWithASPNETUdemy.Data.Converter.VO;
using RestWithASPNETUdemy.Model;
using RestWithASPNETUdemy.Model.Context;
using System.Security.Cryptography;
using System.Text;

namespace RestWithASPNETUdemy.Repository
{
    public class UserRepository : IUserRepository
    {
        public readonly MySQLContext _context;

        public UserRepository(MySQLContext context)
        {
            _context = context;
        }

        public User ValidadeCredentials(UserVO user)
        {
            //encriptar senha
            var pass = ComputeHash(user.Password, new SHA256CryptoServiceProvider());
            return _context.Users.FirstOrDefault(u => (u.UserName == user.UserName) && (u.Password == pass));
        }

        public User RefreshUserInfo(User user)
        {
            if (_context.Users.Any(p => p.Id.Equals(user.Id))) return null;

            var result = _context.Users.SingleOrDefault(p => p.Id.Equals(user.Id));
            if (result != null)
            {
                try
                {
                    _context.Entry(result).CurrentValues.SetValues(user);
                    _context.SaveChanges();
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                return result;
            }
        }

        private string ComputeHash(string input, SHA256CryptoServiceProvider sHA256CryptoServiceProvider)
        {
            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            Byte[] hashedBytes = sHA256CryptoServiceProvider.ComputeHash(inputBytes);
            return BitConverter.ToString(hashedBytes);
        }
    }
}
