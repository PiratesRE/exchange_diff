using System;
using System.ComponentModel;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:NavigationHelpControl runat=\"server\" />")]
	public class NavigationHelpControl : DropDownButton
	{
		[DefaultValue(false)]
		public bool IsInCrossPremise { get; set; }

		[DefaultValue(true)]
		public bool InAdminUI
		{
			get
			{
				return this.inAdminUI;
			}
			set
			{
				if (this.inAdminUI != value)
				{
					this.inAdminUI = value;
					string helpId = value ? EACHelpId.Default.ToString() : OptionsHelpId.OwaOptionsDefault.ToString();
					string arg = HttpUtility.JavaScriptStringEncode(HelpUtil.BuildEhcHref(helpId));
					Command command = base.DropDownCommand.Commands.FindCommandByName("ContextualHelp");
					if (command != null)
					{
						command.OnClientClick = string.Format("PopupWindowManager.showContextualHelp('{0}', {1});", arg, this.InAdminUI ? "false" : "true");
					}
					Command command2 = base.DropDownCommand.Commands.FindCommandByName("CmdletLogging");
					if (command2 != null && !value)
					{
						command2.Visible = false;
					}
				}
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			DropDownCommand dropDownCommand = base.DropDownCommand;
			dropDownCommand.AllowAddSubCommandIcon = true;
			dropDownCommand.Name = "Help";
			dropDownCommand.DefaultCommandName = "ContextualHelp";
			dropDownCommand.Text = string.Empty;
			dropDownCommand.ImageAltText = Strings.Help;
			dropDownCommand.ImageId = CommandSprite.SpriteId.HelpCommand;
			string arg = HttpUtility.JavaScriptStringEncode(HelpUtil.BuildEhcHref(EACHelpId.Default.ToString()));
			Command command = new Command();
			command.Name = "ContextualHelp";
			command.Text = Strings.Help;
			command.OnClientClick = string.Format("PopupWindowManager.showContextualHelp('{0}', {1});", arg, this.InAdminUI ? "false" : "true");
			dropDownCommand.Commands.Add(command);
			Command command2 = new Command();
			command2.Name = "FVA";
			command2.Text = Strings.FVAToggleText;
			command2.ClientCommandHandler = "FVAHelpEnabledToggleCommandHandler";
			dropDownCommand.Commands.Add(command2);
			Command command3 = new Command();
			command3.Name = "PerformanceConsole";
			command3.Visible = (this.IsInCrossPremise || StringComparer.OrdinalIgnoreCase.Equals("true", WebConfigurationManager.AppSettings["ShowPerformanceConsole"]));
			command3.Text = Strings.PerformanceConsole;
			command3.ClientCommandHandler = "ShowPerfConsoleCommandHandler";
			dropDownCommand.Commands.Add(command3);
			Command command4 = new Command();
			command4.Name = "CmdletLogging";
			command4.Text = Strings.CmdLogButtonText;
			if (this.IsInCrossPremise)
			{
				command4.ClientCommandHandler = "HybridCmdletLoggingCommandHandler";
			}
			else
			{
				command4.Visible = EacFlightUtility.GetSnapshotForCurrentUser().Eac.CmdletLogging.Enabled;
				command4.OnClientClick = "CmdletLoggingNavHelper.OpenCmdletLoggingWindow('CmdletLogging');";
			}
			dropDownCommand.Commands.Add(command4);
			if (Util.IsDataCenter)
			{
				Command command5 = new Command();
				command5.Name = "Community";
				command5.Text = Strings.Community;
				command5.OnClientClick = string.Format("PopupWindowManager.showHelpClient('{0}');", HttpUtility.HtmlEncode(HelpUtil.BuildCommunitySiteHref()));
				dropDownCommand.Commands.Add(command5);
			}
			string text = HelpUtil.BuildPrivacyStatmentHref();
			if (!string.IsNullOrEmpty(text))
			{
				Command command6 = new Command();
				command6.Name = "Privacy";
				command6.Text = Strings.Privacy;
				command6.OnClientClick = string.Format("PopupWindowManager.showHelpClient('{0}');", HttpUtility.HtmlEncode(text));
				dropDownCommand.Commands.Add(command6);
			}
			Command command7 = new Command();
			command7.Name = "Copyright";
			command7.Text = Strings.CopyRight;
			command7.OnClientClick = string.Format("PopupWindowManager.showHelpClient('{0}');", "http://go.microsoft.com/fwlink/p/?LinkId=256676");
			dropDownCommand.Commands.Add(command7);
		}

		public NavigationHelpControl() : base(HtmlTextWriterTag.Span)
		{
		}

		private const string ContextualHelpName = "ContextualHelp";

		private const string CmdletLoggingName = "CmdletLogging";

		private const string CopyRightName = "Copyright";

		private bool inAdminUI = true;
	}
}
