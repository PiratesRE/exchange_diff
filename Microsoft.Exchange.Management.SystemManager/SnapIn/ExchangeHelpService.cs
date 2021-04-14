using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.CommonHelpProvider;
using Microsoft.Exchange.Diagnostics.Components.Management.SystemManager;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.SnapIn
{
	internal class ExchangeHelpService
	{
		protected ExchangeHelpService()
		{
		}

		public static void Initialize()
		{
			Microsoft.Exchange.CommonHelpProvider.HelpProvider.InitializeViaCmdlet(Microsoft.Exchange.CommonHelpProvider.HelpProvider.HelpAppName.Toolbox, null, PSConnectionInfoSingleton.GetInstance().GetMonadConnectionInfo());
		}

		public static void ShowHelpFromHelpTopicId(string helpTopicId)
		{
			ExchangeHelpService.ShowHelpFromHelpTopicId(null, helpTopicId);
		}

		public static void ShowHelpFromHelpTopicId(Control control, string helpTopicId)
		{
			string helpUrlForTopic = ExchangeHelpService.GetHelpUrlForTopic(helpTopicId);
			ExchangeHelpService.ShowHelpFromUrl(control, helpUrlForTopic);
		}

		public static void ShowHelpFromPage(ExchangePage page)
		{
			string helpUrlFromPage = ExchangeHelpService.GetHelpUrlFromPage(page);
			ExchangeHelpService.ShowHelpFromUrl(page, helpUrlFromPage);
		}

		internal static string GetHelpUrlForTopic(string helpTopicId)
		{
			return Microsoft.Exchange.CommonHelpProvider.HelpProvider.ConstructHelpRenderingUrl(helpTopicId).ToString();
		}

		internal static string GetHelpUrlFromPage(ExchangePage page)
		{
			ExchangeForm exchangeForm = page.ParentForm as ExchangeForm;
			string result;
			if (exchangeForm == null || string.IsNullOrEmpty(exchangeForm.HelpTopic))
			{
				result = Microsoft.Exchange.CommonHelpProvider.HelpProvider.ConstructHelpRenderingUrl(page.HelpTopic).ToString();
			}
			else
			{
				result = Microsoft.Exchange.CommonHelpProvider.HelpProvider.ConstructHelpRenderingUrl(exchangeForm.HelpTopic).ToString();
			}
			return result;
		}

		private static void ShowHelpFromUrl(string helpUrl)
		{
			ExchangeHelpService.ShowHelpFromUrl(null, helpUrl);
		}

		private static void ShowHelpFromUrl(Control control, string helpUrl)
		{
			if (!string.IsNullOrEmpty(helpUrl))
			{
				ExTraceGlobals.ProgramFlowTracer.TraceFunction<string>(0L, "*--ExchangeHelpService.ShowHelpFromUrl: {0}", helpUrl);
				string url = helpUrl.Substring(helpUrl.IndexOf("http"));
				ExchangeHelpService.ieHelper.NavigateInSingleIE(url, ExchangeHelpService.GetUIService(control));
			}
		}

		private static IUIService GetUIService(Control control)
		{
			IServiceProvider serviceProvider = control as IServiceProvider;
			IUIService result;
			if (serviceProvider != null)
			{
				result = (((IUIService)serviceProvider.GetService(typeof(IUIService))) ?? new UIService(control));
			}
			else
			{
				result = new UIService(control);
			}
			return result;
		}

		private static IEHelper ieHelper = new IEHelper();
	}
}
