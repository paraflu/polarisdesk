using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PolarisDesk.API.Interface;
using PolarisDesk.API.Services;
using PolarisDesk.Models;
using System;

namespace PolarisDesk.API
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

            services.AddControllers();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://localhost:5001/";
                    options.Audience = "polarisdeskapi";
                });


            //Dep
#if DEBUG            
            services.AddTransient<ICrudService<Ticket,Guid>, TicketServiceMock>();
            services.AddTransient<ICrudService<TicketStatus, Guid>, TicketStatusService<TicketStatus, Guid>>();
            services.AddTransient<ICrudService<TicketPriority, Guid>, TicketPriorityService<TicketPriority, Guid>>();
            services.AddTransient<ICrudService<Customer, Guid>, CustomerService<Customer, Guid>>();
#endif

#if !DEBUG
            services.AddTransient<ICrudService<Ticket, Guid>, TicketService<Ticket, Guid>>();
            services.AddTransient<ICrudService<TicketStatus, Guid>, TicketStatusService<TicketStatus, Guid>>();
            services.AddTransient<ICrudService<TicketPriority, Guid>, TicketPriorityService<TicketPriority, Guid>>();
            services.AddTransient<ICrudService<Customer, Guid>, CustomerService<Customer, Guid>>();
#endif

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PolarisDesk.API", Version = "v1" });
            });

            services.AddCors(o => o.AddPolicy("AllowAllCors", builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PolarisDesk.API v1"));
            }

            app.UseCors("AllowAllCors");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
