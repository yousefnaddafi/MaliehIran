using MaliehIran.Infrastructure;
using MaliehIran.Models;
using MaliehIran.Services.AccountServices;
using MaliehIran.Services.CronServices;
using MaliehIran.Services.CryptographyServices;
using MaliehIran.Services.CoinexServices;
using MaliehIran.Services.EmailServices;
using MaliehIran.Services.JWTServices;
using MaliehIran.Services.KucoinServices;
using MaliehIran.Services.MediaServices;
using MaliehIran.Services.OrderServices;
using MaliehIran.Services.UserServices;
using MaliehIran.Services.UserTypeServices;
using MaliehIran.Settings;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaliehIran.Services.CryptoAccountServices;

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
            services.AddScoped<IProjectEFRepository<Market>, ProjectEFRepository<Market>>();
            services.AddScoped<IProjectEFRepository<Cron>, ProjectEFRepository<Cron>>();
            services.AddScoped<IProjectEFRepository<Order>, ProjectEFRepository<Order>>();
            services.AddScoped<IProjectEFRepository<CryptoAccount>, ProjectEFRepository<CryptoAccount>>();
            
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IMediaService, MediaService>();
            services.AddTransient<IUserTypeService, UserTypeService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<ICoinexService, CoinexService>();
            //services.AddTransient<IKucoinClientSpotApiAccount, KucoinClientSpotApiAccount>();
            services.AddTransient<IKuCoinService, KuCoinService>();
            //services.AddTransient<IKucoinClientSpotApi, KucoinClientSpotApi>();

            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<ICronService, CronService>();
            services.AddTransient<ICryptoAccountService, CryptoAccountService>();


            services.AddTransient(typeof(IGenerateJwtService), typeof(GenerateJwtService));
            services.AddTransient(typeof(IConfirmationCodeSetting), typeof(ConfirmationCodeSetting));
            services.AddTransient(typeof(IEncryptService), typeof(EncryptService));
            services.AddTransient(typeof(IDecryptService), typeof(DecryptService));
        }
    }
}
