using System;
using System.Globalization;
using Hangfire;
using Library.Security;
using LibraryData;
using LibraryData.Models.Account;
using LibraryService;
using LibraryService.EmailConfiguration;
using LibraryService.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Library
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteRolePolicy",
                policy => policy.RequireClaim("Delete Role", "true"));

            });

            services.AddSingleton<IEmailConfiguration>(Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<ILibraryAssetService, LibraryAssetService>();
            services.AddScoped<ICheckout, CheckoutService>();
            services.AddScoped<IPatron, PatronService>();
            services.AddScoped<ILibraryBranch, LibraryBranchService>();

            services.AddDbContext<LibraryContext>(options
                => options.UseSqlServer(Configuration.GetConnectionString("LibraryConnection")));

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
                .AddEntityFrameworkStores<LibraryContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<CustomEmailConfirmationTokenProvider<User>>("CustomEmailConfirmation");

            services.AddHangfire(options =>
            {
                options.UseSqlServerStorage(Configuration.GetConnectionString("LibraryConnection"));
            });

            services.AddScoped<IWaitingHoldsProcessingTask, WaitingHoldsProcessingTask>();

            services.Configure<DataProtectionTokenProviderOptions>(x =>
            x.TokenLifespan = TimeSpan.FromHours(5));

            services.Configure<CustomEmailConfirmationTokenProviderOptions>(x =>
            x.TokenLifespan = TimeSpan.FromDays(3));

            services.AddSingleton<DataProtectionPurposeStrings>();

            services.AddAutoMapper(typeof(Startup));
            services.AddMediatR(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard();
            app.UseHangfireServer();
            RecurringJob.AddOrUpdate<WaitingHoldsProcessingTask>(x => x.Execute(), Cron.Hourly);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            //Culture specific problems
            var currentCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            currentCulture.NumberFormat.NumberDecimalSeparator = ".";
            CultureInfo.DefaultThreadCurrentCulture = currentCulture;
        }
    }
}
