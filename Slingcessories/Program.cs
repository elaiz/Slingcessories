using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Slingcessories;
using Slingcessories.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Point HttpClient to API service (adjust port if your API runs on a different one)
builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7289/") // API base from service project's https profile
});

// Register services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UserStateService>();

await builder.Build().RunAsync();
