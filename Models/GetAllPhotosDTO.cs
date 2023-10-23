using MongoDB.Driver.Linq;

namespace MelyPhotography.Models
{
    public class GetAllPhotosDTO
    {
        public IMongoQueryable<PhotoDTO> Photos { get; set; } = null;
        public bool Success { get; set; }
    }
}
