using System;
using System.Collections.Generic;

namespace DataAccess.DataModels;

public partial class WeeklyTimeSheet
{
    public int TimeSheetId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int ProviderId { get; set; }

    public int? PayRateId { get; set; }

    public int? AdminId { get; set; }

    public bool? IsFinalized { get; set; }

    public string? AdminNote { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual PayRate? PayRate { get; set; }

    public virtual Physician Provider { get; set; } = null!;

    public virtual ICollection<WeeklyTimeSheetDetail> WeeklyTimeSheetDetails { get; set; } = new List<WeeklyTimeSheetDetail>();
}
