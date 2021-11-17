using Blazor.Analytics;
using BlazorTable;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PuremojiENS.Client;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazorTable();
builder.Services.AddLoadingBar();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }.EnableIntercept(sp));

builder.Services.AddGoogleAnalytics("G-7TJ49RJQVT");

builder.UseLoadingBar();

await builder.Build().RunAsync();
