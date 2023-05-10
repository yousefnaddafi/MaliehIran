using MaliehIran.Infrastructure;
using MaliehIran.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.UserTypeServices
{
    public class UserTypeService : IUserTypeService
    {
        private readonly IProjectEFRepository<UserType> userTypeRepository;

        public UserTypeService(IProjectEFRepository<UserType> userTypeRepository)
        {
            this.userTypeRepository = userTypeRepository;
        }
        public async Task<long> Create(UserType inputDto)
        {
            var creation = userTypeRepository.Insert(inputDto);
            return creation.UserTypeId;
        }

        public async Task<UserType> Update(UserType item)
        {
            await userTypeRepository.Update(item);
            return item;
        }

        public async Task<List<UserType>> GetAll()
        {
            List<UserType> dbUserTypes = new List<UserType>();
            dbUserTypes = userTypeRepository.GetQuery().ToList();
            return dbUserTypes;
        }

        public async Task<bool> Delete(long id)
        {
            try
            {
                var dbUserType = userTypeRepository.GetQuery().FirstOrDefault(z => z.UserTypeId == id);
                await userTypeRepository.Delete(dbUserType);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
            
        }

        public async Task<UserType> Get(long id)
        {
            var dbUserType = userTypeRepository.GetQuery().FirstOrDefault(z => z.UserTypeId == id);
            return dbUserType;
        }
        public List<UserType> GetByCommand(string sql)
        {
            var dbUserType = userTypeRepository.SqlExecute(sql).ToList();
            return dbUserType;
        }
        public long UpdateByCmd(string name, long id)
        {
            string UpdateCmd = $"Update [User].[Types] Set Name = '{name}' Where UserTypeId={id}";
            try
            {
                var result = userTypeRepository.SqlExecute(UpdateCmd);
                return id;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
