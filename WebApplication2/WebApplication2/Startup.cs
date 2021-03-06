using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using Swashbuckle.AspNetCore.Swagger;

namespace WebApplication2
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            //配置Swagger
            //注册Swagger生成器，定义一个Swagger 文档
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("ReservationController", new OpenApiInfo
                {
                    Version = "v2",
                    Title = "reservation API文档",
                    Description = "预订",
                });
                c.SwaggerDoc("CustomerController", new OpenApiInfo
                {
                    Version = "v2",
                    Title = "customer API文档",
                    Description = "顾客",
                });
                c.SwaggerDoc("DishController", new OpenApiInfo
                {
                    Version = "v2",
                    Title = "dish API文档",
                    Description = "菜品",
                });
                c.SwaggerDoc("TableController", new OpenApiInfo
                {
                    Version = "v2",
                    Title = "dining_table API文档",
                    Description = "餐桌",
                });
                c.SwaggerDoc("OrderController", new OpenApiInfo
                {
                    Version = "v2",
                    Title = "dish_order API文档",
                    Description = "订单",
                });
                c.SwaggerDoc("EvaluationController", new OpenApiInfo
                {
                    Version = "v2",
                    Title = "evaluation API文档",
                    Description = "评价",
                });
            });
            services.AddMvc(x => x.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //启用中间件服务生成Swagger
            app.UseSwagger();
            //启用中间件服务生成Swagger，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/CustomerController/swagger.json", "customer API");
                c.SwaggerEndpoint("/swagger/TableController/swagger.json", "dining_table API");
                c.SwaggerEndpoint("/swagger/DishController/swagger.json", "dish API");
                c.SwaggerEndpoint("/swagger/OrderController/swagger.json", "dish_order API");
                c.SwaggerEndpoint("/swagger/EvaluationController/swagger.json", "evaluation API");
                c.SwaggerEndpoint("/swagger/ReservationController/swagger.json", "reservation API");
                c.RoutePrefix = string.Empty;//设置根节点访问
            });

            app.UseMvc();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
