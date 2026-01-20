using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineVotingSystem.Models.ViewModels // <<< Make sure this namespace matches your project structure
{
    public class ElectionViewModel
    {
        // 1. ElectionId: Used to identify the election when editing (optional for Create)
        public int ElectionId { get; set; }

        // 2. Name: Required field with a maximum length
        [Required(ErrorMessage = "The election name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        [Display(Name = "Election Name")]
        public string Name { get; set; }

        // 3. Description: Optional field, but useful for detailing the election
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        // 4. StartDate: Required field for the election schedule
        [Required(ErrorMessage = "A start date is required.")]
        [Display(Name = "Voting Start Date")]
        [DataType(DataType.DateTime)] // Helps the HTML view render a date/time input
        public DateTime StartDate { get; set; }

        // 5. EndDate: Required field for the election schedule
        [Required(ErrorMessage = "A closing date is required.")]
        [Display(Name = "Voting End Date")]
        [DataType(DataType.DateTime)] // Helps the HTML view render a date/time input
        public DateTime EndDate { get; set; }

        // NOTE: You do NOT include Status or CreatedDate here, as those are 
        // system-managed fields that the AdminController sets internally.
    }
}