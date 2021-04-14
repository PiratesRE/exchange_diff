using System;
using System.Globalization;
using System.Web;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource(null, "Microsoft.Exchange.Management.ControlPanel.Client.Navigation.js")]
	public class HybridPage : EcpPage
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.LoadUrlParameters();
			int num = (!string.IsNullOrEmpty(this.onPremiseFeatures)) ? this.onPremiseFeatures.Length : 0;
			char c = (num > 1) ? this.onPremiseFeatures[1] : '0';
			string userFeatureAtCurrentOrg = CrossPremiseUtil.UserFeatureAtCurrentOrg;
			this.crossPremise.Feature = string.Format("0{0}-{1}", c, userFeatureAtCurrentOrg);
			this.crossPremise.LogoutHelperPage = string.Format("https://{0}/ecp/hybridlogouthelper.aspx?xprs={1}", this.onPremiseServer, this.Context.GetRequestUrl().Host);
			this.hasHelpdesk = (c == '1' || userFeatureAtCurrentOrg[1] == '1');
			this.BuildNameDropDown();
		}

		protected void RenderIFrames()
		{
			this.LoadUrlParameters();
			string ietfLanguageTag = CultureInfo.CurrentCulture.IetfLanguageTag;
			bool flag = RbacPrincipal.Current.IsFederatedUser();
			if (!string.IsNullOrEmpty(this.onPremiseServer))
			{
				base.Response.Output.WriteLine("<iframe src='https://{0}/ecp/?xprs={1}{5}&p={2}&mkt={3}&topnav=0&exsvurl=1' class='hw100' id='onprem' style='display:{4}' frameborder='0'></iframe>", new object[]
				{
					this.onPremiseServer,
					this.Context.GetRequestUrl().Host,
					string.IsNullOrEmpty(this.onPremiseStartPageId) ? string.Empty : HttpUtility.UrlEncode(this.onPremiseStartPageId),
					ietfLanguageTag,
					this.crossPremise.OnPremiseFrameVisible ? "block" : "none",
					flag ? "&cross=1" : string.Empty
				});
			}
			base.Response.Output.WriteLine("<iframe src='default.aspx?xprs={0}{4}&p={1}&mkt={2}&topnav=0&exsvurl=1' class='hw100' id='cloud' style='display:{3}' frameborder='0'></iframe>", new object[]
			{
				this.onPremiseServer,
				string.IsNullOrEmpty(this.cloudStartPageId) ? string.Empty : HttpUtility.UrlEncode(this.cloudStartPageId),
				ietfLanguageTag,
				this.crossPremise.OnPremiseFrameVisible ? "none" : "block",
				flag ? "&cross=1" : string.Empty
			});
			base.Response.Output.WriteLine("<iframe src='' id='logouthelper' style='display:none' frameborder='0'></iframe>");
		}

		private void LoadUrlParameters()
		{
			if (!this.urlParamLoaded)
			{
				this.onPremiseServer = base.Request.QueryString["xprs"];
				this.onPremiseFeatures = base.Request.QueryString["xprf"];
				this.onPremiseStartPageId = base.Request.QueryString["op"];
				this.cloudStartPageId = base.Request.QueryString["cp"];
				string text = null;
				if (string.IsNullOrEmpty(this.onPremiseServer))
				{
					text = "xprs";
				}
				else if (string.IsNullOrEmpty(this.onPremiseFeatures))
				{
					text = "xprf";
				}
				if (!string.IsNullOrEmpty(text))
				{
					throw new BadQueryParameterException(text);
				}
				string a = base.Request.QueryString["ov"];
				this.crossPremise.OnPremiseFrameVisible = (a == "1" && !string.IsNullOrEmpty(this.onPremiseServer));
				this.urlParamLoaded = true;
			}
		}

		protected void RenderCssLinks()
		{
			CssFiles.RenderCssLinks(this, _Default.NavigationCssFiles);
		}

		protected void RenderTopNav()
		{
			this.LoadUrlParameters();
			base.Response.Output.Write("<div class='topNav NavigationSprite topNavO365Icon Office365Icon'></div>");
			base.Response.Output.Write("<a href='#' id='{0}' class='topNav {2}' onclick=\"return CrossPremise.NavTo('{0}');\" title='{1}'><span class='topLeftNav'>{1}</span></a>", "enterprise", Strings.Enterprise, this.crossPremise.OnPremiseFrameVisible ? "topNavSelected" : string.Empty);
			base.Response.Output.Write("<a href='#' id='{0}' class='topNav {2}' onclick=\"return CrossPremise.NavTo('{0}');\" title='{1}'><span class='topLeftNav'>{1}</span></a>", "office365", Strings.Office365, this.crossPremise.OnPremiseFrameVisible ? string.Empty : "topNavSelected");
		}

		private void BuildNameDropDown()
		{
			RbacPrincipal rbacPrincipal = RbacPrincipal.Current;
			DropDownCommand dropDownCommand = this.nameDropDown.DropDownCommand;
			dropDownCommand.Name = "UserName";
			dropDownCommand.Text = rbacPrincipal.Name;
			if (this.hasHelpdesk)
			{
				dropDownCommand.Commands.Add(new Command
				{
					Name = "Helpdesk",
					Text = Strings.EntryOnBehalfOf,
					OnClientClick = "CrossPremise.NavTo('helpdesk');"
				});
			}
			if (rbacPrincipal.IsInRole("MailboxFullAccess") && !rbacPrincipal.IsInRole("DelegatedAdmin") && !rbacPrincipal.IsInRole("ByoidAdmin"))
			{
				if (dropDownCommand.Commands.Count > 0)
				{
					dropDownCommand.Commands.Add(new SeparatorCommand());
				}
				dropDownCommand.Commands.Add(new Command
				{
					Name = "SignOff",
					Text = Strings.SignOff,
					OnClientClick = "CrossPremise.NavTo('dualLogout');"
				});
			}
		}

		private const string TopNavFormat = "<a href='#' id='{0}' class='topNav {2}' onclick=\"return CrossPremise.NavTo('{0}');\" title='{1}'><span class='topLeftNav'>{1}</span></a>";

		private string onPremiseServer;

		private string onPremiseFeatures;

		private string onPremiseStartPageId;

		private string cloudStartPageId;

		private bool urlParamLoaded;

		private bool hasHelpdesk;

		protected CrossPremise crossPremise;

		protected DropDownButton nameDropDown;
	}
}
