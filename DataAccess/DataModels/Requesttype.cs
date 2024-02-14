using System;
using System.Collections.Generic;

namespace DataAccess.DataModels;

public partial class Requesttype
{
    public int Requesttypeid { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
