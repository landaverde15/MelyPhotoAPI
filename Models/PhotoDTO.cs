using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MelyPhotography.Models
{
    public class PhotoDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("photo")]
        public string Photo { get; set; }
    }
}