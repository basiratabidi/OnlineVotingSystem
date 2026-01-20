using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineVotingSystem.Models;
using OnlineVotingSystem.Models.Entities;

namespace OnlineVotingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly databaseContext _context;

        public AdminController(databaseContext context)
        {
            _context = context;
        }

        // ================= DASHBOARD =================
        public IActionResult Dashboard()
        {
            ViewBag.ElectionCount = _context.Elections.Count();
            ViewBag.VoterCount = _context.Voters.Count();
            ViewBag.PartyCount = _context.Party.Count();
            ViewBag.CandidateCount = _context.Candidates.Count();
            return View();
        }

        // ================= ELECTIONS =================
        public IActionResult Elections()
        {
            return View(_context.Elections.ToList());
        }

        public IActionResult AddElection() => View();

        [HttpPost]
        public IActionResult AddElection(Election model)
        {
            if (!ModelState.IsValid) return View(model);
            _context.Elections.Add(model);
            _context.SaveChanges();
            return RedirectToAction(nameof(Elections));
        }

        public IActionResult EditElection(int id)
        {
            var election = _context.Elections.Find(id);
            if (election == null) return NotFound();
            return View(election);
        }

        [HttpPost]
        public IActionResult EditElection(Election model)
        {
            if (!ModelState.IsValid) return View(model);
            _context.Elections.Update(model);
            _context.SaveChanges();
            return RedirectToAction(nameof(Elections));
        }

       


        // ================= PARTY =================
        [HttpGet]
        public IActionResult Parties()
        {
            var parties = _context.Party
                .Include(p => p.Election)  // Important: Include Election navigation property
                .OrderBy(p => p.ElectionId)
                .ThenBy(p => p.Name)
                .ToList();

            return View(parties);
        }
        
        public IActionResult PartiesElection(int electionId)
        {
            ViewBag.ElectionId = electionId;
            return View(_context.Party
                .Where(p => p.ElectionId == electionId)
                .ToList());
            
        }

        [HttpGet]
        public IActionResult AddParty(int electionId)
        {
            var election = _context.Elections.Find(electionId);
            if (election == null) return RedirectToAction(nameof(Elections));

            ViewBag.ElectionId = electionId;
            ViewBag.ElectionName = election.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddParty(int electionId, string partyName)
        {
            if (string.IsNullOrWhiteSpace(partyName))
            {
                TempData["Error"] = "Party name is required.";
                return RedirectToAction(nameof(AddParty), new { electionId });
            }

            bool exists = _context.Party.Any(p =>
                p.ElectionId == electionId &&
                p.Name.ToLower() == partyName.ToLower());

            if (exists)
            {
                TempData["Error"] = "Party already exists.";
                return RedirectToAction(nameof(AddParty), new { electionId });
            }

            _context.Party.Add(new Party
            {
                Name = partyName.Trim(),
                ElectionId = electionId
            });
            _context.SaveChanges();

            TempData["Success"] = "Party created successfully.";
            return RedirectToAction(nameof(Parties), new { electionId });
        }

        public IActionResult DeleteParty(int id, int electionId)
        {
            var party = _context.Party.Find(id);
            if (party != null)
            {
                _context.Party.Remove(party);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Parties), new { electionId });
        }

        // ================= CANDIDATES =================
        public IActionResult Candidates(int electionId)
        {
            ViewBag.ElectionId = electionId;
            return View(_context.Candidates
                .Include(c => c.Party)
                .Where(c => c.ElectionId == electionId)
                .ToList());
        }

        [HttpGet]
        public IActionResult RegisterCandidate(int electionId)
        {
            var election = _context.Elections
                .FirstOrDefault(e => e.ElectionId == electionId);

            if (election == null)
            {
                return NotFound();
            }

            ViewBag.ElectionId = electionId;
            ViewBag.ElectionName = election.Name;  

            ViewBag.Parties = _context.Party
                .Where(p => p.ElectionId == electionId)
                .ToList();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterCandidate(int electionId, int partyId, string candidateName)
        {
            if (string.IsNullOrWhiteSpace(candidateName))
            {
                TempData["Error"] = "Candidate name is required.";
                return RedirectToAction(nameof(RegisterCandidate), new { electionId });
            }

            candidateName = candidateName.Trim();

            // 🔒 CONDITION 1:
            // One party can have only ONE candidate
            bool partyAlreadyHasCandidate = _context.Candidates.Any(c =>
                c.ElectionId == electionId &&
                c.PartyId == partyId
            );

            if (partyAlreadyHasCandidate)
            {
                TempData["Error"] = "This party already has a Candidate.";
                return RedirectToAction(nameof(RegisterCandidate), new { electionId });
            }

            // 🔒 CONDITION 2:
            // One candidate can belong to only ONE party in the same election
            bool candidateAlreadyExists = _context.Candidates.Any(c =>
                c.ElectionId == electionId &&
                c.Name.ToLower() == candidateName.ToLower()
            );

            if (candidateAlreadyExists)
            {
                TempData["Error"] = "This Candidate is already registered in another party.";
                return RedirectToAction(nameof(RegisterCandidate), new { electionId });
            }

            // ✅ ALL CONDITIONS PASSED → ADD CANDIDATE
            _context.Candidates.Add(new Candidate
            {
                Name = candidateName,
                ElectionId = electionId,
                PartyId = partyId
            });

            _context.SaveChanges();

            TempData["Success"] = "Candidate successfully registered.";
            return RedirectToAction(nameof(Candidates), new { electionId });
        }



        // ================= VOTERS =================
        public IActionResult Voters()
        {
            var voters = _context.Voters
                .Include(v => v.User)  // important
                .ToList();

            return View(voters);
        }


        // ================= RESULTS =================
        public IActionResult Results()
        {
            var results = _context.Elections
                .Include(e => e.Party)
                .Select(e => new
                {
                    Election = e,
                    Party = e.Party
                }).ToList();

            return View(results);
        }
    }
}
