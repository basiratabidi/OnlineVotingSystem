using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineVotingSystem.Models;
using OnlineVotingSystem.Models.Entities;
using OnlineVotingSystem.Models.ViewModels;

namespace OnlineVotingSystem.Controllers
{
    [Authorize(Roles = "Admin,Voter")]
    public class ResultsController : Controller
    {
        private readonly databaseContext _context;

        public ResultsController(databaseContext context)
        {
            _context = context;
        }

        // GET: Results/Index - Display list of elections
        public IActionResult Index()
        {
            var elections = _context.Elections
                .OrderByDescending(e => e.EndDate)
                .ToList();

            return View(elections);
        }

        // GET: Results/Details/{id} - Display detailed results for a specific election
        public IActionResult Details(int id)
        {
            var election = _context.Elections
                .FirstOrDefault(e => e.ElectionId == id);

            if (election == null)
            {
                return NotFound();
            }

            // Get all candidates for this election
            var candidates = _context.Candidates
                .Include(c => c.Party)
                .Where(c => c.ElectionId == id)
                .ToList();

            // Get all votes for this election
            var votes = _context.Votes
                .Where(v => v.ElectionId == id)
                .ToList();

            // Calculate total votes
            int totalVotes = votes.Count;

            // Build candidate results with vote counts
            var candidateResults = candidates.Select(candidate =>
            {
                int voteCount = votes.Count(v => v.CandidateId == candidate.CandidateId);
                double votePercentage = totalVotes > 0 ? (voteCount * 100.0) / totalVotes : 0;

                return new CandidateResult
                {
                    CandidateId = candidate.CandidateId,
                    CandidateName = candidate.Name ?? "Unknown",
                    PartyName = candidate.Party?.Name ?? "Independent",
                    VoteCount = voteCount,
                    VotePercentage = Math.Round(votePercentage, 2)
                };
            })
            .OrderByDescending(cr => cr.VoteCount)
            .ToList();

            // Calculate party-wise results
            var parties = _context.Party
                .Where(p => p.ElectionId == id)
                .Include(p => p.Election)
                .ToList();

            var partyResults = parties.Select(party =>
            {
                // Get all candidates for this party
                var partyCandidates = candidates.Where(c => c.PartyId == party.PartyId).ToList();
                
                // Calculate total votes for this party (sum of all candidate votes in this party)
                int partyVoteCount = 0;
                var candidateNames = new List<string>();
                
                foreach (var candidate in partyCandidates)
                {
                    int candidateVotes = votes.Count(v => v.CandidateId == candidate.CandidateId);
                    partyVoteCount += candidateVotes;
                    if (!string.IsNullOrEmpty(candidate.Name))
                    {
                        candidateNames.Add(candidate.Name);
                    }
                }

                double partyVotePercentage = totalVotes > 0 ? (partyVoteCount * 100.0) / totalVotes : 0;

                return new PartyResult
                {
                    PartyId = party.PartyId,
                    PartyName = party.Name ?? "Unknown Party",
                    TotalVotes = partyVoteCount,
                    VotePercentage = Math.Round(partyVotePercentage, 2),
                    CandidateCount = partyCandidates.Count,
                    CandidateNames = candidateNames
                };
            })
            .OrderByDescending(pr => pr.TotalVotes)
            .ToList();

            // Create ViewModel
            var viewModel = new ResultViewModel
            {
                ElectionId = election.ElectionId,
                ElectionName = election.Name,
                ElectionDescription = election.Description,
                Status = election.Status,
                StartDate = election.StartDate,
                EndDate = election.EndDate,
                TotalVotes = totalVotes,
                CandidateResults = candidateResults,
                PartyResults = partyResults
            };

            return View(viewModel);
        }
    }
}

