using BM.BackEnd.Models.Entities;
using System.Collections.Generic;

namespace BM.BackEnd.Services{
    public interface IUserService {
        User Authenticate(User username, string password);        
        IEnumerable<User> getAll();
        User getById(int id);
        User Create(User user,string password);
        void Update(User user,string password = null);
        void Delete(int id);
    }
}