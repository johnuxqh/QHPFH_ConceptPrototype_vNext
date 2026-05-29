using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using QHPFH_ConceptPrototype;
using MudBlazor.Services;
using QHPFH_ConceptPrototype.Services;
using QHPFH_ConceptPrototype.Services.Kpi;
using QHPFH_ConceptPrototype.Services.Adaptive;
using QHPFH_ConceptPrototype.Services.Experience;
using QHPFH_ConceptPrototype.Services.Layout;
using QHPFH_ConceptPrototype.Services.Context;
using QHPFH_ConceptPrototype.Services.Navigation;
using QHPFH_ConceptPrototype.Services.Workspace;
using QHPFH_ConceptPrototype.Services.Actions;
using QHPFH_ConceptPrototype.Services.Rules;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddScoped<QHPFH_ConceptPrototype.Components.Shells.NavigationState>();
builder.Services.AddScoped<PrototypeDataStore>();
builder.Services.AddScoped<PrototypeDataService>();
builder.Services.AddScoped<KpiCalculationService>();
builder.Services.AddScoped<PrototypeExperienceStateService>();
builder.Services.AddScoped<AdaptivePerspectiveEngine>();
builder.Services.AddScoped<ExperienceModeEngine>();
builder.Services.AddScoped<LayoutVariantEngine>();
builder.Services.AddScoped<ContextAwarenessService>();
builder.Services.AddScoped<NavigationStateService>();
builder.Services.AddScoped<WorkspaceDensityEngine>();
builder.Services.AddScoped<GlobalActionService>();
builder.Services.AddScoped<OperationalRulesService>();

await builder.Build().RunAsync();
