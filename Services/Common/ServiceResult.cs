using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.Common
{
    public class ServiceResult<TData> : IServiceResult<TData>
    {
        public TData Data { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }

        public IServiceResult<TData> Failure(string errorMessage)
        {
            ErrorMessage = errorMessage;
            IsSuccess = false;
            return this;
        }

        public IServiceResult<TData> Ok(TData data)
        {
            Data = data;
            IsSuccess = true;
            return this;
        }
    }
    public class ServiceResult : IServiceResult
    {
        public string ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }

        public IServiceResult Failure(string errorMessage)
        {
            ErrorMessage = errorMessage;
            IsSuccess = false;
            return this;
        }

        public IServiceResult Ok()
        {
            IsSuccess = true;
            return this;
        }
    }
}
