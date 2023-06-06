using MaliehIran.Models.Enums;
using MaliehIran.Models;
using MaliehIran.Services.ReportServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MaliehIran.Extensions;
using System;

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
        public async Task<long> Create(IFormFile file,long shopId, long userId,string description,string title,ReportType type,bool? sendSMS,string? link)
        {
            var reportModel = new Report()
            {
                ShopId = shopId,
                CreateDate = DateTime.Now,
                Description = description,
                IsDeleted = false,
                ReportId = 0,
                Title = title,
                Type = type,
                UserId = userId,
                Link = link
            };
            return await reportService.Create(reportModel,sendSMS,file);
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<Report> Update(Report model)
        {
            return await reportService.Update(model);
        }
        [HttpPost]
        public async Task<Report> Get(long id)
        {
            return await reportService.Get(id);
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<object> GetAll(int pageNumber, int count, long? shopId, long? userId, ReportType? type, string? searchCommand)
        {
            return await reportService.GetAll(pageNumber,count, shopId, userId, type, searchCommand);
        }
        [HttpPost]
        public async Task<object> GetAllForUser(int pageNumber, int count, long? shopId, ReportType? type, string? searchCommand)
        {
            var userId = _accessor.HttpContext.User.Identity.IsAuthenticated ?
                _accessor.HttpContext.User.Identity.GetUserId() : 0;
            return await reportService.GetAll(pageNumber, count, shopId, userId, type, searchCommand);
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<bool> Delete(long id)
        {
            return await reportService.Delete(id);
        }
    }
}
