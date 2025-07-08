using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPlanner.Data;
using MyPlanner.Models;
using System.Security.Claims;

namespace MyPlanner.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly FinanceContext _context;

        public TransactionsController(FinanceContext context)
        {
            _context = context;
        }


        [HttpGet("by-month")]
        public IActionResult GetByMonth(int year, int month)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var transactions = _context.Transactions
                .Where(t => t.Date.Year == year && t.Date.Month == month && t.UserId == userId)
                .OrderBy(t => t.Date)
                .ToList();

            return Ok(transactions);
        }

        [HttpGet("moneyboxTotal")]
        public async Task<IActionResult> GetTotalMoneybox()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var total = await _context.Moneyboxes
                .Where(m => m.UserId == userId)
                .SumAsync(m => m.Operation == "Add" ? m.Amount : -m.Amount);
            return Ok(new { total });
        }

        [HttpGet("moneyboxHistory")]
        public async Task<IActionResult> GetMoneyboxHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var history = await _context.Moneyboxes
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.Date)
                .ToListAsync();
            return Ok(history);
        }

        [HttpPost("moneyboxPut")]
        public async Task<IActionResult> AddMoneyboxOperation([FromBody] Moneybox model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.UserId = userId;
            model.Date = DateTime.Now;
            _context.Moneyboxes.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        // 1. Entrate/Uscite mensili (ultimo anno)
        [HttpGet("monthly-summary")]
        public async Task<IActionResult> GetMonthlySummary()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var now = DateTime.Now;
            var start = now.AddMonths(-11).Date;

            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId && t.Date >= start)
                .ToListAsync();

            var months = Enumerable.Range(0, 12)
                .Select(i => start.AddMonths(i))
                .ToList();

            var entrate = months.Select(m =>
                transactions.Where(t => t.Type == "Entrata" && t.Date.Month == m.Month && t.Date.Year == m.Year)
                            .Sum(t => t.Amount)
            ).ToList();

            var uscite = months.Select(m =>
                transactions.Where(t => t.Type == "Uscita" && t.Date.Month == m.Month && t.Date.Year == m.Year)
                            .Sum(t => t.Amount)
            ).ToList();

            var labels = months.Select(m => m.ToString("MMM", new System.Globalization.CultureInfo("it-IT"))).ToList();

            return Ok(new { labels, entrate, uscite });
        }

        [HttpGet("entrate-categorie")]
        public async Task<IActionResult> GetEntrateCategorie(string period = "anno")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var now = DateTime.Now;
            DateTime? fromDate;
            switch (period)
            {
                case "oggi":
                    fromDate = now.Date;
                    break;
                case "settimana":
                    fromDate = now.Date.AddDays(-6);
                    break;
                case "mese":
                    fromDate = now.Date.AddMonths(-1).AddDays(1);
                    break;
                case "anno":
                    fromDate = now.Date.AddYears(-1).AddDays(1);
                    break;
                case "sempre":
                default:
                    fromDate = null;
                    break;
            }
            var query = _context.Transactions
                .Where(t => t.UserId == userId && t.Type == "Entrata");

            if (fromDate.HasValue)
                query = query.Where(t => t.Date >= fromDate.Value && t.Date <= now);

            var data = await query
                .GroupBy(t => t.Category)
                .Select(g => new { category = g.Key, amount = g.Sum(t => t.Amount) })
                .ToListAsync();

            var labels = data.Select(d => d.category).ToList();
            var amounts = data.Select(d => d.amount).ToList();

            return Ok(new { labels, data = amounts });
        }

        [HttpGet("uscite-categorie")]
        public async Task<IActionResult> GetUsciteCategorie(string period = "anno")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var now = DateTime.Now;
            DateTime? fromDate;
            switch (period)
            {
                case "oggi":
                    fromDate = now.Date;
                    break;
                case "settimana":
                    fromDate = now.Date.AddDays(-6);
                    break;
                case "mese":
                    fromDate = now.Date.AddMonths(-1).AddDays(1);
                    break;
                case "anno":
                    fromDate = now.Date.AddYears(-1).AddDays(1);
                    break;
                case "sempre":
                default:
                    fromDate = null;
                    break;
            }
            var query = _context.Transactions
                .Where(t => t.UserId == userId && t.Type == "Uscita");

            if (fromDate.HasValue)
                query = query.Where(t => t.Date >= fromDate.Value && t.Date <= now);

            var data = await query
                .GroupBy(t => t.Category)
                .Select(g => new { category = g.Key, amount = g.Sum(t => t.Amount) })
                .ToListAsync();

            var labels = data.Select(d => d.category).ToList();
            var amounts = data.Select(d => d.amount).ToList();

            return Ok(new { labels, data = amounts });
        }

        // GET: api/Transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            return await _context.Transactions
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId)
                .ToListAsync();

            var totalEntrate = transactions.Where(t => t.Type == "Entrata").Sum(t => t.Amount);
            var totalUscite = transactions.Where(t => t.Type == "Uscita").Sum(t => t.Amount);

            return Ok(new
            {
                totalEntrate,
                totalUscite
            });
        }

        // POST: api/Transactions
        // MyPlanner\Controllers\TransactionsController.cs
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
        {
            // Prendi l'ID dell'utente dal token JWT
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            transaction.UserId = userId;
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTransactions), new { id = transaction.Id }, transaction);
        }

        // DELETE: api/Transactions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
