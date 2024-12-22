namespace JailLensApi.Data
{
    public class InmateImage
    {
        public Inmate InmateDetails { get; set; }
        public string Base64Image { get; set; }
        public string ImageExtension { get; set; }
    }
}
