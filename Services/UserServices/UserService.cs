using MaliehIran.Extensions;
using MaliehIran.Infrastructure;
using MaliehIran.Models;
using MaliehIran.Models.Enums;
using MaliehIran.Models.Urls;
using MaliehIran.Services.Common;
using MaliehIran.Services.CryptographyServices;
using MaliehIran.Services.MediaServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MaliehIran.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IProjectEFRepository<User> userRepository;
        private readonly IProjectEFRepository<UserType> userTypeRepository;
        private readonly IProjectEFRepository<Media> _mediaRepository;
        private readonly IProjectEFRepository<Shop> _shopRepository;
        private readonly IMediaService _mediaService;
        private readonly IHttpContextAccessor _accessor;
        private readonly IEncryptService _encryptService;

        public UserService(IProjectEFRepository<User> userRepository,
            IProjectEFRepository<Media> mediaRepository,
            IHttpContextAccessor accessor,
            IProjectEFRepository<Shop> shopRepository,
            IProjectEFRepository<UserType> userTypeRepository,
            IMediaService mediaService,
            IEncryptService encryptService)
        {
            this.userRepository = userRepository;
            this.userTypeRepository = userTypeRepository;
            _mediaRepository = mediaRepository;
            _mediaService = mediaService;
            _accessor = accessor;
            _encryptService = encryptService;
            _shopRepository = shopRepository;
        }
        public async Task<long> Create(User inputDto)
        {

            Regex regex = new Regex(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*]).{8,}$");
            Match match = regex.Match(inputDto.Password);
            if (match.Success)
            {
                inputDto.Password = _encryptService.Encrypt(inputDto.Password).Data;
            }
            else
            {
                throw new Exception("Password format not correct");
            }
            var creation = userRepository.Insert(inputDto);
            await userRepository.Save();
            return creation.UserId;
        }

        public async Task<IServiceResult<User>> Update(User item)
        {
            var userId = _accessor.HttpContext.User.Identity.IsAuthenticated
                    ? _accessor.HttpContext.User.Identity.GetUserId()
                    : 0;
            var dbUser = userRepository.GetQuery().FirstOrDefault(z => z.UserId == userId);

            var dbLastSelfie = _mediaRepository.GetQuery()
                .FirstOrDefault(z => z.ObjectId == userId && z.Type == MediaTypes.Profile);

            if (item.ProfileImage == "Delete")
            {
                if (dbLastSelfie != null)
                {
                    dbLastSelfie.PictureUrl = "";
                    await _mediaRepository.Update(dbLastSelfie);
                    await _mediaRepository.Delete(dbLastSelfie);
                }
            }
            else if (!string.IsNullOrEmpty(item.ProfileImage))
            {
                var outPut = _mediaService.SaveImage(item.ProfileImage, EntityUrls.ProfileMediaUrl);
                if (outPut.IsSuccess)
                {
                    Media dbMedia = new Media()
                    {
                        IsDeleted = false,
                        ObjectId = userId,
                        PictureUrl = outPut.ImageName,
                        Type = MediaTypes.Profile,
                        MediaId = 0,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    };

                    if (dbLastSelfie != null)
                    {
                        dbLastSelfie.PictureUrl = outPut.ImageName;
                        dbLastSelfie.UpdateDate = DateTime.Now;
                        await _mediaRepository.Update(dbLastSelfie);
                        dbUser.ProfileImage = outPut.ImageName;
                    }
                    else
                    {
                        await _mediaRepository.InsertAsync(dbMedia);
                        dbUser.ProfileImage = outPut.ImageName;
                    }
                }

            }
            dbUser.UpdateDate = DateTime.Now;
            dbUser.Name = string.IsNullOrEmpty(item.Name) ? dbUser.Name : item.Name;
            dbUser.Email = string.IsNullOrEmpty(item.Email) ? dbUser.Email : item.Email;
            dbUser.Mobile = string.IsNullOrEmpty(item.Mobile) ? dbUser.Mobile : item.Mobile;
            dbUser.FamilyName = string.IsNullOrEmpty(item.FamilyName) ? dbUser.FamilyName: item.FamilyName;
            dbUser.UserName= string.IsNullOrEmpty(item.UserName) ? dbUser.UserName: item.UserName;

            if (!string.IsNullOrEmpty(item.Password))
            {
                var dbCurrentFromUser = _encryptService.Encrypt(item.CurrentPassword).Data;
                if (dbCurrentFromUser == dbUser.Password)
                {
                    Regex regex = new Regex(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*]).{8,}$");
                    Match match = regex.Match(item.Password);
                    if (match.Success)
                    {
                        dbUser.Password = _encryptService.Encrypt(item.Password).Data;
                    }
                    else
                    {
                        return new ServiceResult<User>().Failure("Password format not matched");
                    }
                }
                else
                {
                    return new ServiceResult<User>().Failure("Password Is not correct");
                }
            }

            await userRepository.Update(dbUser);
            dbUser.Password = null;
            return new ServiceResult<User>().Ok(dbUser);
        }

        public async Task<object> GetAll(int pageNumber, int count, string searchCommand)
        {
            if (searchCommand == null)
            {
                searchCommand = "";
            }
            List<User> users = new List<User>();
            users = userRepository.GetQuery().Where(z => z.Name.Contains(searchCommand)
            || z.FamilyName.Contains(searchCommand) || z.UserName.Contains(searchCommand)).ToList();
            var usersCount = users.Count();
            if (pageNumber != 0 && count != 0)
            {
                users = users.Skip((pageNumber - 1) * count).Take(count).ToList();
            }
            users.ForEach(z => z.Password = null);
            users.ForEach(z => z.ProfileImage = _mediaRepository.GetQuery().
            FirstOrDefault(x => x.ObjectId == z.UserId && x.Type == MediaTypes.Profile) == null ? ""
            : "media/gallery/Profile/" + _mediaRepository.GetQuery().
            FirstOrDefault(x => x.ObjectId == z.UserId && x.Type == MediaTypes.Profile).PictureUrl);

            users.ForEach(z => z.Shops = new List<Shop>());
            users.ForEach(z => z.Shops = _shopRepository.GetQuery().Where(x => x.UserId == z.UserId).ToList());
            var obj = new
            {
                Users = users,
                UsersCount = usersCount
            };
            return obj;
        }

        public async Task<object> ChangeUserRole(long userId, long userRole)
        {
            var dbUser = userRepository.GetQuery().FirstOrDefault(z => z.UserId == userId);
            dbUser.Type = userRole;
            dbUser.UpdateDate = DateTime.Now;
            await userRepository.Update(dbUser);
            return dbUser.UserId;
        }

        //public async Task<object> GetAllSP()
        //{
        //    var cmd = "select M.PictureUrl, U.UserId, U.Name, U.FamilyName, U.Email, U.Mobile, U.Type, U.Status, U.FormalId, US.Bio, US.BirthDate, US.Gender, US.UserTag, US.Flag, US.IsPrivate from[User].Users as U left join[User].UserSettings  as US on U.UserId = US.UserId left join Social.Medias as M on (M.ObjectId = U.UserId and M.Type = 2)";
        //    var res = await userRepository.DapperSqlQuery(cmd);
        //    var Des = JsonSerializer.Serialize<object>(res);
        //    //var sdf = JsonSerializer.Deserialize<UserAndUserSetting>(Des);
        //    return Des;
        //}

        public async Task<bool> Delete(long id)
        {
            try
            {
                var dbUser = userRepository.GetQuery().FirstOrDefault(z => z.UserId == id && z.IsDeleted == false);
                if (dbUser != null)
                {
                    await userRepository.Delete(dbUser);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        public User GetById()
        {
            var userId = _accessor.HttpContext.User.Identity.IsAuthenticated
                    ? _accessor.HttpContext.User.Identity.GetUserId()
                   : 0;

            var dbUser = userRepository.GetQuery().FirstOrDefault(z => z.UserId == userId);

            var dbProfile = _mediaRepository.GetQuery().FirstOrDefault(z => z.ObjectId == userId && z.Type == MediaTypes.Profile);
            if (dbProfile != null)
            {
                dbUser.ProfileImage = "media/gallery/Profile/" + dbProfile.PictureUrl;
            }

            dbUser.Role = userTypeRepository.GetQuery().FirstOrDefault(z => z.UserTypeId == dbUser.Type).Name;
            dbUser.Shops = new List<Shop>();
            dbUser.Shops = _shopRepository.GetQuery().Where(z => z.UserId == userId).ToList();
            dbUser.Password = null;
            return dbUser;
        }

        public User Get(long id)
        {
            var dbUser = userRepository.GetQuery().FirstOrDefault(z => z.UserId == id);

            var dbProfile = _mediaRepository.GetQuery().FirstOrDefault(z => z.ObjectId == id && z.Type == MediaTypes.Profile);
            if (dbProfile != null)
            {
                dbUser.ProfileImage = "media/gallery/Profile/" + dbProfile.PictureUrl;
            }
            dbUser.Shops =new List<Shop>();
            dbUser.Shops = _shopRepository.GetQuery().Where(z=>z.UserId == id).ToList();
            dbUser.Password = null;
            return dbUser;
        }
        //public List<User> SearchByName(string name)
        //{
        //    var userId = _accessor.HttpContext.User.Identity.IsAuthenticated
        //            ? _accessor.HttpContext.User.Identity.GetUserId()
        //            : 0;

        //    var cmd = $"select top 10 * from [User].Users where (Name like '%{name}%' or FamilyName like '%{name}%') And UserId !={userId}";
        //    var DeSerializeObj = userRepository.DapperSqlQuery(cmd);
        //    var objSer = JsonSerializer.Serialize<object>(DeSerializeObj.Result);
        //    var dbUsers = JsonSerializer.Deserialize<List<User>>(objSer);
        //    foreach (var user in dbUsers)
        //    {
        //        user.Password = null;
        //        var selfie = _mediaRepository.GetQuery().FirstOrDefault(z => z.ObjectId == user.UserId && z.Type == MediaTypes.Profile);
        //        user.SelfieFileData = selfie == null ? "" : "media/gallery/Profile/" + selfie.PictureUrl;
        //        user.UserSetting = userSettingRepository.GetQuery().FirstOrDefault(z => z.UserId == user.UserId);
        //    }
        //    return dbUsers;
        //}
    }
}
