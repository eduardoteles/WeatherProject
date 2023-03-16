using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeatherProject.Models;

namespace WeatherProject.Data
{
    public class WeatherProjectContext : DbContext
    {
        public WeatherProjectContext (DbContextOptions<WeatherProjectContext> options)
            : base(options)
        {
        }

        public DbSet<WeatherProject.Models.Forecast> Forecast { get; set; } = default!;
    }
}
