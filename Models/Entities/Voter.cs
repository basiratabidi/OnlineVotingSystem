using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlineVotingSystem.Models.Entities.Authentication;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace OnlineVotingSystem.Models.Entities

{
    [Table("Voter")]
    public class Voter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Voter_Id")]   

        public int VoterId { get; set; }
        

        [Column("Name")]
        [Required]
        public string Name { get; set; }

        [NotMapped]
        


        [Column("Voter_Email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Column("Password")]
        [DataType(DataType.Password)]
         public string password { get; set; }
       
        // Indicates if the voter has voted
        [Column("has_voted")]
        public bool HasVoted { get; set; } = false;

        // Foreign Key to User
        [Column("Voter_UserID")]
        public int UserId { get; set; }

        // Navigation Property
        [ForeignKey("UserId")]
        public User? User { get; set; }

    }
}
