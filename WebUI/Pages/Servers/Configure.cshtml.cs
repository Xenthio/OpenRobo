using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BotWebUI.Pages
{
	public class ConfigureModel : PageModel
	{
		private readonly ILogger<ConfigureModel> _logger;

		public ConfigureModel(ILogger<ConfigureModel> logger)
		{
			_logger = logger;
		}

		public void OnGet()
		{

		}
	}
}
