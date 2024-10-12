using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BotWebUI.Pages
{
	public class LeaderboardModel : PageModel
	{
		private readonly ILogger<LeaderboardModel> _logger;

		public LeaderboardModel(ILogger<LeaderboardModel> logger)
		{
			_logger = logger;
		}

		public void OnGet()
		{

		}
	}
}
