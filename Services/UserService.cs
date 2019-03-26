using System;
using System.Linq;
using System.Collections.Generic;
using BM.BackEnd.Models.Entities;
using BM.BackEnd.Models;
using BM.BackEnd.Helpers;

namespace BM.BackEnd.Services{
    class UserService : IUserService
    {
        private UserContext _context;
        public UserService(UserContext context)
        {
            _context = context;
        }

        public User Authenticate(User user, string password){
            if(!VerifyPasswordHash(password,user.PasswordHash,user.PasswordSalt)){
                return null;
            }
            return user;
        }
        public User AuthenticateByUserNameAndPassword(string username, string password)
        {
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)){
                return null;
            }
            var user = _context.Users.SingleOrDefault(x => x.UserName == username);
            if(user == null){
                return null;
            }

            return Authenticate(user,password);
        }

        public User AuthenticateByPhoneNumberAndPassword(string phonenumber, string password)
        {
            if(string.IsNullOrEmpty(phonenumber) || string.IsNullOrEmpty(password)){
                return null;
            }
            var user = _context.Users.SingleOrDefault(x => x.MobileNumber == phonenumber);
            if(user == null){
                return null;
            }

            return Authenticate(user,password);
        }

        public User Create(User user, string password)
        {
            if(string.IsNullOrWhiteSpace(password)) throw new AppException("Password is Required");
            if(_context.Users.Any(x => x.UserName == user.UserName)) throw new AppException("Username is Already Taken");
            if(_context.Users.Any(x => x.MobileNumber == user.MobileNumber)) throw new AppException("Mobile Number is Already Taken");

            byte[] passwordhash,passwordsalt;
            CreatePasswordHash(password,out passwordhash,out passwordsalt);
            user.PasswordHash = passwordhash;
            user.PasswordSalt = passwordsalt;
            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if(user == null){
                throw new AppException("User Not Found");
            }

            _context.Users.Remove(user);
            _context.SaveChanges();            
        }

        public IEnumerable<User> getAll() => _context.Users;

        public User getById(int id) => _context.Users.Find(id);        

        public void Update(User user, string password = null)
        {
            var founduser = _context.Users.Find(user.Id);
            if(founduser == null) throw new AppException("User Not Found");
            if(founduser.UserName != user.UserName){
                if(_context.Users.Any(x => x.UserName == user.UserName))
                    throw new AppException("User Name is Already Taken");
            }
            if(founduser.MobileNumber != user.MobileNumber){
                if(_context.Users.Any(x => x.MobileNumber == user.MobileNumber))
                    throw new AppException("Mobile Number is Already Taken");
            }

            founduser.FirstName  = user.FirstName;
            founduser.LastName = user.LastName;
            founduser.MobileNumber  = user.MobileNumber ;
            founduser.Email = user.Email;
            founduser.Education  = user.Education; 
            founduser.MaritalStatus = user.MaritalStatus ;
            founduser.UserName  = user.UserName;

            if(!string.IsNullOrEmpty(password)){
                byte[] passhash,passsalt;
                CreatePasswordHash(password, out passhash, out passsalt);
                founduser.PasswordHash = passhash;
                founduser.PasswordSalt = passsalt;
            } 

            _context.Users.Update(founduser);
            _context.SaveChanges();
        }

        public static void CreatePasswordHash(string password,out byte[] passwordhash,out byte[] passwordsalt){
            if(password == null) throw new ArgumentNullException("Password");
            if(string.IsNullOrWhiteSpace(password)) throw new ArgumentException("value can not be whitespace.","Password");
            using(var hmac = new System.Security.Cryptography.HMACSHA512()){
                passwordsalt = hmac.Key;
                passwordhash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public static bool VerifyPasswordHash(string password,byte[] passwordHash, byte[] passwordSalt){
            if(password == null) throw new ArgumentNullException("Password");
            if(string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value can not be whitespace","Password");
            if (passwordHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (passwordSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)){
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i=0; i < computeHash.Length; i++){
                    if(computeHash[i] != passwordHash[i]){
                        return false;
                    }
                }
            }
            return true;
        }
    }
}