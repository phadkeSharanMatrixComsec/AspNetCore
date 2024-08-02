using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using Entities;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//add services into IoC container
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();

builder.Services.AddDbContext<PersonsDbContext>(options => {
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? "");
});

var app = builder.Build();

// if (builder.Environment.IsDevelopment())
// {
  app.UseSwagger();
  app.UseSwaggerUI();
  app.UseDeveloperExceptionPage();
// }

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
