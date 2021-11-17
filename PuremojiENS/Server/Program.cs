using Newtonsoft.Json;
using PuremojiENS.Server;
using PuremojiENS.Server.APIs.OpenSea;
using PuremojiENS.Server.APIs.TheGraph;
using PuremojiENS.Server.Worker;
using PuremojiENS.Shared;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


var config = builder.Configuration.Get<Config>();
builder.Services.AddSingleton(config);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var content = File.ReadAllText(config.PuremojisPath, Encoding.UTF8);
var puremojis = JsonConvert.DeserializeObject<List<Emoji>>(content);
builder.Services.AddSingleton(puremojis);

content = File.ReadAllText(config.ValidTokenIdsPath, Encoding.UTF8);
var validTokenIds = JsonConvert.DeserializeObject<List<string>>(content);
builder.Services.AddSingleton(validTokenIds);

builder.Services.AddTransient<TheGraph>();
builder.Services.AddTransient<OpenSea>();

builder.Services.AddHostedService<UpdatePuremojis>();

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

app.Run();
