namespace JailLensApi.Data
{
    public class Attendance
    {
        public int id { get; set; }
        public string intakeid { get; set; }
        public string programname { get; set; }
        public DateTime timeslot { get; set; }
        public string attendance { get; set; }
        public string? actualprogramname { get; set; }
    }
}
