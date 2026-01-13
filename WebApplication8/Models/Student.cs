using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication8.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string FullName { get; set; } = null!;

    public int? SubjectId { get; set; }

    public DateOnly? EnrollmentDate { get; set; }

    [Column(TypeName ="decimal(18,2)")]
    public decimal? Marks { get; set; }

    public virtual Subject? Subject { get; set; }
}
