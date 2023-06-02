using MaliehIran.Models.Enums;
using MaliehIran.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MaliehIran.Services.ReportServices
{
    public interface IReportService
    {
        Task<long> Create(Report model, IFormFile file);
        Task<Report> Update(Report model);
        Task<Report> Get(long id);
        Task<object> GetAll(int pageNumber, int count, long? shopId, long? userId, ReportType? type, string? searchCommand);
        Task<bool> Delete(long id);
    }
}
