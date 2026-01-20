using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineVotingSystem.Models.Entities
{
    [Table("Candidate")]
    public class Candidate
    {
        [Column("CandidateId")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CandidateId { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Required]
        [Column("ElectionId")]
        // Foreign key
        public int ElectionId {get; set; }
        public Election Election { get; set; }


        [ForeignKey("PartyId")]
        public Party Party { get; set; }
        
        public int PartyId { get;set; }

        // Navigation property to Votes (for calculating total votes)
        // Total votes can be calculated as: Votes.Count() or Votes.Count
        public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}