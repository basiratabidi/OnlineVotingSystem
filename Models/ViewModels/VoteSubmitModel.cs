namespace OnlineVotingSystem.Models
{
    public class VoteSubmitModel
    {
        public int ElectionId { get; set; }
        public int CandidateId { get; set; }
        public int VoterId { get; set; }
        public string? AuthToken { get; set; }
    }


}