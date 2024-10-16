﻿using gps_jamming_classifier_be.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace gps_jamming_classifier_be.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SignalData> SignalDatas { get; set; }
        public DbSet<Spectrogram> Spectrograms { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
