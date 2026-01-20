using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineVotingSystem.Models.Entities.Authentication
{
    [Table("User")]
    public class User
    {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Column("User_id")]
            public int UserId { get; set; }

            [Column("Email")]
            public string Email { get; set; }

            [Column("hashed_password")]
            public string HashedPassword { get; set; }
        
            [Column("Role")]
            public string? Role { get; set; } // Admin or Voter
        }
    }
