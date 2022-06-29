using MongoDB.Driver;
using SWETeam.Common.Libraries;

namespace SWETeam.Common.MongoDB
{
    public class MongoDBContext
    {
        #region Declares
        private readonly IMongoDatabase _db;
        #endregion

        #region Constructor
        public MongoDBContext(string connectionString, string databaseName)
        {
            MongoClient client = new MongoClient(connectionString);
            if (client != null)
            {
                _db = client.GetDatabase(databaseName);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get collection
        /// </summary>
        public IMongoCollection<TDocument> GetCollection<TDocument>()
        {
            string collectionName = typeof(TDocument).Name.ToSnakeCaseLower();
            return _db.GetCollection<TDocument>(collectionName);
        }
        #endregion
    }
}
