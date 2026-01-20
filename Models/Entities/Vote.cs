

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineVotingSystem.Models.Entities;


[Table("Vote")]
public class Vote
{
    [Column("VoteId")]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int VoteId { get; set; }

    [Column("ElectionId")]
    [ForeignKey("ElectionId")]
    public int ElectionId { get; set; }

    [Column("CandidateId")]
    [ForeignKey("CandidateId")]
    public int CandidateId { get; set; }

    [Column("VoterId")]
    [ForeignKey("VoterId")]
    public int VoterId { get; set; }



    [Column("VotedAt")]
    public DateTime VotedAt { get; set; }

   
    public string VoteToken { get; set; }

    // Navigation Properties
    public Election? Election { get; set; }
    public Candidate? Candidate { get; set; }

   
    public Voter? Voter { get; set; }
}