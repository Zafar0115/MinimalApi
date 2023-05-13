using MinimalJwt.Models;

namespace MinimalJwt.Repositories
{
    public class UserRepository
    {
        public static List<User> Users = new List<User>()
        {
            new User()
            {
                UserName="jason_admin",
                EmailAddress="json@email.com",
                Password="my_Password",
                GivenName="Jason",
                Surname="Brian",
                Role="Administrator"},

            new User()
            {
                UserName="elyse_seller",
                EmailAddress="elyse@email.com",
                Password="my_Passw0rd",
                GivenName="Elyse",
                Surname="Lambert",
                Role="Standart"
            }
        };
    }
}
