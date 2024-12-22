using System.ComponentModel.DataAnnotations;

namespace JailLensApi.Data
{
    public class Programs
    {
        [Key]
        public int programid { get; set; }
        public string programname { get; set; }
        public string programdefaultlocation { get; set; }
        public TimeOnly defaultstarttime { get; set; }
        public TimeOnly defaultendtime { get; set; }
    }
}
