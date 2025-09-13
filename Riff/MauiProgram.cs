using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Riff.ConnectionRepository;
using Riff.Managers;
using Riff.Repositories;
using Riff.Repositories.Database;
using Riff.Repositories.Interfaces;
using Riff.Services;
using Riff.Services.Interfaces;
using Riff.ViewModels;

namespace Riff
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            string hostUrl = "https://api.byxesh-dev.ufeudor.pw/";


            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<LoginPage>();
            


            builder.Services.AddSingleton<GatewayConnectionRepository>(new GatewayConnectionRepository(hostUrl, "api/gateway"));
            builder.Services.AddSingleton<IGatewayConnectionRepository>(p =>
                p.GetRequiredService<GatewayConnectionRepository>() as IGatewayConnectionRepository);
            builder.Services.AddSingleton<IJwtDependentService>(p =>
                p.GetRequiredService<GatewayConnectionRepository>() as IJwtDependentService);

            builder.Services.AddSingleton<SqliteContext>();
            builder.Services.AddSingleton<IMessageCollectionRepository, MessageCollectionRepository>();
            builder.Services.AddSingleton<IChatRepository, ChatRepository>();

            builder.Services.AddSingleton<IJwtManager, JwtManager>();
            builder.Services.AddSingleton<IAccountService, AccountService>();
            builder.Services.AddSingleton<IChatsService, ChatsService>();
            builder.Services.AddSingleton<IMessageService, MessageService>();

            builder.Services.AddTransient<RegisterPageViewModel>();
            builder.Services.AddTransient<LoginPageViewModel>();
            builder.Services.AddTransient<ChatsPageViewModel>();
            builder.Services.AddTransient<ChatPageViewModel>();



            var app = builder.Build();

            _ = Task.Run(async () =>
            {
                var dbContext = app.Services.GetRequiredService<SqliteContext>();
                await dbContext.ForceInitializeAsync();
                
                var repo = app.Services.GetRequiredService<IGatewayConnectionRepository>();
                await repo.InitializeConnection();
            });

            return app;
        }
    }
}
