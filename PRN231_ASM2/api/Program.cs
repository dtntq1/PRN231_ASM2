using api.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ODataConventionModelBuilder modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<Member>("Members");
modelBuilder.EntitySet<Product>("Products");
modelBuilder.EntitySet<Order>("Orders");
modelBuilder.EntitySet<Category>("Categories");
modelBuilder.EntitySet<OrderDetail>("OrderDetails");

builder.Services.AddControllers().AddOData(option => option.Select().OrderBy()
.Filter().Count().Expand().SetMaxTop(100)
.AddRouteComponents("odata", modelBuilder.GetEdmModel()));

builder.Services.AddDbContext<eStoreContext>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
