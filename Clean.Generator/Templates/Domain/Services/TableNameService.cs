using Clean.Domain.Common.Interfaces;
using Clean.Domain.Common.Model;
using Clean.Domain.ContextNameContext.Entities;
using Clean.Domain.ContextNameContext.Models.LookupRequests;

namespace Clean.Domain.ContextNameContext.Services
{
    public interface ITableNameService
    {
        ResultResponse Create(TableName entity);
        IEnumerable<TableName> Get(TableNameLookupRequest lookupRequest);
        ResultResponse Update(TableName entity);
        ResultResponse Delete(TableName entity);
    }

    public class TableNameService : ITableNameService
    {
        private readonly IRepository<TableName> _TableNameRepository;

        public TableNameService(IRepository<TableName> TableNameRepository)
        {
            _TableNameRepository = TableNameRepository;
        }

        public ResultResponse Create(TableName entity)
        {
            ResultResponse result = entity.Validate();

            if (result.Successful)
                result = _TableNameRepository.Create(entity);

            return result;
        }

        public IEnumerable<TableName> Get(TableNameLookupRequest lookupRequest)
        {
            var predicate = lookupRequest.BuildPreciate<TableName>();
            return _TableNameRepository.Where(predicate);
        }

        public ResultResponse Update(TableName entity)
        {
            ResultResponse result = entity.Validate();

            if (result.Successful)
                _TableNameRepository.Update(entity);

            return result;
        }

        public ResultResponse Delete(TableName entity)
        {
            return _TableNameRepository.Delete(entity);
        }
    }
}