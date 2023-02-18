//This file was auto-genearted by the Clean.Generator.//
//Any modifications to this file will be overwritten on the next run of the generator.//

using Clean.Domain.Common.Interfaces;
using Clean.Domain.Common.Models;
using Clean.Domain.ContextNameContext.Entities;
using Clean.Domain.ContextNameContext.Models.LookupRequests;

namespace Clean.Domain.ContextNameContext.Services
{
    public interface ITableNameService
    {
        ResultResponse<TableName> Create(TableName entity);
        IQueryable<TableName> GetQueryable(TableNameLookupRequest lookupRequest);
        IEnumerable<TableName> GetEnumerable(TableNameLookupRequest lookupRequest);
        FetchResponse<TableName> GetFetchResponse(TableNameLookupRequest lookupRequest);
        ResultResponse<TableName> Update(TableName entity);
        ResultResponse<TableName> Delete(object?[]? primaryKey);
    }

    public class TableNameService : ITableNameService
    {
        private readonly IRepository<TableName> _TableNameRepository;

        public TableNameService(IRepository<TableName> TableNameRepository)
        {
            _TableNameRepository = TableNameRepository;
        }

        public ResultResponse<TableName> Create(TableName entity)
        {
            if (entity == null)
                return new ResultResponse<TableName>() { Errors = new() { "TableName cannot be null." } };

            ResultResponse<TableName> result = entity.Validate();

            if (result.Successful)
                result = _TableNameRepository.Create(entity);

            return result;
        }

        public IQueryable<TableName> GetQueryable(TableNameLookupRequest lookupRequest)
        {
            var predicate = lookupRequest.BuildPredicate<TableName>();
            var orderBy = lookupRequest.GetOrderBy<TableName>();
            return _TableNameRepository.GetQueryable(predicate, orderBy);
        }

        public IEnumerable<TableName> GetEnumerable(TableNameLookupRequest lookupRequest)
        {
            var predicate = lookupRequest.BuildPredicate<TableName>();
            var orderBy = lookupRequest.GetOrderBy<TableName>();
            return _TableNameRepository.GetEnumerable(predicate, lookupRequest.Page, lookupRequest.PageLimit, orderBy);
        }

        public FetchResponse<TableName> GetFetchResponse(TableNameLookupRequest lookupRequest)
        {
            var predicate = lookupRequest.BuildPredicate<TableName>();
            var orderBy = lookupRequest.GetOrderBy<TableName>();
            return _TableNameRepository.GetFetchResponse(predicate, lookupRequest.Page, lookupRequest.PageLimit, orderBy);
        }

        public ResultResponse<TableName> Update(TableName entity)
        {
            if (entity == null)
                return new ResultResponse<TableName>() { Errors = new() { "TableName cannot be null." } };

            ResultResponse<TableName> result = entity.Validate(true);

            if (result.Successful)
                result = _TableNameRepository.Update(entity);

            return result;
        }

        public ResultResponse<TableName> Delete(object?[]? primaryKey)
        {
            return _TableNameRepository.Delete(primaryKey);
        }
    }
}