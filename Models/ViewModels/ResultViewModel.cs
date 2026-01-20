using OnlineVotingSystem.Models.Entities;

namespace OnlineVotingSystem.Models.ViewModels
{
    public class ResultViewModel
    {
        public int ElectionId { get; set; }
        public string? ElectionName { get; set; }
        public string? ElectionDescription { get; set; }
        public string? Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalVotes { get; set; }
        public List<CandidateResult> CandidateResults { get; set; } = new List<CandidateResult>();
        public List<PartyResult> PartyResults { get; set; } = new List<PartyResult>();
    }

    public class CandidateResult
    {
        public int CandidateId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string PartyName { get; set; } = string.Empty;
        public int VoteCount { get; set; }
        public double VotePercentage { get; set; }
    }

    public class PartyResult
    {
        public int PartyId { get; set; }
        public string PartyName { get; set; } = string.Empty;
        public int TotalVotes { get; set; }
        public double VotePercentage { get; set; }
        public int CandidateCount { get; set; }
        public List<string> CandidateNames { get; set; } = new List<string>();
    }
}

