using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineVotingSystem.Models.Entities // <<< VERIFY THIS NAMESPACE
{
    [Table("Elections")]
    public class Election
    {
        // Primary Key
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ElectionId { get; set; }

        // Core Election Details (Made nullable 'string?' to fix compiler warnings)
        [Column("Name")]
        public string? Name { get; set; }

        [Column(" Description")]
        public string? Description { get; set; }

        // Election Schedule (Core Requirement)
        [Column(" startdate")]
        public DateTime StartDate { get; set; }

        [Column("enddate")]
        public DateTime EndDate { get; set; }

        // Status (e.g., Pending, Active, Completed) - System managed
        [Column(" status")]
        public string? Status { get; set; }

        // Audit Field (Required by Controller for creation logic)
        public DateTime CreatedDate { get; set; }

        // Relationship to Candidate Model (For Khushi's work: One Election has Many Candidates)
        // You will need a 'Candidate.cs' class in your Entities folder for this to resolve properly.
       

        public virtual ICollection<Party>Party { get; set; }=new List<Party>();

        // Optional: Field to track who created the election (requires authentication setup)
        // public string? CreatedByUserId { get; set; } 
    }
}