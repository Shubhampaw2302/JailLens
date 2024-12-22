using System.ComponentModel.DataAnnotations;

namespace JailLensApi.Data
{
    public class Location
    {
        [Key]
        public int locationid { get; set; }
        public string locationname { get; set; }
    }
}
