using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using BM.BackEnd.Models.Entities;

namespace BM.BackEnd.Models {
    public class UserContext : DbContext{
        public UserContext(DbContextOptions<UserContext> options)
        :base(options){           
        }
        public DbSet<User> Users { get; set; }

    }
}