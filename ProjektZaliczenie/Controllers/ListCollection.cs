using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjektZaliczenie.Context;
using ProjektZaliczenie.Entities;

namespace ProjektZaliczenie.Controllers
{
    public class ListCollection : Controller
    {
        private readonly MyDbContext _context;
        private const int PageSize = 20;

        public ListCollection(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var gamesWithPublishers = _context.Games
                .Include(g => g.Genre)
                .Include(g => g.GamePublishers)
                .ThenInclude(gp => gp.Publisher);

            var totalItems = await gamesWithPublishers.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            var games = await gamesWithPublishers
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            const int range = 3;
            var startPage = Math.Max(1, page - range);
            var endPage = Math.Min(totalPages, page + range);

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["StartPage"] = startPage;
            ViewData["EndPage"] = endPage;

            return View(games);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var game = await _context.Games
                .Include(g => g.Genre)
                .Include(g => g.GamePublishers)
                .ThenInclude(gp => gp.Publisher)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (game == null) return NotFound();

            return View(game);
        }

        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "GenreName");
            ViewBag.Publishers = new MultiSelectList(_context.Publishers, "Id", "PublisherName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GenreId,GameName")] Game game, int[] selectedPublishers)
        {
            if (ModelState.IsValid)
            {
                _context.Add(game);
                await _context.SaveChangesAsync();

                foreach (var publisherId in selectedPublishers)
                {
                    _context.GamePublishers.Add(new GamePublisher
                    {
                        GameId = game.Id,
                        PublisherId = publisherId
                    });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "GenreName", game.GenreId);
            ViewBag.Publishers = new MultiSelectList(_context.Publishers, "Id", "PublisherName", selectedPublishers);
            return View(game);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var game = await _context.Games
                .Include(g => g.GamePublishers)
                .ThenInclude(gp => gp.Publisher)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null) return NotFound();

            ViewBag.Publishers = new MultiSelectList(_context.Publishers, "Id", "PublisherName",
                game.GamePublishers.Select(gp => gp.PublisherId));
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "GenreName", game.GenreId);
            return View(game);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GenreId,GameName")] Game game, int[] selectedPublishers)
        {
            if (id != game.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);

                    var existingPublishers = _context.GamePublishers.Where(gp => gp.GameId == game.Id);
                    _context.GamePublishers.RemoveRange(existingPublishers);

                    foreach (var publisherId in selectedPublishers)
                    {
                        _context.GamePublishers.Add(new GamePublisher
                        {
                            GameId = game.Id,
                            PublisherId = publisherId
                        });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Publishers = new MultiSelectList(_context.Publishers, "Id", "PublisherName", selectedPublishers);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "GenreName", game.GenreId);
            return View(game);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var game = await _context.Games
                .Include(g => g.Genre)
                .Include(g => g.GamePublishers)
                .ThenInclude(gp => gp.Publisher)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (game == null) return NotFound();

            return View(game);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var gamePublishers = _context.GamePublishers.Where(gp => gp.GameId == id).ToList();
                _context.GamePublishers.RemoveRange(gamePublishers);
                await _context.SaveChangesAsync();

                var game = await _context.Games.FindAsync(id);
                if (game != null)
                {
                    _context.Games.Remove(game);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas usuwania: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.Id == id);
        }
    }
}
