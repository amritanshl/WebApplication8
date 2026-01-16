using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication8.Models;

namespace WebApplication8.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly Amrit01132026Context _context;

        public StudentsController(Amrit01132026Context context)
        {
            _context = context;
        }
        [Authorize(Policy = "ReadAccess")]
      //  [Authorize(Policy = "AdminOnly")]
        // GET: Students
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["SubjectSortParm"] = sortOrder == "Subject" ? "subject_desc" : "Subject";
            ViewData["MarksSortParm"] = sortOrder == "Marks" ? "marks_desc" : "Marks";


            var students  = _context.Students.Include(s => s.Subject).AsQueryable();
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.FullName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                case "Subject":
                    students = students.OrderBy(s => s.Subject.SubjectName);
                    break;
                case "subject_desc":
                    students = students.OrderByDescending(s => s.Subject.SubjectName);
                    break;
                case "Marks":
                    students = students.OrderBy(s => s.Marks);
                    break;
                case "marks_desc":
                    students = students.OrderByDescending(s => s.Marks);
                    break;
                default:
                    students = students.OrderBy(s => s.FullName);
                    break;
            }

            var amrit01132026Context = _context.Students.Include(s => s.Subject);
            return View(await students.AsNoTracking().ToListAsync());
        }


        // GET: Students/Details/5
        [Authorize(Policy = "ReadAccess")]
       // [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Subject)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        //[Authorize(Policy = "ReadAccess")]
        [Authorize(Policy = "AdminOnly")]
        // GET: Students/Create
        public IActionResult Create()
        {
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "SubjectId");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize(Policy = "ReadAccess")]
        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,SubjectId,EnrollmentDate,Marks")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "SubjectId", student.SubjectId);
            return View(student);
        }
        //[Authorize(Policy = "ReadAccess")]
        [Authorize(Policy = "AdminOnly")]

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "SubjectId", student.SubjectId);
            return View(student);
        }
        //[Authorize(Policy = "ReadAccess")]
        [Authorize(Policy = "AdminOnly")]
        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentId,FullName,SubjectId,EnrollmentDate,Marks")] Student student)
        {
            if (id != student.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.StudentId))
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
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "SubjectId", student.SubjectId);
            return View(student);
        }

        // GET: Students/Delete/5
        //[Authorize(Policy = "ReadAccess")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Subject)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }
        //[Authorize(Policy = "ReadAccess")]
        [Authorize(Policy = "AdminOnly")]
        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }
    }
}
