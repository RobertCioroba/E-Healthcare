﻿using E_Healthcare.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Healthcare.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Account> Accounts { get; set; } = default!;
    }
}
