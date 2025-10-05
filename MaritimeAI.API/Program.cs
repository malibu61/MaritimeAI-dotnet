using MaritimeAI.BusinessLayer.Abstract;
using MaritimeAI.BusinessLayer.Concrete;
using MaritimeAI.DataAccessLayer.Abtstract;
using MaritimeAI.DataAccessLayer.Context;
using MaritimeAI.DataAccessLayer.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()    //WithOrigins("https://mydomain.com")   bu þekilde güncellenmesi gerek.
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddScoped<IUserService, MaritimeAI.BusinessLayer.Concrete.UserManager>();
builder.Services.AddScoped<IUserDal, EfUserDal>();

builder.Services.AddDbContext<MaritimeAIContext>();



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
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
