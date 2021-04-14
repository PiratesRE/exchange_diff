using System;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public class Options : OwaPage
	{
		protected override bool UseStrictMode
		{
			get
			{
				return false;
			}
		}

		public Options()
		{
			this.applicationElement = Convert.ToString(base.OwaContext.FormsRegistryContext.ApplicationElement);
			if (base.OwaContext.FormsRegistryContext.Type != null)
			{
				this.type = base.OwaContext.FormsRegistryContext.Type;
				return;
			}
			this.type = "Messaging";
		}

		public string ApplicationElement
		{
			get
			{
				return this.applicationElement;
			}
		}

		public string Type
		{
			get
			{
				return this.type;
			}
		}

		public void RenderNavigation()
		{
			Navigation navigation = new Navigation(NavigationModule.Options, base.OwaContext, base.Response.Output);
			navigation.Render();
		}

		public void RenderNavigationOptions()
		{
			OptionsNavigation optionsNavigation = new OptionsNavigation(this.type);
			optionsNavigation.Render(base.Response.Output, base.UserContext);
		}

		public void RenderOptions(string helpFile)
		{
			OptionsBar optionsBar = new OptionsBar(base.UserContext, base.Response.Output, OptionsBar.SearchModule.None, OptionsBar.RenderingFlags.OptionsSelected, null);
			optionsBar.Render(helpFile);
		}

		public void RenderSubOptions()
		{
			string key;
			switch (key = this.type)
			{
			case "Regional":
				this.RenderOptions(HelpIdsLight.OptionsRegionalSettingsLight.ToString());
				return;
			case "Messaging":
				this.RenderOptions(HelpIdsLight.OptionsMessagingLight.ToString());
				return;
			case "JunkEmail":
				this.RenderOptions(HelpIdsLight.OptionsJunkEmailLight.ToString());
				return;
			case "Calendar":
				this.RenderOptions(HelpIdsLight.OptionsCalendarLight.ToString());
				return;
			case "Oof":
				this.RenderOptions(HelpIdsLight.OptionsOofLight.ToString());
				return;
			case "ChangePassword":
				this.RenderOptions(HelpIdsLight.OptionsChangePasswordLight.ToString());
				return;
			case "General":
				this.RenderOptions(HelpIdsLight.OptionsAccessibilityLight.ToString());
				return;
			case "About":
				this.RenderOptions(HelpIdsLight.OptionsAboutLight.ToString());
				return;
			case "Eas":
				this.RenderOptions(HelpIdsLight.OptionsMobileDevicesLight.ToString());
				return;
			}
			this.RenderOptions(HelpIdsLight.OptionsLight.ToString());
		}

		public void RenderOptionsControl()
		{
			if (this.optionsControl != null)
			{
				this.optionsControl.Render();
			}
		}

		public void RenderHeaderToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, true);
			toolbar.RenderStart();
			if (this.type == "About" || this.type == "Eas")
			{
				toolbar.RenderButton(ToolbarButtons.CloseText);
			}
			else
			{
				toolbar.RenderButton(ToolbarButtons.Save);
			}
			toolbar.RenderFill();
			toolbar.RenderButton(ToolbarButtons.CloseImage);
			toolbar.RenderEnd();
		}

		public void RenderFooterToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, false);
			toolbar.RenderStart();
			toolbar.RenderFill();
			toolbar.RenderEnd();
		}

		protected override void OnInit(EventArgs e)
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "opturl", false);
			if (!string.IsNullOrEmpty(queryStringParameter))
			{
				this.type = queryStringParameter;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			string key;
			switch (key = this.type)
			{
			case "Messaging":
				this.optionsControl = new MessagingOptions(base.OwaContext, base.Response.Output);
				break;
			case "Regional":
				this.optionsControl = new RegionalOptions(base.OwaContext, base.Response.Output);
				break;
			case "JunkEmail":
				if (!base.UserContext.IsFeatureEnabled(Feature.JunkEMail))
				{
					throw new OwaSegmentationException("Junk e-mail filtering configuration is disabled");
				}
				this.optionsControl = new JunkEmailOptions(base.OwaContext, base.Response.Output);
				break;
			case "Calendar":
				if (!base.UserContext.IsFeatureEnabled(Feature.Calendar))
				{
					throw new OwaSegmentationException("Calendar is disabled");
				}
				this.optionsControl = new CalendarOptions(base.OwaContext, base.Response.Output);
				break;
			case "Oof":
				this.optionsControl = new OofOptions(base.OwaContext, base.Response.Output);
				break;
			case "ChangePassword":
				if (!base.UserContext.IsFeatureEnabled(Feature.ChangePassword))
				{
					throw new OwaSegmentationException("Change password is disabled");
				}
				this.optionsControl = new ChangePassword(base.OwaContext, base.Response.Output);
				break;
			case "General":
				this.optionsControl = new GeneralOptions(base.OwaContext, base.Response.Output);
				break;
			case "About":
				this.optionsControl = new AboutOptions(base.OwaContext, base.Response.Output);
				break;
			case "Eas":
				if (!base.UserContext.IsMobileSyncEnabled())
				{
					throw new OwaSegmentationException("Eas is disabled");
				}
				this.optionsControl = new MobileOptions(base.OwaContext, base.Response.Output);
				break;
			case "":
				this.optionsControl = new MessagingOptions(base.OwaContext, base.Response.Output);
				break;
			}
			if (this.optionsControl == null)
			{
				throw new OwaInvalidRequestException("Invalid application type querystring parameter");
			}
		}

		internal const string About = "About";

		internal const string Calendar = "Calendar";

		internal const string ChangePassword = "ChangePassword";

		internal const string General = "General";

		internal const string JunkEmail = "JunkEmail";

		internal const string Messaging = "Messaging";

		internal const string Mobile = "Eas";

		internal const string Oof = "Oof";

		internal const string Regional = "Regional";

		private string applicationElement;

		private string type;

		protected OptionsBase optionsControl;
	}
}
