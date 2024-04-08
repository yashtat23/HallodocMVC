using HalloDoc.DataModels;
using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels
{
    public class SchedulingViewModel
    {

        public List<Region> regions { get; set; }
        public List<PhysicianRegion> physicianregionlist { get; set; }
        [Required]
        public int regionid { get; set; }
        [Required]
        public int providerid { get; set; }
        public DateOnly shiftdateviewshift { get; set; }
        [Required]
        public DateTime shiftdate { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public int repeatcount { get; set; }
        public int shiftid { get; set; }
        public int shiftdetailid { get; set; }
        public string physicianname { get; set; }
        public string regionname { get; set; }

    }
    public class DayWiseScheduling
    {
        public int shiftid { get; set; }
        public DateTime date { get; set; }
        public List<Physician> physicians { get; set; }
        public List<ShiftDetail> shiftdetails { get; set; }
    }
    public class MonthWiseScheduling
    {
        public DateTime date { get; set; }
        public List<ShiftDetail> shiftdetails { get; set; }
        public List<Physician> physicians { get; set; }

    }
    public class WeekWiseScheduling
    {
        public DateTime date { get; set; }
        public List<Physician> physicians { get; set; }

        public List<ShiftDetail> shiftdetails { get; set; }

    }

    public class ProviderOnCall
    {
        public IEnumerable<ShiftDetail> shiftdetaillist { get; set; }
        public IEnumerable<Shift> shiftlist { get; set; }
        public IEnumerable<Physician> ondutyphysicianlist { get; set; }
        public IEnumerable<Physician> offdutyphysicianlist { get; set; }
        public List<Region> regions { get; set; }

    }

}
