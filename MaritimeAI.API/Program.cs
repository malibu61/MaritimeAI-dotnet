using MaritimeAI.API.Hubs;
using MaritimeAI.API.Service;
using MaritimeAI.BusinessLayer.Abstract;
using MaritimeAI.BusinessLayer.Concrete;
using MaritimeAI.DataAccessLayer.Abtstract;
using MaritimeAI.DataAccessLayer.Context;
using MaritimeAI.DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyHeader()
               .AllowAnyMethod()
               .SetIsOriginAllowed((host) => true)
               .AllowCredentials();
    });
});


var defaultCulture = CultureInfo.InvariantCulture;          //gelen veride, double değerleri int olarak getiriyordu
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();

//signalR
builder.Services.AddSignalR();


builder.Services.AddHttpClient();

builder.Services.AddHostedService<MaritimeDataService>();


builder.Services.AddScoped<IUserService, MaritimeAI.BusinessLayer.Concrete.UserManager>();
builder.Services.AddScoped<IShipsService, MaritimeAI.BusinessLayer.Concrete.ShipsManager>();
builder.Services.AddScoped<IUserDal, EfUserDal>();

builder.Services.AddDbContext<MaritimeAIContext>();


builder.Services.AddHttpClient<IShipsService, ShipsManager>(client =>
{
    client.BaseAddress = new Uri("https://www.myshiptracking.com/requests/vesselsonmaptempTTT.php?embed=1&type=json&minlat=40&maxlat=45&minlon=20&maxlon=25&zoom=5&selid=null&seltype=null&timecode=-1&slmp=vd8dz&filters=%7B%22vtypes%22%3A%22%2C0%2C3%2C4%2C6%2C7%2C8%2C9%2C10%2C11%2C12%2C13%22%2C%22minsog%22%3A0%2C%22maxsog%22%3A60%2C%22minsz%22%3A10%2C%22maxsz%22%3A500%2C%22minyr%22%3A1950%2C%22maxyr%22%3A2025%2C%22flag%22%3A%22%22%2C%22status%22%3A%22%22%2C%22mapflt_from%22%3A%22%22%2C%22mapflt_dest%22%3A%22%22%7D&_=1760298482555"); // gerçek API URL’ini buraya koy
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();        //jwt
app.UseAuthorization();

app.MapControllers();

app.MapHub<MaritimeHub>("/maritimehub");

app.Run();
