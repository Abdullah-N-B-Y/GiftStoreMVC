﻿using GiftStoreMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GiftStoreMVC.Controllers
{
    public class MakerController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public MakerController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            decimal? id = HttpContext.Session.GetInt32("UserId");
            var currentUser = _context.GiftstoreUsers.Where(obj => obj.Userid == id).SingleOrDefault();
            ViewData["Username"] = currentUser?.Username;
            ViewData["Password"] = currentUser?.Password;
            ViewData["UserId"] = id;
            ViewData["RoleId"] = currentUser?.Roleid;
            return View(currentUser);
        }


        public IActionResult Category()
        {
            decimal? id = HttpContext.Session.GetInt32("UserId");
            var currentUser = _context.GiftstoreUsers.Where(obj => obj.Userid == id).SingleOrDefault();
            ViewData["Username"] = currentUser?.Username;
            ViewData["Password"] = currentUser?.Password;
            ViewData["UserId"] = id;
            ViewData["RoleId"] = currentUser?.Roleid;
            ViewData["CategoryId"] = currentUser?.Categoryid;

            //var users = _context.GiftstoreUsers.ToList();
            var category = _context.GiftstoreCategories.Where(obj=> obj.Categoryid == currentUser.Categoryid).SingleOrDefault();


            //var userCategories = from u in users
            //                     join c in categories on u.Categoryid equals c.Categoryid
            //                     select new UserCategory
            //                     {
            //                         GiftstoreUser = u,
            //                         GiftstoreCategory = c
            //                     };

            return View(category);
        }

        public IActionResult Gifts(decimal? categoryId)
        {
            decimal? id = HttpContext.Session.GetInt32("UserId");
            var currentUser = _context.GiftstoreUsers.Where(obj => obj.Userid == id).SingleOrDefault();
            ViewData["Username"] = currentUser?.Username;
            ViewData["Password"] = currentUser?.Password;
            ViewData["UserId"] = id;
            ViewData["RoleId"] = currentUser?.Roleid;
            ViewData["CategoryId"] = currentUser?.Categoryid;
            //var users = _context.GiftstoreUsers.ToList();
            //var categories = _context.GiftstoreCategories.ToList();
            var user = _context.GiftstoreUsers
            .SingleOrDefault(obj => obj.Userid == id && obj.Categoryid == currentUser.Categoryid);

                var gifts = _context.GiftstoreGifts
                    .Where(obj => obj.Userid == user.Userid)
                    .ToList();
                // Process the retrieved gifts for the specific user
            
            return View(gifts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GiftstoreGift gift, int? giftAvailability)
        {
            decimal? id = HttpContext.Session.GetInt32("UserId");
            var currentUser = _context.GiftstoreUsers.Where(obj => obj.Userid == id).SingleOrDefault();
            ViewData["Username"] = currentUser?.Username;
            ViewData["Password"] = currentUser?.Password;
            ViewData["UserId"] = id;
            ViewData["RoleId"] = currentUser?.Roleid;
            ViewData["CategoryId"] = currentUser?.Categoryid;
            if (ModelState.IsValid)
            {
                if (gift.GiftImage != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + gift.GiftImage.FileName;
                    string path = Path.Combine(wwwRootPath, "GiftsImages", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await gift.GiftImage.CopyToAsync(fileStream);
                    }
                    gift.Imagepath = fileName;
                }

                gift.Giftavailability = giftAvailability;
                gift.Categoryid = (decimal)ViewData["CategoryId"];
                gift.Userid = id;

                _context.Add(gift);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Userid"] = new SelectList(_context.GiftstoreUsers, "Userid", "Email");
            return View(gift);
        }


        public async Task<IActionResult> Details(decimal? id)
        {
            decimal? id2 = HttpContext.Session.GetInt32("UserId");
            var currentUser = _context.GiftstoreUsers.Where(obj => obj.Userid == id).SingleOrDefault();
            ViewData["Username"] = currentUser?.Username;
            ViewData["Password"] = currentUser?.Password;
            ViewData["UserId"] = id2;
            ViewData["RoleId"] = currentUser?.Roleid;

            if (id == null || _context.GiftstoreGifts == null)
            {
                return NotFound();
            }

            var giftstoreGift = await _context.GiftstoreGifts
                .Include(g => g.Category)
                .Include(g => g.Order)
                .FirstOrDefaultAsync(m => m.Giftid == id);
            if (giftstoreGift == null)
            {
                return NotFound();
            }

            return View(giftstoreGift);
        }

        public IActionResult Create()
        {
            decimal? id = HttpContext.Session.GetInt32("UserId");
            var currentUser = _context.GiftstoreUsers.Where(obj => obj.Userid == id).SingleOrDefault();
            ViewData["Username"] = currentUser?.Username;
            ViewData["Password"] = currentUser?.Password;
            ViewData["UserId"] = id;
            ViewData["RoleId"] = currentUser?.Roleid;
            ViewData["CategoryId"] = currentUser?.Categoryid;

            return View();
        }

        public async Task<IActionResult> Edit(decimal? id)
        {
            decimal? id2 = HttpContext.Session.GetInt32("UserId");
            var currentUser = _context.GiftstoreUsers.Where(obj => obj.Userid == id).SingleOrDefault();
            ViewData["Username"] = currentUser?.Username;
            ViewData["Password"] = currentUser?.Password;
            ViewData["UserId"] = id2;
            ViewData["RoleId"] = currentUser?.Roleid;

            if (id == null || _context.GiftstoreGifts == null)
            {
                return NotFound();
            }

            var giftstoreGift = await _context.GiftstoreGifts.FindAsync(id);
            if (giftstoreGift == null)
            {
                return NotFound();
            }
            ViewData["Categoryid"] = new SelectList(_context.GiftstoreCategories, "Categoryid", "Categoryid", giftstoreGift.Categoryid);
            ViewData["Orderid"] = new SelectList(_context.GiftstoreOrders, "Orderid", "Orderid", giftstoreGift.Orderid);
            return View(giftstoreGift);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Giftid,Giftname,Giftprice,Imagepath,Giftavailability,Giftdescription,Categoryid,Orderid")] GiftstoreGift giftstoreGift)
        {
            decimal? id2 = HttpContext.Session.GetInt32("UserId");
            var currentUser = _context.GiftstoreUsers.Where(obj => obj.Userid == id).SingleOrDefault();
            ViewData["Username"] = currentUser?.Username;
            ViewData["Password"] = currentUser?.Password;
            ViewData["UserId"] = id2;
            ViewData["RoleId"] = currentUser?.Roleid;

            if (id != giftstoreGift.Giftid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(giftstoreGift);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiftstoreGiftExists(giftstoreGift.Giftid))
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
            ViewData["Categoryid"] = new SelectList(_context.GiftstoreCategories, "Categoryid", "Categoryid", giftstoreGift.Categoryid);
            ViewData["Orderid"] = new SelectList(_context.GiftstoreOrders, "Orderid", "Orderid", giftstoreGift.Orderid);
            return View(giftstoreGift);
        }

        public async Task<IActionResult> Delete(decimal? id)
        {
            decimal? id2 = HttpContext.Session.GetInt32("UserId");
            var currentUser = _context.GiftstoreUsers.Where(obj => obj.Userid == id).SingleOrDefault();
            ViewData["Username"] = currentUser?.Username;
            ViewData["Password"] = currentUser?.Password;
            ViewData["UserId"] = id2;
            ViewData["RoleId"] = currentUser?.Roleid;

            if (id == null || _context.GiftstoreGifts == null)
            {
                return NotFound();
            }

            var giftstoreGift = await _context.GiftstoreGifts
                .Include(g => g.Category)
                .Include(g => g.Order)
                .FirstOrDefaultAsync(m => m.Giftid == id);
            if (giftstoreGift == null)
            {
                return NotFound();
            }

            return View(giftstoreGift);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            decimal? id2 = HttpContext.Session.GetInt32("UserId");
            var currentUser = _context.GiftstoreUsers.Where(obj => obj.Userid == id).SingleOrDefault();
            ViewData["Username"] = currentUser?.Username;
            ViewData["Password"] = currentUser?.Password;
            ViewData["UserId"] = id2;
            ViewData["RoleId"] = currentUser?.Roleid;

            if (_context.GiftstoreGifts == null)
            {
                return Problem("Entity set 'ModelContext.GiftstoreGifts'  is null.");
            }
            var giftstoreGift = await _context.GiftstoreGifts.FindAsync(id);
            if (giftstoreGift != null)
            {
                _context.GiftstoreGifts.Remove(giftstoreGift);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Notification(decimal? Categoryid)
        {
            decimal? id = HttpContext.Session.GetInt32("UserId");
            var currentUser = _context.GiftstoreUsers.Where(obj => obj.Userid == id).SingleOrDefault();
            ViewData["Username"] = currentUser.Username;
            ViewData["Password"] = currentUser.Password;
            ViewData["UserId"] = id;
            ViewData["RoleId"] = currentUser.Roleid;
            ViewData["RoleName"] = HttpContext.Session.GetString("RoleName"); ;

            var users = _context.GiftstoreUsers.Where(obj => obj.Roleid == 3).ToList();
            var notifications = _context.GiftstoreNotifications.ToList();

            var model = from user in users
                        join notification in notifications
                        on user.Email equals notification.Email
                        select new UsersNotifications
                        {
                            GiftstoreUser = user,
                            GiftstoreNotification = notification
                        };
            return View(model);
        }

        private bool GiftstoreGiftExists(decimal id)
        {
            return (_context.GiftstoreGifts?.Any(e => e.Giftid == id)).GetValueOrDefault();
        }

    }
}
