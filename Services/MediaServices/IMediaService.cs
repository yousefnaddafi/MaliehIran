using MaliehIran.Models;
using MaliehIran.Models.Enums;
using MaliehIran.Services.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.MediaServices
{
    public interface IMediaService
    {
        Task<IServiceResult<Dictionary<string, string>>> Upload(List<IFormFile> files, int objectId, MediaTypes type);
        IServiceResult<int> Create(Media media);
        Task<IServiceResult<List<Media>>> GetByObjectId(int objectId, MediaTypes type);
        IServiceResult DeleteByMediaId(int mediaId);
        Task<IServiceResult> DeleteByObjectId(int objectId, MediaTypes type);
        OutPutSaveImage SaveImage(string base64, string path);
    }
}
