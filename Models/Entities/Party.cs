using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineVotingSystem.Models.Entities
{
    [Table("Party")]
    public class Party
    {
        [Column("PartyId")]
        public int PartyId { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [ForeignKey("ElectionId")]
        public Election Election { get; set; }
        public int ElectionId { get; set; }

        // Navigation property to Candidates
        public virtual ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();
    }
}
