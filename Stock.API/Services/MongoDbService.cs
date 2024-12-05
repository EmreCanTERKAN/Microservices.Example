using MongoDB.Driver;

namespace Stock.API.Services
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _dataBase;

        public MongoDbService(IConfiguration configuration)
        {
            MongoClient client = new(configuration.GetConnectionString("MongoDb"));
            _dataBase = client.GetDatabase("StockAPIDB");
        }



    }
}
