using Clean.Domain.Common.Enums;

namespace Clean.Domain.Common.Model
{
    public class ResultResponse
    {
        public List<string> Errors { get; set; } = new List<string>();
        public Exception? Exception { get; set; }

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