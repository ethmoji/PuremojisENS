using Newtonsoft.Json;
using PuremojiENS.Server;
using PuremojiENS.Server.APIs.OpenSea;
using PuremojiENS.Server.APIs.TheGraph;
using PuremojiENS.Server.Directory;
using PuremojiENS.Server.Worker;
using PuremojiENS.Shared;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


var config = builder.Configuration.Get<Config>();
builder.Services.AddSingleton(config);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddTransient<TheGraph>();
builder.Services.AddTransient<OpenSea>();

builder.Services.AddHostedService<UpdatePuremojis>();

builder.Services.AddEntityFrameworkSqlite()
    .AddDbContext<EmojisDbContext>();

builder.Services.AddTransient<EmojisDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

//using (var client = new EmojisDbContext())
//{
//    client.Database.EnsureCreated();
//}

app.Run();
