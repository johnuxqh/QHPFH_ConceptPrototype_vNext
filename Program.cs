using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using QHPFH_ConceptPrototype;
using MudBlazor.Services;
using QHPFH_ConceptPrototype.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddScoped<QHPFH_ConceptPrototype.Components.Shells.NavigationState>();
builder.Services.AddScoped<PrototypeDataStore>();

await builder.Build().RunAsync();
