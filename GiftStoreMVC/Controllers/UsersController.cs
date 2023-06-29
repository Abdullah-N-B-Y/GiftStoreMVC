﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GiftStoreMVC.Models;

namespace GiftStoreMVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly ModelContext _context;

        public UsersController(ModelContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.GiftstoreUsers.Include(g => g.Category).Include(g => g.Role);
            return View(await modelContext.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.GiftstoreUsers == null)
            {
                return NotFound();
            }

            var giftstoreUser = await _context.GiftstoreUsers
                .Include(g => g.Category)
                .Include(g => g.Role)
                .FirstOrDefaultAsync(m => m.Userid == id);
            if (giftstoreUser == null)
            {
                return NotFound();
            }

            return View(giftstoreUser);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["Categoryid"] = new SelectList(_context.GiftstoreCategories, "Categoryid", "Categoryid");
            ViewData["Roleid"] = new SelectList(_context.GiftstoreRoles, "Roleid", "Roleid");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Userid,Username,Password,Email,Name,Approvalstatus,Phonenumber,Imagepath,Categoryid,Roleid,Profits")] GiftstoreUser giftstoreUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(giftstoreUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Categoryid"] = new SelectList(_context.GiftstoreCategories, "Categoryid", "Categoryid", giftstoreUser.Categoryid);
            ViewData["Roleid"] = new SelectList(_context.GiftstoreRoles, "Roleid", "Roleid", giftstoreUser.Roleid);
            return View(giftstoreUser);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.GiftstoreUsers == null)
            {
                return NotFound();
            }

            var giftstoreUser = await _context.GiftstoreUsers.FindAsync(id);
            if (giftstoreUser == null)
            {
                return NotFound();
            }
            ViewData["Categoryid"] = new SelectList(_context.GiftstoreCategories, "Categoryid", "Categoryid", giftstoreUser.Categoryid);
            ViewData["Roleid"] = new SelectList(_context.GiftstoreRoles, "Roleid", "Roleid", giftstoreUser.Roleid);
            return View(giftstoreUser);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Userid,Username,Password,Email,Name,Approvalstatus,Phonenumber,Imagepath,Categoryid,Roleid,Profits")] GiftstoreUser giftstoreUser)
        {
            if (id != giftstoreUser.Userid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(giftstoreUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiftstoreUserExists(giftstoreUser.Userid))
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
            ViewData["Categoryid"] = new SelectList(_context.GiftstoreCategories, "Categoryid", "Categoryid", giftstoreUser.Categoryid);
            ViewData["Roleid"] = new SelectList(_context.GiftstoreRoles, "Roleid", "Roleid", giftstoreUser.Roleid);
            return View(giftstoreUser);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.GiftstoreUsers == null)
            {
                return NotFound();
            }

            var giftstoreUser = await _context.GiftstoreUsers
                .Include(g => g.Category)
                .Include(g => g.Role)
                .FirstOrDefaultAsync(m => m.Userid == id);
            if (giftstoreUser == null)
            {
                return NotFound();
            }

            return View(giftstoreUser);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.GiftstoreUsers == null)
            {
                return Problem("Entity set 'ModelContext.GiftstoreUsers'  is null.");
            }
            var giftstoreUser = await _context.GiftstoreUsers.FindAsync(id);
            if (giftstoreUser != null)
            {
                _context.GiftstoreUsers.Remove(giftstoreUser);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GiftstoreUserExists(decimal id)
        {
          return (_context.GiftstoreUsers?.Any(e => e.Userid == id)).GetValueOrDefault();
        }
    }
}