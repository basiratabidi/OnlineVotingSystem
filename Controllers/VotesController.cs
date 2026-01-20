using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineVotingSystem.Models;
using OnlineVotingSystem.Models.Entities;
using OnlineVotingSystem.Models.Helper;
using System.Security.Claims;

namespace OnlineVotingSystem.Controllers
{
    [Authorize(Roles = "Voter")]
    public class VotesController : Controller
    {
        private readonly databaseContext _context;
        private readonly EmailHelper _emailHelper;

        public VotesController(databaseContext context, EmailHelper emailHelper)
        {
            _context = context;
            _emailHelper = emailHelper;
        }
        
        /* ======================================================
            STEP 1: SHOW CANDIDATES FOR SELECTED ELECTION
        ====================================================== */
        [HttpGet]
        public async Task<IActionResult> CastVote(int electionId)
        {
            // Get logged-in user's UserId
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim);

            // Get the corresponding voter
            var voter = await _context.Voters.FirstOrDefaultAsync(v => v.UserId == userId);
            if (voter == null)
            {
                TempData["Error"] = "Voter profile not found.";
                return RedirectToAction("Dashboard", "Voters");
            }

            // Check if voter has already voted in this election
            bool alreadyVoted = await _context.Votes
                .AnyAsync(v => v.VoterId == voter.VoterId && v.ElectionId == electionId);

            if (alreadyVoted)
            {
                TempData["Error"] = "You have already cast your vote for this election.";
                return RedirectToAction("Dashboard", "Voters");
            }

            // Load election details
            var election = await _context.Elections.FindAsync(electionId);
            if (election == null) return NotFound();

            // Load candidates
            var candidates = await _context.Candidates
                .Include(c => c.Party)
                .Where(c => c.ElectionId == electionId)
                .ToListAsync();

            // Create unique vote token (for verification/audit purposes)
            string voteToken = GenerateVoteToken();

            // Pass data to view
            ViewBag.VoteToken = voteToken;
            ViewBag.ElectionId = electionId;
            ViewBag.ElectionName = election.Name;

            return View(candidates);
        }

        /* ======================================================
            STEP 2: SUBMIT VOTE (ASYNC WITH TRANSACTION)
        ====================================================== */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CastVote(int electionId, int candidateId, string voteToken)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim);

            // Get corresponding voter
            var voter = await _context.Voters.FirstOrDefaultAsync(v => v.UserId == userId);
            if (voter == null)
            {
                TempData["Error"] = "Voter record not found.";
                return RedirectToAction("Dashboard", "Voters");
            }

            // Double-check if voter has already voted (Server-side validation)
            bool alreadyVoted = await _context.Votes
                .AnyAsync(v => v.VoterId == voter.VoterId && v.ElectionId == electionId);

            if (alreadyVoted)
            {
                TempData["Error"] = "Security Alert: A vote has already been recorded for this account.";
                return RedirectToAction("Dashboard", "Voters");
            }

            // Use a transaction to ensure both the Vote record and Voter status update happen together
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. Create the Vote record
                    var vote = new Vote
                    {
                        VoterId = voter.VoterId,
                        ElectionId = electionId,
                        CandidateId = candidateId,
                        VoteToken = voteToken,
                        VotedAt = DateTime.Now
                    };

                    _context.Votes.Add(vote);

                    // 2. Mark the voter as having voted
                    voter.HasVoted = true;

                    // 3. Save changes to DB
                    await _context.SaveChangesAsync();

                    // 4. Commit the transaction
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    // If anything goes wrong, roll back changes
                    await transaction.RollbackAsync();
                    TempData["Error"] = "An error occurred while processing your vote. Please try again.";
                    return RedirectToAction("CastVote", new { electionId });
                }
            }

            // --- Post-Submission: Send Confirmation Email ---
            // We do this AFTER the transaction is successful
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                var candidate = await _context.Candidates
                    .Include(c => c.Party)
                    .FirstOrDefaultAsync(c => c.CandidateId == candidateId);
                var election = await _context.Elections.FindAsync(electionId);

                if (user != null && candidate != null && election != null)
                {
                    string candidateInfo = $"{candidate.Name} ({candidate.Party?.Name ?? "Independent"})";
                    // Using the helper asynchronously
                    await _emailHelper.SendVoteConfirmationAsync(user.Email, election.Name, candidateInfo);
                }
            }
            catch
            {
                // We log email failures but don't stop the user since the vote is already saved
                // Optional: Log error here
            }

            TempData["Success"] = "Your vote has been successfully recorded.";
            return RedirectToAction("Thanks");
        }

        /* ======================================================
            THANK YOU PAGE
        ====================================================== */
        public IActionResult Thanks()
        {
            return View();
        }

        /* ======================================================
            TOKEN GENERATOR
        ====================================================== */
        private static string GenerateVoteToken()
        {
            // Use Guid for a unique identifier for the vote
            return Guid.NewGuid().ToString("N").ToUpper();
        }
    }
}