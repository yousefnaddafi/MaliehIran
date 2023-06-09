﻿using MaliehIran.Infrastructure;
using MaliehIran.Models;
using MaliehIran.Services.AccountServices;
using MaliehIran.Services.CryptographyServices;
using MaliehIran.Services.EmailServices;
using MaliehIran.Services.GroupServices;
using MaliehIran.Services.JWTServices;
using MaliehIran.Services.MediaServices;
using MaliehIran.Services.MessageRecipientServices;
using MaliehIran.Services.MessageServices;
using MaliehIran.Services.ReportServices;
using MaliehIran.Services.ShopServices;
using MaliehIran.Services.SMSServices;
using MaliehIran.Services.UserGroupServices;
using MaliehIran.Services.UserServices;
using MaliehIran.Services.UserTypeServices;
using MaliehIran.Services.UtilityServices;
using MaliehIran.Settings;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Extensions
{
    public static class DependencyExtensions
    {
        public static void AddDependency(this IServiceCollection services)
        {
            AddRepositories(services);
            AddServices(services);
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IProjectEFRepository<User>, ProjectEFRepository<User>>();
            services.AddScoped<IProjectEFRepository<UserType>, ProjectEFRepository<UserType>>();
            services.AddScoped<IProjectEFRepository<Media>, ProjectEFRepository<Media>>();
            services.AddScoped<IProjectEFRepository<Message>, ProjectEFRepository<Message>>();
            services.AddScoped<IProjectEFRepository<MessageRecipient>, ProjectEFRepository<MessageRecipient>>();
            services.AddScoped<IProjectEFRepository<Group>, ProjectEFRepository<Group>>();
            services.AddScoped<IProjectEFRepository<UserGroup>, ProjectEFRepository<UserGroup>>();
            services.AddScoped<IProjectEFRepository<Report>, ProjectEFRepository<Report>>();
            services.AddScoped<IProjectEFRepository<Shop>, ProjectEFRepository<Shop>>();
            services.AddScoped<IProjectEFRepository<Utility>, ProjectEFRepository<Utility>>();
            
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IMediaService, MediaService>();
            services.AddTransient<IUserTypeService, UserTypeService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IMessageService, MessageService>();
            services.AddTransient<IMessageRecipientService, MessageRecipientService>();
            services.AddTransient<IGroupService, GroupService>();
            services.AddTransient<IUserGroupService, UserGroupService>();
            services.AddTransient<IShopService, ShopService>();
            services.AddTransient<IReportService, ReportService>();
            services.AddTransient<IUtilityService, UtilityService>();
            services.AddTransient<ISMSService, SMSService>();

            services.AddTransient(typeof(IGenerateJwtService), typeof(GenerateJwtService));
            services.AddTransient(typeof(IConfirmationCodeSetting), typeof(ConfirmationCodeSetting));
            services.AddTransient(typeof(IEncryptService), typeof(EncryptService));
            services.AddTransient(typeof(IDecryptService), typeof(DecryptService));
        }
    }
}
