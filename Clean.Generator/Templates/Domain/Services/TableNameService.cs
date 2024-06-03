//This file was auto-genearted by the Clean.Generator.//
//Any modifications to this file will be overwritten on the next run of the generator.//

using Clean.Domain.Common.Interfaces;
using Clean.Domain.Common.Models;
using Clean.Domain.ContextNameContext.Models.DTOs;
using Clean.Domain.ContextNameContext.Models.LookupRequests;
using Clean.Infrastructure.ContextNameContext;
using Clean.Infrastructure.ContextNameContext.Entities;

namespace Clean.Domain.ContextNameContext.Services
{
    public interface ITableNameService
    {
        ResultResponse<TableNameDTO> Create(TableNameDTO dto);
        IEnumerable<TableNameDTO> GetEnumerable(TableNameLookupRequest lookupRequest);
        FetchResponse<TableNameDTO> GetFetchResponse(TableNameLookupRequest lookupRequest);
        ResultResponse<TableNameDTO> Update(TableNameDTO dto);
        ResultResponse<TableNameDTO> Delete(object?[]? primaryKey);
    }

    public class TableNameService : ITableNameService
    {
        private readonly IRepository<ContextNameDbContext> _TableNameRepository;

        public TableNameService(IRepository<ContextNameDbContext> TableNameRepository)
        {
            _TableNameRepository = TableNameRepository;
        }

        public ResultResponse<TableNameDTO> Create(TableNameDTO dto)
        {
            if (dto == null)
                return new ResultResponse<TableNameDTO>() { Errors = new() { "TableName cannot be null." } };

            ResultResponse<TableNameDTO> result = dto.Validate();

            if (result.Successful)
                result = _TableNameRepository.Create<TableNameDTO, TableName>(dto);

            return result;
        }

        public IEnumerable<TableNameDTO> GetEnumerable(TableNameLookupRequest lookupRequest) => 
            _TableNameRepository.GetEnumerable<TableNameDTO, TableName>(lookupRequest.BuildPredicate<TableName>(), lookupRequest.Page, lookupRequest.PageLimit, lookupRequest.GetOrderBy<TableName>());

        public FetchResponse<TableNameDTO> GetFetchResponse(TableNameLookupRequest lookupRequest) => 
            _TableNameRepository.GetFetchResponse<TableNameDTO, TableName>(lookupRequest.BuildPredicate<TableName>(), lookupRequest.Page, lookupRequest.PageLimit, lookupRequest.GetOrderBy<TableName>());

        public ResultResponse<TableNameDTO> Update(TableNameDTO dto)
        {
            if (dto == null)
                return new ResultResponse<TableNameDTO>() { Errors = new() { "TableName cannot be null." } };

            ResultResponse<TableNameDTO> result = dto.Validate(true);

            if (result.Successful)
                result = _TableNameRepository.Update<TableNameDTO, TableName>(dto);

            return result;
        }

        public ResultResponse<TableNameDTO> Delete(object?[]? primaryKey) => 
            _TableNameRepository.Delete<TableNameDTO, TableName>(primaryKey);
    }
}