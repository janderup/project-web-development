﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectWebDevelopment.Data;
using ProjectWebDevelopment.Data.Entities;
using ProjectWebDevelopment.Models;
using ProjectWebDevelopment.Services;

namespace ProjectWebDevelopment.Controllers
{
    public class AuctionsController : Controller
    {
        private readonly SignInManager<AuctionUser> _signInManager;

        private readonly UserManager<AuctionUser> _userManager;

        private readonly AuctionService _auctionService;

        public AuctionsController(
            SignInManager<AuctionUser> signInManager,
            UserManager<AuctionUser> userManager, 
            AuctionService auctionService
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _auctionService = auctionService;
        }

        public async Task<IActionResult> Index()
        {
            var auctions = await _auctionService.GetAuctionsWithBids();
            
            var auctionViewModels = auctions.Select(auction => new AuctionListItemViewModel
            {
                Id = auction.Id,
                Title = auction.Title,
                ImagePath = auction.Images != null && auction.Images.Any() ? auction.Images.First().Path : "~/images/fallback.jpg",
                HighestBid = auction.Bids != null && auction.Bids.Any() ? auction.Bids.Max(b => b.Price) : null,
                EndDate = auction.EndDate
            });

            var viewModel = new AuctionsListViewModel(auctionViewModels, _signInManager);

            return View(viewModel);
        }


        // GET: Auctions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auction = await _auctionService.GetAuctionByIdWithBids(id.Value);
            
            if (auction == null)
            {
                return NotFound();
            }

            var canBeCancelled = _auctionService.CanAuctionBeCancelled(auction);
            var nextMinimumBid = _auctionService.GetNextMinimumBid(auction);

            var detailsViewModel = new AuctionDetailsViewModel(auction, _signInManager, canBeCancelled, nextMinimumBid);

            return View(detailsViewModel);
        }

        // GET: Auctions/Create
        [Authorize(Roles = "Seller")]
        public IActionResult Create()
        {
            var viewModel = new CreateAuctionViewModel();
            return View(viewModel);
        }

        // POST: Auctions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Create([Bind("AuctionDuration,Title,Description,MinimumBid,Images,Id")] CreateAuctionViewModel auctionViewModel)
        {
            if (ModelState.IsValid)
            {
                var endDate = DateTime.Now;
                endDate = endDate.Add(auctionViewModel.AuctionDuration.ToTimeSpan());

                var user = await _userManager.GetUserAsync(this.User);

                Auction auction = new()
                {
                    StartDate = DateTime.Now,
                    EndDate = endDate,
                    Title = auctionViewModel.Title,
                    Description = auctionViewModel.Description,
                    MinimumBid = auctionViewModel.MinimumBid,
                    SellerId = user.Id
                };

                try
                {
                    var auctionId = await _auctionService.CreateAuction(auction, auctionViewModel.Images);

                    // Add claim for authentication
                    var claim = new Claim("AuctionCreator", auctionId.ToString());
                    await _userManager.AddClaimAsync(user, claim);
                    await _signInManager.RefreshSignInAsync(user);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Handle the exception here
                    ModelState.AddModelError(string.Empty, ex.Message); // Add error message to ModelState
                }
            }

            ViewData["SellerId"] = new SelectList(_userManager.Users, "Id", "Id");
            return View(auctionViewModel);
        }

        // GET: Auctions/Edit/5
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (User.Claims.FirstOrDefault(c => c.Type == "AuctionCreator" && c.Value == id.Value.ToString()) == null)
            {
                return Unauthorized();
            }

            var auction = await _auctionService.GetAuctionById(id.Value);
            if (auction == null)
            {
                return NotFound();
            }
            ViewData["SellerId"] = new SelectList(_userManager.Users, "Id", "Id", auction.SellerId);
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

            if (User.Claims.FirstOrDefault(c => c.Type == "AuctionCreator" && c.Value == id.ToString()) == null)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _auctionService.UpdateAuction(auction);
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
            ViewData["SellerId"] = new SelectList(_userManager.Users, "Id", "Id", auction.SellerId);
            return View(auction);
        }

        // GET: Auctions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auction = await _auctionService.GetAuctionById(id.Value);
            
            if (auction == null)
            {
                return NotFound();
            }

            if (User.Claims.FirstOrDefault(c => c.Type == "AuctionCreator" && c.Value == id.Value.ToString()) == null) 
            {
                return Unauthorized();
            }

            return View(auction);
        }

        // POST: Auctions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (User.Claims.FirstOrDefault(c => c.Type == "AuctionCreator" && c.Value == id.ToString()) == null)
            {
                return Unauthorized();
            }

            var auction = await _auctionService.GetAuctionById(id);
            
            if (auction != null)
            {
                await _auctionService.DeleteAuction(auction);
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool AuctionExists(int id)
        {
            return _auctionService.AuctionExists(id);
        }

        [HttpPost, ActionName("PlaceBid")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> PlaceBid(int id, [Bind("price")] double price, [Bind("name")] string? name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return BadRequest("Captcha failed.");
            }

            var bid = new Bid()
            {
                BuyerId = _userManager.GetUserId(this.User),
                Price = price,
                Date = DateTime.Now,
                AuctionId = id
            };

            try
            {
                await _auctionService.PlaceBid(bid);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }
    }
}
