namespace MelyPhotography.Models
{
    public class GetPhotosDTO
    {
        public List<string> photos { get; set; } = null;
        public bool success { get; set; }
    }
}
