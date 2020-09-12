using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HitPoints.Models;

    public class Context : DbContext
    {
        public Context (DbContextOptions<Context> options)
            : base(options)
        {
        }

        public DbSet<HitPoints.Models.Damage> Damage { get; set; }

        public DbSet<HitPoints.Models.PlayerCharacter> PlayerCharacter { get; set; }
    }