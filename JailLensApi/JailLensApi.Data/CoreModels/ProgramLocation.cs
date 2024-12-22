using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JailLensApi.Data
{
    public class ProgramLocation
    {
        [Key]
        public int programlocationid { get; set; }
        public int programid { get; set; }
        public int locationid { get; set; }
        public Programs Programs { get; set; }
        public Location Location { get; set; }
    }
}
