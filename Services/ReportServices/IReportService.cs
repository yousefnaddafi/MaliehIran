using MaliehIran.Models.Enums;
using MaliehIran.Models;
using System.Threading.Tasks;

namespace MaliehIran.Services.ReportServices
{
    public interface IReportService
    {
        long Create(Report model);
        Task<Report> Update(Report model);
        Report Get(long id);
        object GetAll(int pageNumber, int count, long? shopId, long? userId, ReportType? type, string? searchCommand);
        Task<bool> Delete(long id);
    }
}
