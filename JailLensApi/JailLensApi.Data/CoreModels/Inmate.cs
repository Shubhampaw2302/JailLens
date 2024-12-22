using System.ComponentModel.DataAnnotations;

namespace JailLensApi.Data
{
    public class Inmate
    {
        [Key]
        public int inmateid { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string gender { get; set; }
        public int incarcerationid { get; set; }
        public DateTime dob { get; set; }
        public DateTime admitdate { get; set; }
        public string intakeid { get; set; }
        public string status { get; set; }
        public DateTime? releasedate { get; set; }
    }
}
