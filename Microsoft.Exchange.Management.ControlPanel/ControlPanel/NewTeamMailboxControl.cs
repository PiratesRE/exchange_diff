using System;
using System.Text;
using System.Web;
using System.Web.Security.AntiXss;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("NewTeamMailboxControl", "Microsoft.Exchange.Management.ControlPanel.Client.OrgSettings.js")]
	public class NewTeamMailboxControl : ScriptControlBase
	{
		public NewTeamMailboxControl() : base(HtmlTextWriterTag.Div)
		{
			this.brandingLink = new HyperLink();
			this.brandingLink.ID = "brandingLink";
			this.brandingLink.CssClass = "brandingLink";
			this.contentPanel = new Panel();
			this.contentPanel.CssClass = "newTMContent";
			this.contentPanel.ID = "contentPanel";
		}

		protected override void OnLoad(EventArgs e)
		{
			this.displayName = this.Context.Request.QueryString["Title"];
			if (string.IsNullOrEmpty(this.displayName))
			{
				throw new BadQueryParameterException("Title");
			}
			string uriString = this.Context.Request.QueryString["SPUrl"];
			Uri uri;
			if (!Uri.TryCreate(uriString, UriKind.Absolute, out uri))
			{
				throw new BadQueryParameterException("SPUrl");
			}
			if (!(uri.Scheme == Uri.UriSchemeHttp) && !(uri.Scheme == Uri.UriSchemeHttps))
			{
				throw new BadQueryParameterException("SPUrl");
			}
			this.sharePointUrl = uri.AbsoluteUri;
			string text = this.Context.Request.QueryString["SPTMAppUrl"];
			if (string.IsNullOrEmpty(text))
			{
				text = "_layouts/15/TeamMailbox/mailbox.aspx";
			}
			StringBuilder stringBuilder = new StringBuilder(this.sharePointUrl);
			if (stringBuilder[stringBuilder.Length - 1] == '/')
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			if (text[0] != '/')
			{
				stringBuilder.Append('/');
			}
			stringBuilder.Append(text);
			Uri uri2;
			if (!Uri.TryCreate(stringBuilder.ToString(), UriKind.Absolute, out uri2))
			{
				throw new BadQueryParameterException("SPTMAppUrl");
			}
			this.teamMailboxAppUrl = uri2.AbsoluteUri;
			base.OnLoad(e);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			LiteralControl child = new LiteralControl("\r\n            <div class='brandingArrowContainer'>\r\n                <svg version='1.1' width='6px' height='8px' viewBox='0 0 6 8'>\r\n                    <polygon class='vectorFillColor' points='0.504,0.161 0.504,7.849 5.504,4.005 '/>\r\n                </svg>\r\n            </div>\r\n            <div class='appIconContainer'>\r\n                <svg version='1.1' width='24px' height='24px' viewBox='0 0 48 48'>\r\n                    <g>\r\n                        <path class='vectorFillColor' d='M37.586,32.563c-0.557-0.164-1.48-0.335-2.258-0.335c-2.734,0-4.15,1.416-4.15,1.416v6.836h5.078 l0.004-6.836C36.26,33.645,36.644,32.823,37.586,32.563z'/>\r\n                        <polyline class='vectorStrokeColor' fill='none' stroke-width='1.5' stroke-miterlimit='10' points='28.785,35.256 1.148,35.256 1.148,8.156 43.824,8.156 43.824,22.609 \t'/>\r\n                        <path class='vectorFillColor' d='M46.852,33.645c0,0-1.367-1.416-4.102-1.416s-4.15,1.416-4.15,1.416v6.836h8.252V33.645z'/>\r\n                        <circle class='vectorFillColor' cx='34.879' cy='27.734' r='2.969'/>\r\n                        <circle class='vectorFillColor' cx='42.645' cy='27.734' r='2.969'/>\r\n                        <rect class='vectorFillColor' x='31.91' y='11.188' width='8.141' height='6'/>\r\n                        <line class='vectorStrokeColor' fill='none' stroke-width='1.5' stroke-miterlimit='10' x1='7.863' y1='23.229' x2='28.885' y2='23.229'/>\r\n                        <line class='vectorStrokeColor' fill='none' stroke-width='1.5' stroke-miterlimit='10' x1='7.863' y1='27.042' x2='25.885' y2='27.042'/>\r\n                    </g>\r\n                </svg>\r\n            </div>\r\n            ");
			Label label = new Label();
			label.ID = "brandingLabel";
			label.CssClass = "brandingLabel";
			label.Text = OwaOptionStrings.TeamMailboxAppTitle;
			Panel panel = new Panel();
			panel.ID = "brandingPanel";
			panel.CssClass = "newTMNavBrandingPanel";
			this.brandingLink.Text = AntiXssEncoder.HtmlEncode(this.displayName, true);
			panel.Controls.Add(this.brandingLink);
			panel.Controls.Add(child);
			panel.Controls.Add(label);
			DropDownButton dropDownButton = new DropDownButton(HtmlTextWriterTag.Span);
			dropDownButton.ID = "nameDropDown";
			dropDownButton.DropDownCommand.Name = "UserName";
			dropDownButton.DropDownCommand.Text = RbacPrincipal.Current.Name;
			dropDownButton.DropDownCommand.Commands.Add(new Command
			{
				Name = "SignOff",
				Text = Strings.SignOff,
				OnClientClick = NewTeamMailboxControl.GetLogoffScript()
			});
			DropDownButton dropDownButton2 = new DropDownButton(HtmlTextWriterTag.Span);
			dropDownButton2.ID = "helpControl";
			DropDownCommand dropDownCommand = dropDownButton2.DropDownCommand;
			dropDownCommand.Name = "Help";
			dropDownCommand.DefaultCommandName = "ContextualHelp";
			dropDownCommand.Text = Strings.Help;
			if (Util.IsDataCenter)
			{
				Command command = new Command();
				command.Name = "Community";
				command.Text = Strings.Community;
				command.OnClientClick = string.Format("PopupWindowManager.showHelpClient('{0}');", HttpUtility.HtmlEncode(HelpUtil.BuildCommunitySiteHref()));
				dropDownCommand.Commands.Add(command);
				Command command2 = new Command();
				command2.Name = "Privacy";
				command2.Text = Strings.Privacy;
				command2.OnClientClick = string.Format("PopupWindowManager.showHelpClient('{0}');", HttpUtility.HtmlEncode(HelpUtil.BuildPrivacyStatmentHref()));
				dropDownCommand.Commands.Add(command2);
			}
			Command command3 = new Command();
			command3.Name = "Copyright";
			command3.Text = Strings.CopyRight;
			command3.OnClientClick = string.Format("PopupWindowManager.showHelpClient('{0}');", "http://go.microsoft.com/fwlink/p/?LinkId=256676");
			dropDownCommand.Commands.Add(command3);
			Panel panel2 = new Panel();
			panel2.ID = "topRightNavPanel";
			panel2.CssClass = "newTMNavRight";
			panel2.Controls.Add(dropDownButton);
			panel2.Controls.Add(dropDownButton2);
			Panel panel3 = new Panel();
			panel3.ID = "headerPanel";
			panel3.CssClass = "newTMNavHeader";
			panel3.Controls.Add(panel);
			panel3.Controls.Add(panel2);
			Panel panel4 = new Panel();
			panel4.CssClass = "newTMSeparator";
			panel4.ID = "headerSeparatorPanel";
			this.Controls.Add(panel3);
			this.Controls.Add(panel4);
			this.Controls.Add(this.contentPanel);
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("DisplayName", this.displayName);
			descriptor.AddProperty("SharePointUrl", this.sharePointUrl);
			descriptor.AddProperty("TeamMailboxAppUrl", this.teamMailboxAppUrl);
			descriptor.AddElementProperty("BrandingLink", this.brandingLink.ClientID, true);
			descriptor.AddElementProperty("ContentPanel", this.contentPanel.ClientID, true);
		}

		private static string GetLogoffScript()
		{
			string str = EcpUrl.EcpVDir + "logoff.aspx?src=exch";
			return "window.location.href = '" + str + "';";
		}

		private const string NewTeamMailboxControlScriptComponent = "NewTeamMailboxControl";

		private const string TitleQueryParam = "Title";

		private const string SPUrlQueryParam = "SPUrl";

		private const string TeamMailboxAppUrlQueryParam = "SPTMAppUrl";

		private const string TeamMailboxAppUrlFragmentDefault = "_layouts/15/TeamMailbox/mailbox.aspx";

		private const char SlashSeparator = '/';

		private string teamMailboxAppUrl;

		private string displayName;

		private string sharePointUrl;

		private HyperLink brandingLink;

		private Panel contentPanel;
	}
}
