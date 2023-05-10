using MaliehIran.Models.Enums;
using MaliehIran.Models.Urls;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MaliehIran.Infrastructure;
using MaliehIran.Models;
using MaliehIran.Services.MediaServices;
using System.Linq;

namespace MaliehIran.Services.GroupServices
{
    public class GroupService : IGroupService
    {
        private readonly IProjectEFRepository<Group> groupRepository;
        private readonly IProjectEFRepository<UserGroup> userGroupRepository;
        private readonly IMediaService _mediaService;
        private readonly IProjectEFRepository<Media> _mediaRepository;
        public GroupService(IProjectEFRepository<Group> groupRepository
            , IProjectEFRepository<UserGroup> userGroupRepository
            , IProjectEFRepository<Media> mediaRepository
            , IMediaService mediaService)
        {
            this.groupRepository = groupRepository;
            this.userGroupRepository = userGroupRepository;
            _mediaRepository = mediaRepository;
            _mediaService = mediaService;
        }
        public async Task Create(Group inputDto)
        {
            var dbGroup = groupRepository.Insert(inputDto);
            foreach (var userId in inputDto.Members)
            {
                UserGroup userGroup = new UserGroup()
                {
                    CreateDate = DateTime.Now,
                    IsDeleted = false,
                    GroupId = dbGroup.GroupId,
                    UserGroupId = 0,
                    UserId = userId
                };
                userGroupRepository.Insert(userGroup);
            }
            if (!string.IsNullOrEmpty(inputDto.FileData))
            {
                var outPut = _mediaService.SaveImage(inputDto.FileData, EntityUrls.Group);
                Media dbMedia = new Media()
                {
                    IsDeleted = false,
                    ObjectId = dbGroup.GroupId,
                    PictureUrl = outPut.ImageName,
                    Type = MediaTypes.Group,
                    MediaId = 0,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Priority = 1
                };
                await _mediaRepository.InsertAsync(dbMedia);
            }
        }

        public async Task<Group> Update(Group item)
        {
            var dbLastGroup = _mediaRepository.GetQuery()
                .FirstOrDefault(z => z.ObjectId == item.GroupId && z.Type == MediaTypes.Group);

            if (item.FileData == "Delete")
            {
                if (dbLastGroup != null)
                {
                    dbLastGroup.PictureUrl = "";
                    await _mediaRepository.Update(dbLastGroup);
                    await _mediaRepository.Delete(dbLastGroup);
                }

            }
            else if (!string.IsNullOrEmpty(item.FileData))
            {
                var outPut = _mediaService.SaveImage(item.FileData, EntityUrls.Group);
                if (outPut.IsSuccess)
                {
                    Media dbMedia = new Media()
                    {
                        IsDeleted = false,
                        ObjectId = item.GroupId,
                        PictureUrl = outPut.ImageName,
                        Type = MediaTypes.Group,
                        MediaId = 0,
                        CreateDate = DateTime.Now,
                        UpdateDate= DateTime.Now,
                        Priority = 1
                    };

                    if (dbLastGroup != null)
                    {
                        dbLastGroup.PictureUrl = outPut.ImageName;
                        await _mediaRepository.Update(dbLastGroup);
                        item.FileData = outPut.ImageName;
                    }
                    else
                    {
                        await _mediaRepository.InsertAsync(dbMedia);
                        item.FileData = outPut.ImageName;
                    }
                }
            }
            await groupRepository.Update(item);
            return item;
        }

        public async Task<List<Group>> GetAll()
        {
            List<Group> groups = new List<Group>();
            groups = groupRepository.GetQuery().ToList();
            groups.ForEach(z => z.FileData = _mediaRepository.GetQuery().FirstOrDefault(x => x.ObjectId == z.GroupId && x.Type == MediaTypes.Group) == null ?
             "" : "media/gallery/Group/" + _mediaRepository.GetQuery().FirstOrDefault(x => x.ObjectId == z.GroupId && x.Type == MediaTypes.Group).PictureUrl);
            return groups;
        }

        public async Task Delete(long id)
        {
            var group = groupRepository.GetQuery().FirstOrDefault(z => z.GroupId == id);
            await groupRepository.Delete(group);
        }

        public async Task<Group> Get(long id)
        {
            var group = groupRepository.GetQuery().FirstOrDefault(z => z.GroupId == id);
            group.FileData = _mediaRepository.GetQuery().FirstOrDefault(x => x.ObjectId == group.GroupId && x.Type == MediaTypes.Group) == null ?
             "" : "media/gallery/Group/" + _mediaRepository.GetQuery().FirstOrDefault(x => x.ObjectId == group.GroupId && x.Type == MediaTypes.Group).PictureUrl;
            return group;
        }
    }
}
