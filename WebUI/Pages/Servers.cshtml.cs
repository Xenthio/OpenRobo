using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BotWebUI.Pages
{
	public class ServerModel : PageModel
	{
		private readonly ILogger<ServerModel> _logger;

		public ServerModel(ILogger<ServerModel> logger)
		{
			_logger = logger;
		}

		public void OnGet()
		{

		}
	}
}
