using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using OnlineVotingSystem.Models.Entities;
using System.ComponentModel.DataAnnotations;


namespace OnlineVotingSystem.Models.ViewModels
{
    public class VoterLoginModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [Display(Name = "Voter")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
