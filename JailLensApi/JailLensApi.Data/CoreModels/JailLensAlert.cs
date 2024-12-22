using System.ComponentModel.DataAnnotations;

namespace JailLensApi.Data
{
    public class JailLensAlert
    {
        [Key]
        public int jaillensalertid { get; set; }
        public string intakeid { get; set; }
        public string programname { get; set; }
        public string inmatename { get; set; }
        public string alertdescription { get; set; }
        public string alertcategory { get; set; }
        public DateTime createddate { get; set; }
        public string? comments { get; set; }
        public int isprocessed { get; set; }
        public string actualprogramname { get; set; }
    }
}
