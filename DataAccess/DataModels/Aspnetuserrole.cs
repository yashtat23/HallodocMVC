using System;
using System.Collections.Generic;

namespace DataAccess.DataModels;

public partial class Aspnetuserrole
{
    public string Userid { get; set; } = null!;

    public string Roleid { get; set; }

    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

    public virtual Aspnetuser User { get; set; } = null!;
}
