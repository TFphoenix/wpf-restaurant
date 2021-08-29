using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Authentication;
using Restaurant.Models;

namespace Restaurant.Models.Services
{
    class UserServices
    {
        private readonly RestaurantEDM _context = new RestaurantEDM();

        public user LoginMethod(string email, string password)
        {
            var result = _context.spLogin(email, password).ToList();

            if (result.Count != 1) throw new ArgumentException("Incorrect email or password");

            foreach (var spLoginResult in result)
            {
                if (!spLoginResult.email.Equals(email) || !spLoginResult.password.Equals(password))
                    throw new ArgumentException("Incorrect email or password");
                return new user
                {
                    address = spLoginResult.address,
                    email = spLoginResult.email,
                    first_name = spLoginResult.first_name,
                    id = spLoginResult.id,
                    last_name = spLoginResult.last_name,
                    phone = spLoginResult.phone,
                    role = spLoginResult.role
                };
            }

            throw new AuthenticationException("Unknown error, try again");
        }

        public void RegisterMethod(user newUser)
        {
            _context.spRegister(newUser.first_name,
                    newUser.last_name,
                    newUser.email,
                    newUser.password,
                    newUser.phone,
                    newUser.address);
            _context.SaveChanges();
        }
    }
}
