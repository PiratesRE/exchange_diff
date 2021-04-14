using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource(null, "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	[ToolboxData("<{0}:HelpControl runat=\"server\" />")]
	public class HelpControl : WebControl
	{
		public static Func<string, string> HelpUrlBuilder { get; set; } = new Func<string, string>(HelpUtil.BuildEhcHref);

		[Bindable(true)]
		[Category("Behavior")]
		[DefaultValue(EACHelpId.Default)]
		public string HelpId
		{
			get
			{
				return this.helpId;
			}
			set
			{
				this.helpId = value;
			}
		}

		public string Text { get; set; }

		[Bindable(true)]
		[Category("Behavior")]
		[DefaultValue(true)]
		public bool ShowHelp
		{
			get
			{
				return this.showHelp;
			}
			set
			{
				this.showHelp = value;
			}
		}

		[Bindable(true)]
		[Category("Behavior")]
		[DefaultValue(false)]
		public bool NeedPublishHelpLinkWhenHidden
		{
			get
			{
				return this.needPublishHelpLinkWhenHidden;
			}
			set
			{
				this.needPublishHelpLinkWhenHidden = value;
			}
		}

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Div;
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (this.ShowHelp)
			{
				this.AddNormalHelpLink();
				base.Render(writer);
				return;
			}
			if (this.NeedPublishHelpLinkWhenHidden)
			{
				this.PublishHelpLink(writer);
			}
		}

		private string GetHref()
		{
			return HelpControl.HelpUrlBuilder(this.HelpId);
		}

		private void AddNormalHelpLink()
		{
			HyperLink hyperLink = new HyperLink();
			hyperLink.ID = "HelpLink";
			hyperLink.NavigateUrl = this.GetHref();
			hyperLink.CssClass = "helpLink";
			hyperLink.ToolTip = Strings.Help;
			hyperLink.Text = (string.IsNullOrEmpty(this.Text) ? Strings.Help : this.Text);
			hyperLink.Attributes.Add("onclick", "PopupWindowManager.showHelpClient(this.href); return false;");
			this.Controls.Add(hyperLink);
		}

		private void PublishHelpLink(HtmlTextWriter writer)
		{
			string script = string.Format("window.getHelpLink = function() {{return '{0}';}};\n", HttpUtility.JavaScriptStringEncode(this.GetHref()));
			ScriptManager.RegisterStartupScript(this, base.GetType(), this.ClientID + "_init", script, true);
		}

		private string helpId = EACHelpId.Default.ToString();

		private bool showHelp = true;

		private bool needPublishHelpLinkWhenHidden;
	}
}
