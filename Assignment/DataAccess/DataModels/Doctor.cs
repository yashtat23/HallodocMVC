using System;
using System.Collections.Generic;

namespace DataAccess.DataModels;

public partial class Doctor
{
    public int Doctorid { get; set; }

    public DateTime Createddate { get; set; }

    public string? Specialist { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
