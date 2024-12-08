using MongoDB.Bson.Serialization.Attributes;

namespace Stock.API.Models.Entities
{
    public class Stock
    {
        // primary key verir.
        [BsonId]
        // Bu guid yapısının c# ortamında saklanmasını sağlar
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.CSharpLegacy)]
        // sırasını kontrol eder...
        [BsonElement(Order = 0)]
        public Guid Id { get; set; }



        // Bu guid yapısının c# ortamında saklanmasını sağlar
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        // sırasını kontrol eder...
        [BsonElement(Order = 1)]
        public string ProductId { get; set; }



        // Bu guid yapısının c# ortamında saklanmasını sağlar
        [BsonRepresentation(MongoDB.Bson.BsonType.Int64)]
        // sırasını kontrol eder...
        [BsonElement(Order = 2)]
        public int Count { get; set; }
    }
}
