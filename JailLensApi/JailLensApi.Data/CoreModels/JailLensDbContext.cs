using JailLensApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace JailLensApi.Data
{
    public class JailLensDbContext : DbContext
    {
        public JailLensDbContext(DbContextOptions<JailLensDbContext> options) : base(options)
        {

        }

        public DbSet<Inmate> inmate { get; set; }
        public DbSet<FaceRecognitionEvents> facerecognitionevents { get; set; }
        public DbSet<Programs> programs { get; set; }
        public DbSet<Location> locations { get; set; }
        public DbSet<ProgramLocation> programlocation { get; set; }
        public DbSet<InmateSchedule> inmateschedule { get; set; }
        public DbSet<JailLensAlert> jaillensalert { get; set; }
        public DbSet<Attendance> attendance { get; set; }
        public DbSet<Response> Result { get; set; }
        public DbSet<ProgramResponse> ActualPrograms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();

            modelBuilder.Entity<ProgramLocation>()
            .HasOne(o => o.Programs)
            .WithMany()
            .HasForeignKey(o => o.programid);

            modelBuilder.Entity<ProgramLocation>()
            .HasOne(o => o.Location)
            .WithMany()
            .HasForeignKey(o => o.locationid);

            modelBuilder.Entity<InmateSchedule>()
            .HasOne(o => o.Programs)
            .WithMany()
            .HasForeignKey(o => o.programid);

            modelBuilder.Entity<Response>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<ProgramResponse>(entity =>
            {
                entity.HasNoKey();
            });
        }
    }
}
