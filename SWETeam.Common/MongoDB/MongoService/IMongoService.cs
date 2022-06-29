using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SWETeam.Common.Entities;

namespace SWETeam.Common.MongoDB
{
    public interface IMongoService<TDocument>
    {
        IMongoCollection<TDocument> collection { get; }

        /// <summary>
        /// Lấy full document
        /// </summary>
        /// <returns></returns>
        List<TDocument> GetAll();

        /// <summary>
        /// Lấy documents paging
        /// </summary>
        /// <returns></returns>
        (List<TDocument> Data, long Total) GetPaging(PaginationRequest paginationRequest);

        /// <summary>
        /// Lấy document theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TDocument Get(object id);

        /// <summary>
        /// Lấy document theo id async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TDocument> GetAsync(object id);

        /// <summary>
        /// Thêm document
        /// </summary>
        /// <param name="document"></param>
        void Insert(TDocument document);

        /// <summary>
        /// Thêm document async
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        Task InsertAsync(TDocument document);

        /// <summary>
        /// Thêm nhiều documents
        /// </summary>
        /// <param name="documents"></param>
        /// <returns></returns>
        bool InsertMany(IList<TDocument> documents);

        /// <summary>
        /// Thêm nhiều documents async
        /// </summary>
        /// <param name="documents"></param>
        /// <returns></returns>
        Task<bool> InsertManyAsync(IList<TDocument> documents);

        /// <summary>
        /// Cập nhật document
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateDefinition"></param>
        /// <returns></returns>
        bool Update(object id, UpdateDefinition<TDocument> updateDefinition);

        /// <summary>
        /// Cập nhật document async
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateDefinition"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(object id, UpdateDefinition<TDocument> updateDefinition);

        /// <summary>
        /// Replace document
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        bool Replace(TDocument document);

        /// <summary>
        /// Replace document async
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        Task<bool> ReplaceAsync(TDocument document);

        /// <summary>
        /// Replace documents async
        /// </summary>
        /// <param name="documents"></param>
        /// <returns></returns>
        Task<bool> ReplaceManyAsync(IList<TDocument> documents);

        /// <summary>
        /// Upsert document
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        bool Upsert(TDocument document);

        /// <summary>
        /// Upsert document async
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        Task<bool> UpsertAsync(TDocument document);

        /// <summary>
        /// Xóa document
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(object id);

        /// <summary>
        /// Xóa document async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(object id);

        /// <summary>
        /// Lấy document theo điều kiện
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<TDocument> FindAsync(Expression<Func<TDocument, bool>> filter);
    }
}
