var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddRestEaseClient<IAppApi>(builder.HostEnvironment.BaseAddress);

builder.Services.AddFluxor(options => options
    .ScanAssemblies(typeof(Anchor).Assembly)
    .UseReduxDevTools());

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredToast();

await builder.Build().RunAsync();
