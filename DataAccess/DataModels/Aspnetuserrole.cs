using System;
using System.Collections.Generic;

namespace DataAccess.DataModels;

public partial class Aspnetuserrole
{
    public string Userid { get; set; } = null!;

    public int Roleid { get; set; }

    public virtual Aspnetuser User { get; set; } = null!;
}
