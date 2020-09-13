using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using laundry.Data;
using laundry.Models;

namespace laundry.Controllers
{
    public class AdminRequestController : Controller
    {
        private readonly laundryNewDbContext _context;

        public AdminRequestController(laundryNewDbContext context)
        {
            _context = context;
        }

        // GET: Customers1
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customers.ToListAsync());
        }

        // GET: Customers1/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var customers = await _context.Customers
        //        .FirstOrDefaultAsync(m => m.CustomerId == id);
        //    if (customers == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(customers);
        //}

        // GET: Customers1/Create
        public IActionResult Edit(int id = 0)
        {
            if (id == 0)
                return View(new Customers());
            else
                return View(_context.Customers.Find(id));
        }

        // POST: Customers1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("CustomerId,Name,Email,FPick_Up_Date,Type_Of_Ser,Address,Phone_number")] Customers customers)
        {
            if (ModelState.IsValid)
            {
                if (customers.CustomerId == 0)
                    _context.Add(customers);
                else
                    _context.Update(customers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customers);
        }

        // GET: Customers1/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var customers = await _context.Customers.FindAsync(id);
        //    if (customers == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(customers);
        //}

        //// POST: Customers1/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("CustomerId,Name,Email,FPick_Up_Date,Type_Of_Ser,Address,Phone_number")] Customers customers)
        //{
        //    if (id != customers.CustomerId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(customers);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CustomersExists(customers.CustomerId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(customers);
        //}

        // GET: Customers1/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var customers = await _context.Customers
        //        .FirstOrDefaultAsync(m => m.CustomerId == id);
        //    if (customers == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(customers);
        //}

        //// POST: Customers1/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var customers = await _context.Customers.FindAsync(id);
            _context.Customers.Remove(customers);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //private bool CustomersExists(int id)
        //{
        //    return _context.Customers.Any(e => e.CustomerId == id);
        //}
    }
}
