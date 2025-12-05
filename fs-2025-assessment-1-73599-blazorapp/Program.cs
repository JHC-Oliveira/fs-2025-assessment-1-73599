using fs_2025_assessment_1_73599_blazorapp.Components;
using fs_2025_assessment_1_73599_blazorapp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

// Add HttpClient for API communication
builder.Services.AddScoped(sp =>
	new HttpClient { BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7253") });

// Add StationsApiClient service
builder.Services.AddScoped<StationsApiClient>();

// Add CORS support for API calls
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowLocalhost", policy =>
	{
		policy.WithOrigins("https://localhost:7253")
			  .AllowAnyMethod()
			  .AllowAnyHeader();
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors("AllowLocalhost");

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();