using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MyCity.API.Services.SMS;
using MyCity.DataModel;
using MyCity.DataModel.AppModels;
using MyCity.DataModel.ToranjModels;

namespace MyCity.API {
	public class Startup {
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration) {
			this.Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services) {
			services.AddDbContext<AppDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("SystemBase")));

			services.AddIdentity<User, Role>(options => {
				options.User.RequireUniqueEmail = false;
				options.SignIn.RequireConfirmedAccount = false;
			}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

			services.AddControllersWithViews();

			services.AddScoped<ISmsService, SmsService>();
			services.AddScoped<IMyDataService, MyDataService>();
			services.AddScoped<IToranjServices, ToranjServices>();


			services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
			opt => {
				opt.LoginPath = "/Auth/Account/Login";
				opt.AccessDeniedPath = "/Auth/Account/Login";
			});

			services.Configure<IdentityOptions>(options => {
				options.Password.RequireDigit = false;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireUppercase = false;
				options.Password.RequiredLength = 6;
				options.User.RequireUniqueEmail = false;
			});

			var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenOptions:Key"]));

			services.AddAuthentication().AddCookie(options => {
				options.Cookie.HttpOnly = true;
				options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
				options.LoginPath = "/Auth/Account/Login";
				options.LogoutPath = "/Auth/Account/Logout";
				options.AccessDeniedPath = "/Auth/Account/Login";
				options.SlidingExpiration = true;
			}).AddJwtBearer(options => {
				options.RequireHttpsMetadata = false;
				options.SaveToken = true;
				options.TokenValidationParameters = new TokenValidationParameters() {
					ValidIssuer = "ysp24.ir",
					ValidAudience = "ysp24.ir",
					IssuerSigningKey = symmetricSecurityKey
				};
			});

			services.AddCors(options =>
			{
				options.AddDefaultPolicy(
					builder => {
						builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
					});
			});

			services.AddAuthorization(options => options.AddPolicy("AdminAccess", policy => {
				policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
				policy.RequireAuthenticatedUser();
				policy.RequireRole("Admin");
			}));

			services.AddAuthorization(options => options.AddPolicy("UserAccess", policy => {
				policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
				policy.RequireAuthenticatedUser();
				policy.RequireRole("Admin", "User");
			}));

			services.AddAuthorization(options => options.AddPolicy("ExpertAccess", policy => {
				policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
				policy.RequireAuthenticatedUser();
				policy.RequireRole("Admin", "Expert");
			}));

			services.Configure<IISServerOptions>(options => {
				options.AutomaticAuthentication = true;
			});

			services.AddSwaggerGen();

			services.AddRazorPages();
			services.AddDirectoryBrowser();

		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			} else {
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseSwagger();
			app.UseSwaggerUI();

			app.UseRouting();

			app.UseCors();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints => {
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");

				endpoints.MapControllerRoute(
					name: "areas",
					pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
				);

				endpoints.MapRazorPages();
			});
		}
	}
}
