using MaliehIran.Infrastructure;
using MaliehIran.Models;
using MaliehIran.Models.Enums;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.ReportServices
{
    public class ReportService : IReportService
    {
        private readonly IProjectEFRepository<Report> _reportRepository;
        public ReportService(IProjectEFRepository<Report> reportRepository)
        {
            _reportRepository = reportRepository;
        }
        public long Create(Report model)
        {
            model.CreateDate = DateTime.Now;
            var creation = _reportRepository.Insert(model);
            return creation.ReportId;
        }
        public async Task<Report> Update(Report model)
        {
            return await _reportRepository.Update(model);
        }
        public Report Get(long id)
        {
            var dbReport = _reportRepository.GetQuery().FirstOrDefault(z => z.ReportId == id);
            return dbReport;
        }
        public object GetAll(int pageNumber, int count,long? shopId,long? userId,ReportType? type, string? searchCommand)
        {
            searchCommand = searchCommand ?? "";
            var dbReports = _reportRepository.GetQuery().Where(z => z.Title.Contains(searchCommand)).ToList();
            if(userId != null)
            {
                dbReports = dbReports.Where(z => z.UserId == userId).ToList();
            }
            if(shopId != null)
            {
                dbReports = dbReports.Where(z => z.ShopId == shopId).ToList();
            }
            if(type != null)
            {
                dbReports = dbReports.Where(z=>z.Type == type).ToList();
            }
                        
            var dbCount = dbReports.Count();

            if (pageNumber != 0 && count != 0)
            {
                dbReports = dbReports.OrderByDescending(z => z.CreateDate).Skip((pageNumber - 1) * count).Take(count).ToList();
            }
            var result = new
            {
                Reports = dbReports,
                Count = dbCount
            };
            return result;
        }
        public async Task<bool> Delete(long id)
        {
            try
            {
                var dbReport = _reportRepository.GetQuery().FirstOrDefault(z => z.ReportId == id);
                if (dbReport != null)
                {
                    await _reportRepository.Delete(dbReport);
                    return true;
                }
                else return false;
            }
            catch(Exception ex) { return false; }
        }
        
    }
}
