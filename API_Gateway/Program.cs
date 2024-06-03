using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddReverseProxy()
  .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy")); //appsettings.json dosyasından okurken
#region LoadFromMemory
//.LoadFromMemory(new List<RouteConfig>   //in memory'den okurken
//{
//    new RouteConfig
//    {
//         RouteId = "API1-Route",
//         ClusterId  = "API1-Cluster",
//         Match = new()
//         {
//              Path = "/api1/{**catch-all}"
//         },
//         Transforms = new List<Dictionary<string, string>>
//         {
//            new Dictionary<string, string>()
//            {
//                ["RequestHeader"] = "api1-request-header",
//                ["Append"] = "api1 request"
//            },
//               new()
//            {
//                { "ResponseHeader", "api1-response-header" },
//                { "Append", "api1 response" },
//                { "When", "Always" },
//            }
//         }
//    },
//    new RouteConfig
//    {
//         RouteId = "API2-Route",
//         ClusterId  = "API2-Cluster",
//         Match = new()
//         {
//              Path = "/api2/{**catch-all}"
//         },
//         Transforms = new List<Dictionary<string, string>>
//         {
//            new Dictionary<string, string>()
//            {
//                ["RequestHeader"] = "api2-request-header",
//                ["Append"] = "api2 request"
//            },
//               new()
//            {
//                { "ResponseHeader", "api2-response-header" },
//                { "Append", "api2 response" },
//                { "When", "Always" },
//            }
//         }
//    },
//    new RouteConfig
//    {
//         RouteId = "API3-Route",
//         ClusterId  = "API3-Cluster",
//         Match = new()
//         {
//              Path = "/api3/{**catch-all}"
//         },
//         Transforms = new List<Dictionary<string, string>>
//         {
//            new Dictionary<string, string>()
//            {
//                ["RequestHeader"] = "api3-request-header",
//                ["Append"] = "api3 request"
//            },
//               new()
//            {
//                { "ResponseHeader", "api3-response-header" },
//                { "Append", "api3 response" },
//                { "When", "Always" },
//            }
//         }
//    }
//}, new List<ClusterConfig>
//{
//    new ClusterConfig()
//    {
//        ClusterId = "API1-Cluster",
//        Destinations = new Dictionary<string, DestinationConfig>
//        {
//            ["destination1"] = new(){ Address = "https://localhost:7042"}
//        }
//    },
//    new ClusterConfig()
//    {
//        ClusterId = "API2-Cluster",
//        Destinations = new Dictionary<string, DestinationConfig>
//        {
//            ["destination1"] = new(){ Address = "https://localhost:7093"}
//        }
//    },
//    new ClusterConfig()
//    {
//        ClusterId = "API3-Cluster",
//        Destinations = new Dictionary<string, DestinationConfig>
//        {
//            ["destination1"] = new(){ Address = "https://localhost:7072"}
//        }
//    }
//});
#endregion

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuer = true,
               ValidateAudience = true,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               ValidIssuer = builder.Configuration["Jwt:Issuer"],
               ValidAudience = builder.Configuration["Jwt:Audience"],
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
           };
       });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser()); //yetki olarak authenticate olması yeterli demiş oluyoruz.
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();
