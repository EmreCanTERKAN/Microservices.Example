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
        //databaseden okumak istediğimiz tabloyu getirecek fonksiyon yazmamız gerek.
        //Dinamik bir şekilded koleksiyon çağırılır...
        public IMongoCollection<T> GetCollection<T>() => _dataBase.GetCollection<T>(typeof(T).Name.ToLowerInvariant());
    }
}
