using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Business.Data.Repositories;
using StargateAPI.Controllers.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<StargateContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("StarbaseApiDatabase")));

builder.Services.AddScoped<IRepository<Person>, Repository<Person>>();
builder.Services.AddScoped<IRepository<AstronautDetail>, Repository<AstronautDetail>>();
builder.Services.AddScoped<IRepository<AstronautDuty>, Repository<AstronautDuty>>();
builder.Services.AddScoped<IRepository<Log>, Repository<Log>>();

builder.Services.AddMediatR(cfg =>
{
    cfg.AddRequestPreProcessor<CreateAstronautDutyPreProcessor>();
    cfg.AddRequestPreProcessor<CreatePersonPreProcessor>();
    cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly);
});

builder.Services.AddExceptionHandler<ExceptionHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(options => {});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


