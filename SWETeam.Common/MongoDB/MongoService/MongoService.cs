using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using SWETeam.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SWETeam.Common.MongoDB
{
    public class MongoService<TDocument> : IMongoService<TDocument>
    {
        #region Declares
        private readonly IServiceProvider _provider;
        private readonly IConfiguration _config;
        private readonly string _connectionString;
        private readonly string _databaseName;

        /// <summary>
        /// Đối tượng truy cập mongo
        /// </summary>
        private MongoDBContext _mongoDBContext;
        public MongoDBContext mongoDBContext
        {
            get
            {
                if (_mongoDBContext == null)
                {
                    _mongoDBContext = new MongoDBContext(_connectionString, _databaseName);
                }
                return _mongoDBContext;
            }
        }

        /// <summary>
        /// Đối tượng mongo collection
        /// </summary>
        private IMongoCollection<TDocument> _collection;
        public virtual IMongoCollection<TDocument> collection
        {
            get
            {
                if (_collection == null)
                {
                    _collection = mongoDBContext.GetCollection<TDocument>();
                }
                return _collection;

            }
        }
        #endregion

        #region Constructor
        public MongoService(IServiceProvider provider)
        {
            _provider = provider;
            _config = provider.GetRequiredService<IConfiguration>();
            _connectionString = _config.GetConnectionString("Cluster");
            _databaseName = _config.GetSection("Database:Mongo").Value;
        }
        #endregion

        #region Methods
        public List<TDocument> GetAll()
        {
            return collection.Find(f => true).ToList();
        }

        /// <summary>
        /// Lấy documents paging
        /// </summary>
        /// <returns></returns>
        public (List<TDocument> Data, long Total) GetPaging(PaginationRequest paginationRequest)
        {
            var filter = Builders<TDocument>.Filter.Empty;
            int skip = (paginationRequest.PageIndex - 1) * paginationRequest.PageSize;

            List<TDocument> data = collection.Find(filter).Skip(skip).Limit(paginationRequest.PageSize).ToList();
            long total = collection.CountDocuments(filter);

            return (data, total);
        }

        public TDocument Get(object id) => GetAsync(id).GetAwaiter().GetResult();

        public async Task<TDocument> GetAsync(object id)
        {
            FilterDefinition<TDocument> filter = Builders<TDocument>.Filter.Eq("_id", ObjectId.Parse(id.ToString()));
            var data = await collection.FindAsync(filter);
            return data.FirstOrDefault();
        }

        public void Insert(TDocument document) => InsertAsync(document).GetAwaiter().GetResult();

        public async Task InsertAsync(TDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            await collection.InsertOneAsync(document);
        }

        public bool InsertMany(IList<TDocument> documents) => InsertManyAsync(documents).GetAwaiter().GetResult();

        public async Task<bool> InsertManyAsync(IList<TDocument> documents)
        {
            if (documents == null || documents.Count == 0)
            {
                throw new ArgumentNullException(nameof(documents));
            }

            List<WriteModel<TDocument>> upserts = new List<WriteModel<TDocument>>();
            foreach (TDocument document in documents)
            {
                upserts.Add(new InsertOneModel<TDocument>(document));
            }

            return (await collection.BulkWriteAsync(upserts)).InsertedCount > 0;
        }

        public bool Update(object id, UpdateDefinition<TDocument> updateDefinition) => UpdateAsync(id, updateDefinition).GetAwaiter().GetResult();

        public async Task<bool> UpdateAsync(object id, UpdateDefinition<TDocument> updateDefinition)
        {
            //var filter = Builders<TDocument>.Filter.Eq("_id", id);
            //await collection.UpdateOneAsync<TDocument>(filter), updateDefinition);
            throw new NotImplementedException();
        }

        public bool Replace(TDocument document) => ReplaceAsync(document).GetAwaiter().GetResult();

        public Task<bool> ReplaceAsync(TDocument document)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReplaceManyAsync(IList<TDocument> documents)
        {
            throw new NotImplementedException();
        }

        public bool Upsert(TDocument document) => UpsertAsync(document).GetAwaiter().GetResult();
        public Task<bool> UpsertAsync(TDocument document)
        {
            throw new NotImplementedException();
        }

        public bool Delete(object id) => DeleteAsync(id).GetAwaiter().GetResult();

        public Task<bool> DeleteAsync(object id)
        {
            throw new NotImplementedException();
        }

        public async Task<TDocument> FindAsync(Expression<Func<TDocument, bool>> filter)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
