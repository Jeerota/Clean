using Clean.Domain.Common.Enums;

namespace Clean.Domain.Common.Models
{
    public class ResultResponse<T> where T : class
    {
        public List<string> Errors { get; set; } = new List<string>();
        public Exception? Exception { get; set; }
        public T? Entity { get; set; }

        public ResultType Result
        {
            get
            {
                return !Errors.Any() && Exception == null
                    ? ResultType.Success
                    : ResultType.Error;
            }
        }

        public bool Successful
        {
            get
            {
                return Result == ResultType.Success;
            }
        }

        public ResultResponse() { }
    }
}