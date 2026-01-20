using Microsoft.EntityFrameworkCore;
using OnlineVotingSystem.Models.Entities;
using OnlineVotingSystem.Models.Entities.Authentication;

namespace OnlineVotingSystem.Models
{
    public class databaseContext : DbContext
    {
        public databaseContext(DbContextOptions<databaseContext> options) : base(options)
        {
        }

        public DbSet<Voter> Voters { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Election> Elections { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Party> Party { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Vote -> Candidate (Break Path)
            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Candidate)
                .WithMany(c => c.Votes)
                .HasForeignKey(v => v.CandidateId)
                .OnDelete(DeleteBehavior.NoAction);

            // 2. Vote -> Election (Break Path)
            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Election)
                .WithMany()
                .HasForeignKey(v => v.ElectionId)
                .OnDelete(DeleteBehavior.NoAction);

            // 3. Vote -> Voter (Break Path)
            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Voter)
                .WithMany()
                .HasForeignKey(v => v.VoterId)
                .OnDelete(DeleteBehavior.NoAction);

            // 4. Candidate -> Election (Break Path)
            modelBuilder.Entity<Candidate>()
                .HasOne(c => c.Election)
                .WithMany()
                .HasForeignKey(c => c.ElectionId)
                .OnDelete(DeleteBehavior.NoAction);

            // 5. Party -> Election (Break Path)
            modelBuilder.Entity<Party>()
                .HasOne(p => p.Election)
                .WithMany(e => e.Party)
                .HasForeignKey(p => p.ElectionId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}