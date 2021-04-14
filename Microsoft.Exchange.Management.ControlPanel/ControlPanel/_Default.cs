using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Online.BOX.UI.Shell;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource(null, "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	public class _Default : EcpPage, IThemable
	{
		public bool HasHybridParameter
		{
			get
			{
				return !string.IsNullOrEmpty(base.Request.QueryString["xprs"]);
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (string.Equals(base.Request.QueryString["flight"], "geminishellux", StringComparison.OrdinalIgnoreCase))
			{
				_Default.SetCookie(this.Context, "flight", "geminishellux", null);
			}
			if (!Util.IsDataCenter && this.HasHybridParameter)
			{
				base.Response.Headers.Remove("X-Frame-Options");
			}
			string text = base.Request.QueryString["xsysprobeid"];
			if (!string.IsNullOrEmpty(text))
			{
				Guid guid;
				SystemProbe.ActivityId = (Guid.TryParse(text.ToString(), out guid) ? guid : Guid.Empty);
				if (SystemProbe.ActivityId != Guid.Empty)
				{
					_Default.SetCookie(this.Context, "xsysprobeid", text, null);
				}
			}
			this.navigation.NavigationTree = this.CreateNavTree();
			this.navigation.CloudServer = this.GetCloudServer();
			this.navigation.ServerVersion = Util.ApplicationVersion;
			this.navigation.HasHybridParameter = this.HasHybridParameter;
			this.InitFeatureSetAndStartPage();
			this.InitTopNav();
		}

		private string GetCloudServer()
		{
			string text = string.Empty;
			if (!Util.IsDataCenter)
			{
				text = OrganizationCache.CrossPremiseServer;
				if (string.IsNullOrEmpty(text))
				{
					text = base.Request.QueryString["xprs"];
				}
			}
			return text;
		}

		private static void SetCookie(HttpContext httpContext, string cookieName, string cookieValue, string cookieDomain)
		{
			HttpCookie httpCookie = new HttpCookie(cookieName);
			httpCookie.HttpOnly = true;
			httpCookie.Path = "/";
			httpCookie.Value = cookieValue;
			if (cookieDomain != null)
			{
				httpCookie.Domain = cookieDomain;
			}
			httpContext.Response.Cookies.Add(httpCookie);
		}

		private void InitTopNav()
		{
			string text = base.Request.QueryString["topnav"];
			if (!string.IsNullOrEmpty(text))
			{
				int num;
				if (!int.TryParse(text, out num) || num < 0 || num > 3)
				{
					num = 1;
				}
				this.topNavType = (_Default.TopNav)num;
			}
			else
			{
				this.topNavType = (RbacPrincipal.Current.IsInRole("MailboxFullAccess") ? (Util.IsDataCenter ? _Default.TopNav.O365GNav : _Default.TopNav.Metro) : _Default.TopNav.NoTopNav);
			}
			if (this.topNavType == _Default.TopNav.Metro)
			{
				this.BuildNameDropDown();
			}
			else
			{
				this.helpControl.Visible = false;
				this.nameDropDown.Visible = false;
			}
			if (this.topNavType == _Default.TopNav.O365GNav || this.topNavType == _Default.TopNav.O365GNavFallback)
			{
				this.navBarClient = NavBarClientBase.Create(this.showAdminFeatures.Value, this.topNavType == _Default.TopNav.O365GNavFallback, false);
				if (this.navBarClient != null)
				{
					this.navBarClient.PrepareNavBarPack();
				}
			}
			if (this.notificationTemplate != null)
			{
				this.notificationTemplate.Visible = this.showAdminFeatures.Value;
			}
			if (this.ntfIcon != null)
			{
				this.ntfIcon.Visible = (this.showAdminFeatures.Value && this.topNavType == _Default.TopNav.Metro);
			}
		}

		protected void RenderAlertBarBegin()
		{
			RbacPrincipal rbacPrincipal = RbacPrincipal.Current;
			string text = null;
			string text2 = null;
			if (!rbacPrincipal.IsInRole("MailboxFullAccess"))
			{
				string logonUser = HttpUtility.HtmlEncode((rbacPrincipal.IsInRole("Enterprise") && rbacPrincipal.RbacConfiguration.SecurityAccessToken != null && rbacPrincipal.RbacConfiguration.SecurityAccessToken.LogonName != null && rbacPrincipal.RbacConfiguration.Impersonated) ? rbacPrincipal.RbacConfiguration.SecurityAccessToken.LogonName : rbacPrincipal.RbacConfiguration.LogonUserDisplayName);
				string accessedUser = HttpUtility.HtmlEncode(rbacPrincipal.RbacConfiguration.ExecutingUserDisplayName);
				text = Strings.OnbehalfOfAlert("<strong>", logonUser, "</strong>", accessedUser);
				text2 = Strings.OnbehalfOfAlert(string.Empty, logonUser, string.Empty, accessedUser);
			}
			else if (rbacPrincipal.IsInRole("DelegatedAdmin") || rbacPrincipal.IsInRole("ByoidAdmin"))
			{
				string targetTenant = HttpContext.Current.GetTargetTenant();
				text = Strings.ManageOtherTenantAlert("<strong>", targetTenant, "</strong>");
				text2 = Strings.ManageOtherTenantAlert(string.Empty, targetTenant, string.Empty);
			}
			if (text != null)
			{
				this.renderAlertBar = true;
				string cssClass = NavigationSprite.GetCssClass(NavigationSprite.SpriteId.EsoBarEdge);
				base.Response.Output.Write("<div id=\"EsoBar\" class=\"{0}\"><img id=\"EsoBarIcon\" class=\"{1}\" src=\"{6}\" alt=\"\" /><div id=\"EsoBarMsg\" title=\"{2}\">{3}</div><img id=\"EsoBarLeftEdge\" class=\"{4}\" src=\"{6}\" alt=\"\"/><img id=\"EsoBarRightEdge\" class=\"{5}\" src=\"{6}\" alt=\"\"></div></div><div id=\"EsoNavWrap\">", new object[]
				{
					HorizontalSprite.GetCssClass(HorizontalSprite.SpriteId.EsoBar),
					CommonSprite.GetCssClass(CommonSprite.SpriteId.Information),
					text2,
					text,
					cssClass,
					cssClass,
					Util.GetSpriteImageSrc(this)
				});
			}
		}

		protected void RenderAlertBarClose()
		{
			if (this.renderAlertBar)
			{
				base.Response.Output.Write("</div>");
			}
		}

		protected void RenderTopNavBegin()
		{
			switch (this.topNavType)
			{
			case _Default.TopNav.NoTopNav:
				base.Response.Output.Write("<div id=\"topNavZone\" role=\"banner\" umc-topregion=\"true\" class=\"{0}\">", "hidden");
				return;
			case _Default.TopNav.O365GNav:
			case _Default.TopNav.O365GNavFallback:
				base.Response.Output.Write("<div id=\"topNavZone\" role=\"banner\" umc-topregion=\"true\" class=\"{0}\">", "o365TopNav");
				this.RenderO365TopNav();
				return;
			}
			base.Response.Output.Write("<div id=\"topNavZone\" role=\"banner\" umc-topregion=\"true\" class=\"{0}\">", "defaultTopNav");
			this.RenderMetroTopNav();
		}

		private void RenderMetroTopNav()
		{
			if (this.showAdminFeatures.Value && RbacPrincipal.Current.IsInRole("ControlPanelAdmin"))
			{
				if (!Util.IsPartnerHostedOnly)
				{
					base.Response.Output.Write("<div class='topNav topNavO365Icon NavigationSprite Office365Icon'></div>");
				}
				if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Eac.Office365DIcon.Enabled)
				{
					base.Response.Output.Write("<a href='#' class='topNav topNavSelected' id='enterprise' tabindex='-1' title='{0}'><span class='topLeftNav'>{0}</span></a>", Strings.Office365D);
					return;
				}
				if (!Util.IsDataCenter)
				{
					base.Response.Output.Write("<a href='#' class='topNav topNavSelected' id='enterprise' tabindex='-1' title='{0}'><span class='topLeftNav'>{0}</span></a>", Strings.Enterprise);
					if (OrganizationCache.EntHasTargetDeliveryDomain && OrganizationCache.EntHasServiceInstance)
					{
						base.Response.Output.Write("<a href='#' id='{0}' class='topNav' onclick=\"return JumpTo('{1}',false, '{2}');\" title='{3}'><span class='topLeftNav'>{3}</span></a>", new object[]
						{
							"office365",
							"office365",
							CrossPremiseUtil.GetLinkToCrossPremise(this.Context, base.Request),
							Strings.Office365
						});
						return;
					}
					base.Response.Output.Write("<a href='{1}' id='{0}' class='topNav' target='_blank' title='{2}'><span class='topLeftNav'>{2}</span></a>", "office365", "http://go.microsoft.com/fwlink/p/?LinkId=258351", Strings.Office365);
					return;
				}
				else
				{
					if (Util.IsMicrosoftHostedOnly)
					{
						base.Response.Output.Write("<a href='#' class='topNav topNavSelected' id='enterprise' tabindex='-1' title='{0}'><span class='topLeftNav'>{0}</span></a>", Strings.Office365);
						return;
					}
					if (Util.IsPartnerHostedOnly)
					{
						base.Response.Output.Write("<a href='#' class='topNav topNavSelected' id='enterprise' tabindex='-1' title='{0}'><span class='topLeftNav'>{0}</span></a>", string.Empty);
						return;
					}
				}
			}
			else
			{
				base.Response.Output.Write("<div class=\"topLeftNav NavigationSprite OwaBrand\"></div>");
			}
		}

		private void RenderO365TopNav()
		{
			if (this.navBarClient != null)
			{
				NavBarPack navBarPack = this.navBarClient.GetNavBarPack();
				if (navBarPack != null)
				{
					string str = "NavBar.Create(" + navBarPack.ToJsonString(null) + ");";
					string script = "Sys.Application.add_init(function(){" + str + "});";
					base.ClientScript.RegisterStartupScript(base.GetType(), "NavBarInfo", script, true);
				}
			}
		}

		protected void RenderFooterBar()
		{
			if (this.topNavType == _Default.TopNav.O365GNav || this.topNavType == _Default.TopNav.O365GNavFallback)
			{
				base.Response.Output.Write("<div id=\"footerBar\" class=\"footerBar\"></div>");
			}
		}

		protected void RenderTopNavEnd()
		{
			base.Response.Output.Write("</div>");
		}

		protected void RenderTitleBar()
		{
			if (this.showAdminFeatures.Value)
			{
				base.Response.Output.Write("<div id=\"titleBar\" class=\" titleBar {0}\" ><div class=\"mainHeader\"><span>{1}</span></div></div>", (this.topNavType == _Default.TopNav.NoTopNav) ? "noTopNavTitleBar" : string.Empty, Util.IsDataCenter ? ClientStrings.DataCenterMainHeader : ClientStrings.EnterpriseMainHeader);
			}
		}

		protected void RenderMiddleNav()
		{
			switch (this.topNavType)
			{
			case _Default.TopNav.NoTopNav:
				return;
			case _Default.TopNav.O365GNav:
			case _Default.TopNav.O365GNavFallback:
				if (NavigationUtil.ShouldRenderOwaLink(RbacPrincipal.Current, this.showAdminFeatures.Value) && !string.IsNullOrWhiteSpace(HttpContext.Current.GetOwaNavigationParameter()))
				{
					base.Response.Output.Write("<div id=\"middleNavZone\" class=\"{3}\" ><div class=\"middleLeftNav\"><a id=\"returnToOWA\" onclick=\"return JumpTo('{2}', true);\" title=\"{0}\" href=\"#\"><img id=\"imgRetrunToOWA\" class=\"NavigationSprite ReturnToOWA\" src='{1}' alt=\"{0}\" title=\"{0}\"/></a></div></div>", new object[]
					{
						OwaOptionStrings.ReturnToOWA,
						Util.GetSpriteImageSrc(this),
						HttpUtility.HtmlEncode(HttpUtility.JavaScriptStringEncode(EcpUrl.GetOwaNavigateBackUrl())),
						"o365MiddleNav"
					});
					return;
				}
				return;
			}
			if ((!this.showAdminFeatures.Value || !RbacPrincipal.Current.IsInRole("ControlPanelAdmin")) && !string.IsNullOrWhiteSpace(HttpContext.Current.GetOwaNavigationParameter()))
			{
				base.Response.Output.Write("<div id=\"middleNavZone\" class=\"{3}\" ><div class=\"middleLeftNav\"><a id=\"returnToOWA\" onclick=\"return JumpTo('{2}', true);\" title=\"{0}\" href=\"#\"><img id=\"imgRetrunToOWA\" class=\"NavigationSprite ReturnToOWA\" src='{1}' alt=\"{0}\" title=\"{0}\"/></a></div></div>", new object[]
				{
					OwaOptionStrings.ReturnToOWA,
					Util.GetSpriteImageSrc(this),
					HttpUtility.HtmlEncode(HttpUtility.JavaScriptStringEncode(EcpUrl.GetOwaNavigateBackUrl())),
					"defaultMiddleNav"
				});
			}
		}

		protected void RenderMainNavBegin()
		{
			string arg = string.Empty;
			switch (this.topNavType)
			{
			case _Default.TopNav.NoTopNav:
				arg = "noTopNavMainNav";
				goto IL_3B;
			case _Default.TopNav.O365GNav:
			case _Default.TopNav.O365GNavFallback:
				arg = "o365MainNav";
				goto IL_3B;
			}
			arg = "defaultMainNav";
			IL_3B:
			base.Response.Output.Write("<div id=\"MainNav\" class=\"{0}\">", arg);
		}

		protected void RenderMainNavEnd()
		{
			base.Response.Output.Write("</div>");
		}

		private void BuildNameDropDown()
		{
			RbacPrincipal rbacPrincipal = RbacPrincipal.Current;
			DropDownCommand dropDownCommand = this.nameDropDown.DropDownCommand;
			dropDownCommand.Name = "UserName";
			dropDownCommand.Text = rbacPrincipal.Name;
			if (NavigationUtil.ShouldRenderOwaLink(rbacPrincipal, this.showAdminFeatures.Value))
			{
				dropDownCommand.Commands.Add(new Command
				{
					Name = "MailLnk",
					Text = Strings.MyMail,
					OnClientClick = "JumpTo('" + EcpUrl.OwaVDir + "', true);"
				});
			}
			if (this.HasAccessTo("helpdesk"))
			{
				dropDownCommand.Commands.Add(new Command
				{
					Name = "Helpdesk",
					Text = Strings.EntryOnBehalfOf,
					OnClientClick = "JumpTo('helpdesk');"
				});
			}
			if (NavigationUtil.ShouldRenderLogoutLink(rbacPrincipal))
			{
				if (dropDownCommand.Commands.Count > 0)
				{
					dropDownCommand.Commands.Add(new SeparatorCommand());
				}
				dropDownCommand.Commands.Add(new Command
				{
					Name = "SignOff",
					Text = Strings.SignOff,
					OnClientClick = "Navigation.SignOut();"
				});
			}
		}

		private bool HasAccessTo(string topNavId)
		{
			bool result = false;
			foreach (NavigationTreeNode navigationTreeNode in this.navigation.NavigationTree.Children)
			{
				if (navigationTreeNode.ID == topNavId)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		protected void UpdateNavZoneHtml(int level)
		{
			if (level == 1 && !this.showAdminFeatures.Value)
			{
				base.Response.Output.Write(string.Format("<div id=\"priNavHeader\"><span>{0}</span></div>", OwaOptionStrings.Options));
			}
			NavigationTreeNode navigationTree = this.navigation.NavigationTree;
			string format = _Default.itemFormats[level];
			string text = _Default.selectedClasses[level];
			string text2 = _Default.normalClasses[level];
			NavigationTreeNode selectedNode = this.GetSelectedNode(level - 1, navigationTree);
			if (selectedNode != null && selectedNode.Children.Count > 0)
			{
				int selected = selectedNode.Selected;
				base.Response.Output.Write(_Default.startFormats[level]);
				for (int i = 0; i < selectedNode.Children.Count; i++)
				{
					NavigationTreeNode navigationTreeNode = selectedNode.Children[i];
					if (string.IsNullOrEmpty(navigationTreeNode.HybridRole))
					{
						bool flag = i == selected;
						string text3 = flag ? text : text2;
						string text4 = flag ? "true" : "false";
						string str = " " + ((level == 1) ? ClientStrings.PrimaryNavigation : ClientStrings.SecondaryNavigation);
						base.Response.Output.Write(format, new object[]
						{
							navigationTreeNode.ID,
							text3,
							navigationTreeNode.Title,
							text4,
							navigationTreeNode.Title + str
						});
					}
				}
				base.Response.Output.Write(_Default.endFormats[level]);
			}
		}

		protected void RenderCssLinks()
		{
			CssFiles.RenderCssLinks(this, _Default.NavigationCssFiles);
		}

		private NavigationTreeNode GetSelectedNode(int level, NavigationTreeNode navTree)
		{
			NavigationTreeNode navigationTreeNode = navTree;
			for (int i = 0; i <= level; i++)
			{
				if (navigationTreeNode.Children.Count == 0)
				{
					return null;
				}
				navigationTreeNode = navigationTreeNode.Children[navigationTreeNode.Selected];
			}
			return navigationTreeNode;
		}

		private void InitFeatureSetAndStartPage()
		{
			bool flag = false;
			NavigationTreeNode navigationTree = this.navigation.NavigationTree;
			if (navigationTree != null)
			{
				string text = base.Request.QueryString["p"];
				if (!string.IsNullOrEmpty(text) && text != "helpdesk")
				{
					flag = this.SelectStartPage(navigationTree, EcpUrl.EcpVDir + text, text);
					if (flag)
					{
						this.showAdminFeatures = new bool?(navigationTree.Children[navigationTree.Selected].ID == "myorg");
					}
				}
				else
				{
					this.referrer = base.Request.QueryString["rfr"];
					if (!string.IsNullOrEmpty(this.referrer))
					{
						this.referrer = this.referrer.ToLower();
						string a;
						if ((a = this.referrer) != null)
						{
							if (!(a == "owa") && !(a == "olk"))
							{
								if (a == "admin")
								{
									this.showAdminFeatures = new bool?(true);
								}
							}
							else
							{
								this.showAdminFeatures = new bool?(false);
							}
						}
					}
					if (this.showAdminFeatures == null)
					{
						this.showAdminFeatures = new bool?(RbacPrincipal.Current.IsInRole("ControlPanelAdmin"));
					}
					flag = this.SelectStartPage(navigationTree, null, this.showAdminFeatures.Value ? "myorg" : "myself");
				}
				if (flag)
				{
					if (navigationTree.Children[navigationTree.Selected].ID == "myself")
					{
						this.Context.ThrowIfViewOptionsWithBEParam(FeatureSet.Options);
						for (int i = navigationTree.Children.Count - 1; i > 1; i--)
						{
							navigationTree.Children.RemoveAt(i);
						}
					}
					else if (navigationTree.Children[0].ID == "myself")
					{
						navigationTree.Children.RemoveAt(0);
						navigationTree.Selected = 0;
					}
				}
			}
			if (!flag)
			{
				throw new UrlNotFoundOrNoAccessException(Strings.UrlNotFoundOrNoAccessMessage);
			}
			base.FeatureSet = (this.showAdminFeatures.Value ? FeatureSet.Admin : FeatureSet.Options);
			this.helpControl.InAdminUI = this.showAdminFeatures.Value;
		}

		private bool SelectStartPage(NavigationTreeNode node, string startPageUrl, string id)
		{
			bool flag = false;
			if (string.Compare(node.ID, id, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(node.Url, startPageUrl, StringComparison.OrdinalIgnoreCase) == 0)
			{
				flag = string.IsNullOrEmpty(node.HybridRole);
			}
			else
			{
				int i = 0;
				while (i < node.Children.Count)
				{
					NavigationTreeNode navigationTreeNode = node.Children[i];
					flag = this.SelectStartPage(navigationTreeNode, startPageUrl, id);
					if (flag)
					{
						node.Selected = i;
						if (!navigationTreeNode.Children.IsNullOrEmpty())
						{
							break;
						}
						string text = base.Request.QueryString["q"];
						if (!string.IsNullOrEmpty(text))
						{
							NavigationTreeNode navigationTreeNode2 = navigationTreeNode;
							navigationTreeNode2.Url = navigationTreeNode2.Url + "?" + text;
							break;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
			return flag;
		}

		private NavigationTreeNode CreateNavTree()
		{
			NavigationTreeNode navigationTreeNode = null;
			using (SiteMapDataSource siteMapDataSource = new SiteMapDataSource())
			{
				SiteMapNode rootNode = siteMapDataSource.Provider.RootNode;
				navigationTreeNode = this.CreateDataContract(rootNode);
				if (navigationTreeNode == null || !navigationTreeNode.HasContentPage)
				{
					throw new CmdletAccessDeniedException(Strings.AccessDeniedMessage);
				}
			}
			return navigationTreeNode;
		}

		private NavigationTreeNode CreateDataContract(SiteMapNode sNode)
		{
			NavigationTreeNode navigationTreeNode = null;
			if (sNode != null)
			{
				navigationTreeNode = new NavigationTreeNode(sNode);
				foreach (object obj in sNode.ChildNodes)
				{
					SiteMapNode sNode2 = (SiteMapNode)obj;
					NavigationTreeNode navigationTreeNode2 = this.CreateDataContract(sNode2);
					if (navigationTreeNode2.HasContentPage)
					{
						navigationTreeNode.AddChild(navigationTreeNode2);
					}
				}
				navigationTreeNode.AggregateHybridRole();
			}
			return navigationTreeNode;
		}

		public const string IdEnterprise = "enterprise";

		public const string IdOffice365 = "office365";

		public const string NodeIdMyOrg = "myorg";

		public const string NodeIdMyself = "myself";

		public const string NodeIdHelpdesk = "helpdesk";

		public const string SystemProbeCookie = "xsysprobeid";

		private const string AlertBarFormatStart = "<div id=\"EsoBar\" class=\"{0}\"><img id=\"EsoBarIcon\" class=\"{1}\" src=\"{6}\" alt=\"\" /><div id=\"EsoBarMsg\" title=\"{2}\">{3}</div><img id=\"EsoBarLeftEdge\" class=\"{4}\" src=\"{6}\" alt=\"\"/><img id=\"EsoBarRightEdge\" class=\"{5}\" src=\"{6}\" alt=\"\"></div></div><div id=\"EsoNavWrap\">";

		private const string AlertBarFormatEnd = "</div>";

		private const string TopNavZoneFormat = "<div id=\"topNavZone\" role=\"banner\" umc-topregion=\"true\" class=\"{0}\">";

		private const string TopNavBrandFormat = "<a href='#' class='topNav topNavSelected' id='enterprise' tabindex='-1' title='{0}'><span class='topLeftNav'>{0}</span></a>";

		private const string TopNavO365Format = "<a href='#' id='{0}' class='topNav' onclick=\"return JumpTo('{1}',false, '{2}');\" title='{3}'><span class='topLeftNav'>{3}</span></a>";

		internal const string TopNavAdFormat = "<a href='{1}' id='{0}' class='topNav' target='_blank' title='{2}'><span class='topLeftNav'>{2}</span></a>";

		private const string TopNavOwaOptionFormat = "<div class=\"topLeftNav NavigationSprite OwaBrand\"></div>";

		private const string MiddleNavFormat = "<div id=\"middleNavZone\" class=\"{3}\" ><div class=\"middleLeftNav\"><a id=\"returnToOWA\" onclick=\"return JumpTo('{2}', true);\" title=\"{0}\" href=\"#\"><img id=\"imgRetrunToOWA\" class=\"NavigationSprite ReturnToOWA\" src='{1}' alt=\"{0}\" title=\"{0}\"/></a></div></div>";

		private static string[] startFormats = new string[]
		{
			null,
			"<ul>",
			string.Empty
		};

		private static string[] itemFormats = new string[]
		{
			null,
			"<li id=\"Menu_{0}\" class=\"{1}\"><span><a class=\"priNavLnk\" href=\"#\" aria-expanded=\"{3}\" aria-label=\"{4}\">{2}</a></span></li>",
			"<div id=\"Menu_{0}\" class=\"{1}\"><a class=\"secNavLnk\" href=\"#\" aria-expanded=\"{3}\" aria-label=\"{4}\">{2}</a></div>"
		};

		private static string[] endFormats = new string[]
		{
			null,
			"</ul>",
			string.Empty
		};

		private static string[] selectedClasses = new string[]
		{
			null,
			"priSelected",
			"secNavBtn secSelected"
		};

		private static string[] normalClasses = new string[]
		{
			string.Empty,
			string.Empty,
			"secNavBtn"
		};

		internal static readonly string[] NavigationCssFiles = new string[]
		{
			"NavCombine.css"
		};

		protected Navigation navigation;

		protected Label lblSeparator;

		protected HtmlForm mainForm;

		protected DropDownButton nameDropDown;

		protected Label ntfIcon;

		protected UserControl notificationTemplate;

		protected NavigationHelpControl helpControl;

		private bool? showAdminFeatures;

		private bool renderAlertBar;

		private _Default.TopNav topNavType;

		private string referrer;

		private NavBarClientBase navBarClient;

		private enum TopNav
		{
			NoTopNav,
			Metro,
			O365GNav,
			O365GNavFallback,
			MaxValue = 3
		}
	}
}
