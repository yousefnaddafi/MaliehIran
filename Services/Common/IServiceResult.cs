using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.Common
{
    public interface IServiceResult<TData>
    {
        TData Data { get; set; }
        string ErrorMessage { get; set; }
        bool IsSuccess { get; set; }

        IServiceResult<TData> Ok(TData data);
        IServiceResult<TData> Failure(string errorMessage);
    }
    public interface IServiceResult
    {
        string ErrorMessage { get; set; }
        bool IsSuccess { get; set; }

        IServiceResult Ok();
        IServiceResult Failure(string errorMessage);
    }
}
