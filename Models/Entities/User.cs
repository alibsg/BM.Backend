using System;

namespace BM.BackEnd.Models.Entities{
    public class Role
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }
    public class User{
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string Education { get; set; }
        public int MaritalStatus { get; set; }
        public DateTime BDate { get; set; }
        public int Sex { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}