using JailLensApi.Data;
using JailLensApi.Infrastructure.IService;

namespace JailLensApi.Infrastructure.Service
{
    public class InmateService : IInmateService
    {
        private readonly JailLensDbContext _dbContext;

        public InmateService(JailLensDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Inmate IsMatchFound(string base64Image)
        {
            try
            {
                Random random = new Random();

                bool randomBool = random.Next(2) == 0;

                if (randomBool)
                {
                    int total = _dbContext.inmate.Count();
                    Random r = new Random();
                    int offset = r.Next(0, total);

                    var result = _dbContext.inmate.Skip(offset).FirstOrDefault();

                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while finding a match for the inmate", ex);
            }
        }

        public async Task RegisterInmate(InmateImage inmate)
        {
            try
            {
                await AddSchedule(inmate.InmateDetails.intakeid);
                _dbContext.inmate.Add(inmate.InmateDetails);
                SaveImage(inmate.InmateDetails.intakeid, inmate.Base64Image, inmate.ImageExtension);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while registering inmate", ex);
            }
        }

        public async Task<Inmate> FindMatch(string base64Image)
        {
            try
            {
                return IsMatchFound(base64Image);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while finding match : ", ex);
            }
        }

        public async Task AddSchedule(string intakeid)
        {
            int numberOfRandomRows = 8;

            int totalCount = _dbContext.programs.Count();

            int rowsToSkip = totalCount <= numberOfRandomRows
                ? 0
                : new Random().Next(0, totalCount - numberOfRandomRows);

            var randomProgramId = _dbContext.programs.Select(program => program.programid)
                .OrderBy(r => Guid.NewGuid())
                .Skip(rowsToSkip)
                .Take(numberOfRandomRows)
                .ToList();

            foreach(var id in randomProgramId)
            {
                var schedule = new InmateSchedule
                {
                    intakeid = intakeid,
                    programid = id
                };

                _dbContext.inmateschedule.Add(schedule);
            }

            await _dbContext.SaveChangesAsync();
        }

        public void SaveImage(string imgName, string base64, string extension)
        {
            string uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsDirectory))
            {
                Directory.CreateDirectory(uploadsDirectory);
            }

            //string fileExtension = Path.GetExtension("png");
            string uniqueFileName = $"{imgName}_{Path.GetRandomFileName()}.{extension}";

            // Save the uploaded image to the server
            string filePath = Path.Combine(uploadsDirectory, uniqueFileName);

            byte[] imageBytes = Convert.FromBase64String(base64);

            System.IO.File.WriteAllBytes(filePath, imageBytes);
        }

        public async Task ReleaseInmate(ReleaseInmate inmateDetails)
        {
            try
            {
                var inmate = _dbContext.inmate.Where(inm => inm.intakeid == inmateDetails.intakeid).First();

                inmate.status = "Released";
                inmate.releasedate = inmateDetails.releaseDate;

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while releasing inmate : ", ex);
            }
        }
    }
}
