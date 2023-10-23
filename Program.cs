var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var corsSpecs = "CorsSpecs";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsSpecs,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000",
                                              "https://www.mely.photos",
                                              "http://www.mely.photos")
                                                .AllowAnyHeader()
                                                .AllowAnyMethod();
                      });
});

var app = builder.Build();

app.UseSwagger();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors(corsSpecs);
app.UseAuthorization();

app.MapControllers();

app.Run();
