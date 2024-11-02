using BookStore.Models.Account;
using BookStore.Repository.Repository;
using System.Collections.Generic;
using System.Linq;

namespace BookStore.Services
{
    public class AccountService
    {
        GenericRepository<User> userRepo = new GenericRepository<User>();
        GenericRepository<Role> userRoleRepo = new GenericRepository<Role>();

        public bool AuthenticateUser(string email,string password)
        {
            return userRepo.Gets(x => x.Email == email && x.Password == password).Any();
        }
        public bool AuthenticateUserEmail(string email)
        {
            return userRepo.Gets(x => x.Email == email).Any();
        }
        public User GetUser(string email)
        {
            return userRepo.Gets(x => x.Email == email).FirstOrDefault();
        }
        public User AddUser(User user)
        {
            userRepo.Insert(user);
            userRepo.Save();
            return user;
        }
        public List<Role> GetUserRoleList()
        {
            List<Role> roles =
                userRoleRepo.GetAll().OrderBy(n => n.RoleName).ToList();
            return roles;
        }
    }
}
