using System.ComponentModel.DataAnnotations;

namespace JailLensApi.Data
{
    public class FaceRecognitionEvents
    {
        [Key]
        public int eventid { get; set; }
        public string location { get; set; }
        public DateTime eventdatetime { get; set; }
        public string personrecognized { get; set; }
        public string linktomatchreport { get; set; }
        public string? additionalinfo { get; set; }
        public int isprocessed { get; set; }
    }
}
