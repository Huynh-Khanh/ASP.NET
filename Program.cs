using HuynhDuyKhanh_2122110004.Data;
using HuynhDuyKhanh_2122110004.Model;
using HuynhDuyKhanh_2122110004.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HuynhDuyKhanh_2122110004.Controllers;


var builder = WebApplication.CreateBuilder(args);

// Cấu hình JWT từ appsettings.json (nếu bạn chuyển sang dùng sau này)
var jwtSettings = new JwtSettings
{
    SecretKey = "THIS_IS_A_SUPER_SECRET_KEY_1234567890_32",
    Issuer = "HuynhDuyKhanh_2122110004App",
    Audience = "HuynhDuyKhanh_2122110004Users",
    ExpirationMinutes = 60
};

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddScoped<TokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Thêm JWT Authentication vào Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Nhập token theo định dạng: Bearer {your token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication(); // BẮT BUỘC trước Authorization
app.UseAuthorization();

// Endpoint mapping
app.MapControllers();

// Gọi các Map endpoint
app.MapAuthEndpoints();

app.MapBrandEndpoints();

app.MapCategoryEndpoints();

app.MapOrderEndpoints();

app.MapOrderItemEndpoints();


app.MapUserEndpoints();

app.MapProductEndpoints();

app.Run();
