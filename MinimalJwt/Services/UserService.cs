using MinimalJwt.Models;
using MinimalJwt.Repositories;

namespace MinimalJwt.Services
{
    public class UserService : IUserService
    {
        public User? Get(UserLogin userLogin)
        {
            User? user = UserRepository.Users.FirstOrDefault(o =>
            o.UserName.Equals(userLogin.UserName, StringComparison.OrdinalIgnoreCase) &&
            o.Password.Equals(userLogin.Password, StringComparison.OrdinalIgnoreCase));

            if (user is not null)
                return user;

            return null;
        }
    }
}
