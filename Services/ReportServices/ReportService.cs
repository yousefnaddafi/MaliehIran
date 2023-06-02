using MaliehIran.Infrastructure;
using MaliehIran.Models;
using MaliehIran.Models.Enums;
using MaliehIran.Models.Urls;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.ReportServices
{
    public class ReportService : IReportService
    {
        private readonly IProjectEFRepository<Report> _reportRepository;
        private readonly IProjectEFRepository<Media> _mediaRepository;
        private readonly IProjectEFRepository<Shop> _shopRepository;
        public ReportService(IProjectEFRepository<Report> reportRepository,
            IProjectEFRepository<Media> mediaRepository,IProjectEFRepository<Shop> shopRepository)
        {
            _mediaRepository = mediaRepository;
            _reportRepository = reportRepository;
            _shopRepository = shopRepository;
        }
        public async Task<long> Create(Report model,IFormFile file)
        {
            model.CreateDate = DateTime.Now;
            var creation = _reportRepository.Insert(model);
            if (creation != null)
            {
                await UploadFile(file, creation.ReportId);
            }
            
            return creation.ReportId;
        }
        public async Task<Report> Update(Report model)
        {
            return await _reportRepository.Update(model);
        }
        public async  Task<Report> Get(long id)
        {
            var dbReport = _reportRepository.GetQuery().FirstOrDefault(z => z.ReportId == id);
            if (dbReport != null)
            {
                dbReport.ShopName = _shopRepository.GetQuery().FirstOrDefault(x=>x.ShopId == dbReport.ShopId)?.ShopName;

                var dbMedia = _mediaRepository.GetQuery().FirstOrDefault(z => z.ObjectId == dbReport.ReportId && (z.Type == MediaTypes.Podcast
                || z.Type == MediaTypes.Video || z.Type == MediaTypes.Picture || z.Type == MediaTypes.PDF));
                if (dbMedia != null)
                {
                    var ContentType = dbMedia.PictureUrl.Split('.').Last();
                    var directoryDTO = await GetPath(ContentType);
                    if (directoryDTO != null)
                    {
                        dbReport.MediaInfo = new MediaInfo
                        {
                            FilePath = directoryDTO.Path.Replace("wwwroot/", "") + "/" + dbMedia.PictureUrl,
                            Type = directoryDTO.Type,
                        };
                    }
                }
            }
            
            return dbReport;
        }
        public async Task<object> GetAll(int pageNumber, int count,long? shopId,long? userId,ReportType? type, string? searchCommand)
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
            foreach(var dbReport in dbReports)
            {
                dbReport.ShopName = _shopRepository.GetQuery().FirstOrDefault(x => x.ShopId == dbReport.ShopId)?.ShopName;

                var dbMedia = _mediaRepository.GetQuery().FirstOrDefault(z => z.ObjectId == dbReport.ReportId && (z.Type == MediaTypes.Podcast
                || z.Type == MediaTypes.Video || z.Type == MediaTypes.Picture || z.Type == MediaTypes.PDF));
                if (dbMedia != null)
                {
                    var ContentType = dbMedia.PictureUrl.Split('.').Last();
                    var directoryDTO = await GetPath(ContentType);
                    if (directoryDTO != null)
                    {
                        dbReport.MediaInfo = new MediaInfo
                        {
                            FilePath = directoryDTO.Path.Replace("wwwroot/","") + "/"+ dbMedia.PictureUrl,
                            Type = directoryDTO.Type,
                        };
                    }
                }
                
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
        public async Task<DirectoryDTO> GetPath(string contentType)
        {
            MediaTypes mediaTypes = new MediaTypes();
            string directoryPath = "";

            if (contentType.ToLower().Contains("jpg") || contentType.ToLower().Contains("jpeg") || contentType.ToLower().Contains("png"))
            {
                directoryPath = EntityUrls.Picture;
                mediaTypes = MediaTypes.Picture;
            }
            if (contentType.ToLower().Contains("mp4") || contentType.ToLower().Contains("mov"))
            {
                directoryPath = EntityUrls.Video;
                mediaTypes = MediaTypes.Video;
            }
            if (contentType.ToLower().Contains("pdf") || contentType.ToLower().Contains("docs") || contentType.ToLower().Contains("txt"))
            {
                directoryPath = EntityUrls.PDF;
                mediaTypes = MediaTypes.PDF;
            }
            if (contentType.ToLower().Contains("mp3") || (contentType.ToLower().Contains("wav")) || (contentType.ToLower().Contains("mpeg")) || (contentType.ToLower().Contains("m4a")))
            {
                directoryPath = EntityUrls.Podcast;
                mediaTypes = MediaTypes.Podcast;
            }
            var result = new DirectoryDTO
            {
                Path = directoryPath,
                Type = mediaTypes
            };
            return result;
        }

        public async Task<object> UploadFile(IFormFile file,long reportId)
        {
            var ContentType = file.FileName.Split('.').Last();
            var directoryDTO = await GetPath(ContentType);
            var directoryPath = directoryDTO.Path;
            MediaTypes mediaTypes = directoryDTO.Type;

            if (file.Length > 0)
            {

                string fileName = System.Guid.NewGuid() + $".{ContentType}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), directoryPath, fileName);
                
                //var filePath = "wwwroot/Media/File/Video/";
                
                //var lastPriority = _mediaRepository.GetQuery().Where(z => z.ObjectId == productId).OrderByDescending(c => c.Priority).Select(x => x.Priority).First();
                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
                _mediaRepository.Insert(new Media()
                {
                    IsDeleted = false,
                    UpdateDate = DateTime.Now,
                    MediaId = 0,
                    ObjectId = reportId,
                    PictureUrl = fileName,
                    Priority =  1,
                    Type = mediaTypes,
                    Title = fileName,
                    CreateDate = DateTime.Now
                });
            }

            var obj = new
            {
                Name = file.FileName,
                Type = file.ContentType
            };
            return obj;
        }
        public class DirectoryDTO
        {
            public string Path { get; set; }
            public MediaTypes Type { get; set; }
        }
    }
}
