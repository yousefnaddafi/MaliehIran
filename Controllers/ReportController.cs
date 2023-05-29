using MaliehIran.Models.Enums;
using MaliehIran.Models;
using MaliehIran.Services.ReportServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MaliehIran.Extensions;

namespace MaliehIran.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService reportService;
        private readonly IHttpContextAccessor _accessor;
        public ReportController(IReportService reportService,IHttpContextAccessor accessor)
        {
            this.reportService = reportService;
            _accessor = accessor;
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public long Create(Report model)
        {
            return reportService.Create(model);
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<Report> Update(Report model)
        {
            return await reportService.Update(model);
        }
        [HttpPost]
        public Report Get(long id)
        {
            return reportService.Get(id);
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public object GetAll(int pageNumber, int count, long? shopId, long? userId, ReportType? type, string? searchCommand)
        {
            return reportService.GetAll(pageNumber,count, shopId, userId, type, searchCommand);
        }
        [HttpPost]
        public object GetAllForUser(int pageNumber, int count, long? shopId, ReportType? type, string? searchCommand)
        {
            var userId = _accessor.HttpContext.User.Identity.IsAuthenticated ?
                _accessor.HttpContext.User.Identity.GetUserId() : 0;
            return reportService.GetAll(pageNumber, count, shopId, userId, type, searchCommand);
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<bool> Delete(long id)
        {
            return await reportService.Delete(id);
        }
    }
}
