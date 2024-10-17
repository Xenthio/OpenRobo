using OpenRobo.WebUI.Components;
namespace OpenRobo.WebUI;

public class MainWebUI
{
	public static void WebUIMain(string[] args)
	{
		Directory.SetCurrentDirectory("WebUI");
		var cd = Directory.GetCurrentDirectory();
		var opts = new WebApplicationOptions()
		{
			Args = args,
			WebRootPath = Path.Join(cd, "wwwroot")
		};

		var builder = WebApplication.CreateBuilder(opts);

		// Add services to the container.
		builder.Services.AddRazorComponents()
			.AddInteractiveServerComponents();

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Error", createScopeForErrors: true);
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		app.UseHttpsRedirection();

		app.UseStaticFiles();
		app.UseAntiforgery();

		app.MapRazorComponents<App>()
			.AddInteractiveServerRenderMode();

		app.Run();
	}
}