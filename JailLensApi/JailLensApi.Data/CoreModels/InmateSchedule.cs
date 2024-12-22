using System.ComponentModel.DataAnnotations;

namespace JailLensApi.Data
{
    public class InmateSchedule
    {
        [Key]
        public int inmatescheduleid { get; set; }
        public string intakeid { get; set; }
        public int programid { get; set; }
        public Programs Programs { get; set; }
    }
}
