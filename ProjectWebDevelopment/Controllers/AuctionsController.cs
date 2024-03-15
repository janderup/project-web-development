using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectWebDevelopment.Data;
using ProjectWebDevelopment.Data.Entities;
using ProjectWebDevelopment.Models;
using ProjectWebDevelopment.Services;

namespace ProjectWebDevelopment.Controllers
{
    public class AuctionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<AuctionUser> _userManager;

        private readonly IAuctionImageProcessor _imageProcessor;

        public AuctionsController(
            ApplicationDbContext context, 
            UserManager<AuctionUser> userManager, 
            IAuctionImageProcessor imageProcessor
            )
        {
            _context = context;
            _userManager = userManager;
            _imageProcessor = imageProcessor;
        }

        // GET: Auctions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Auctions.Include(a => a.Seller);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Auctions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auction = await _context.Auctions
                .Include(a => a.Seller)
                .Include(a => a.Images)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (auction == null)
            {
                return NotFound();
            }

            return View(auction);
        }

        // GET: Auctions/Create
        public IActionResult Create()
        {
            ViewData["SellerId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Auctions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("EndDate,Title,Description,MinimumBid,Images,Id")] CreateAuctionViewModel auctionViewModel)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    Auction auction = new()
                    {
                        StartDate = DateTime.Now,
                        EndDate = auctionViewModel.EndDate,
                        Title = auctionViewModel.Title,
                        Description = auctionViewModel.Description,
                        MinimumBid = auctionViewModel.MinimumBid,
                        SellerId = _userManager.GetUserId(this.User)
                    };
                    _context.Add(auction);

                    await _context.SaveChangesAsync();

                    if (auctionViewModel.Images != null)
                    {
                        foreach (var image in auctionViewModel.Images)
                        {
                            Image auctionImage = new()
                            {
                                Path = _imageProcessor.ProcessUploadedImage(image),
                                AuctionId = auction.Id
                            };
                            _context.Add(auctionImage);
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    
                } catch  (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["SellerId"] = new SelectList(_context.Users, "Id", "Id");
            return View(auctionViewModel);
        }

        // GET: Auctions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null)
            {
                return NotFound();
            }
            ViewData["SellerId"] = new SelectList(_context.Users, "Id", "Id", auction.SellerId);
            return View(auction);
        }

        // POST: Auctions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StartDate,EndDate,Title,Description,MinimumBid,SellerId,Id")] Auction auction)
        {
            if (id != auction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(auction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuctionExists(auction.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SellerId"] = new SelectList(_context.Users, "Id", "Id", auction.SellerId);
            return View(auction);
        }

        // GET: Auctions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auction = await _context.Auctions
                .Include(a => a.Seller)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (auction == null)
            {
                return NotFound();
            }

            return View(auction);
        }

        // POST: Auctions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction != null)
            {
                _context.Auctions.Remove(auction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuctionExists(int id)
        {
            return _context.Auctions.Any(e => e.Id == id);
        }
    }
}
