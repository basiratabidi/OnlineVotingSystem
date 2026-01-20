using System.ComponentModel.DataAnnotations;

namespace OnlineVotingSystem.Models.ViewModels
{
    public class VoterRegisterModel
    {
      
          
        
        [Required]
            public string Name { get; set; }


            [Required, EmailAddress]
            public string Email { get; set; }

           

            [Required, MinLength(6)]
            public string Password { get; set; }

      
        
    }
}
