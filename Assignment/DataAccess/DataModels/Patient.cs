using System;
using System.Collections.Generic;

namespace DataAccess.DataModels;

public partial class Patient
{
    public int Id { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public int Doctorid { get; set; }

    public int? Age { get; set; }

    public string Email { get; set; } = null!;

    public string? Phoneno { get; set; }

    public string? Disease { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Createddate { get; set; }

    public DateTime? Modifieddate { get; set; }

    public string? Gender { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;
}
