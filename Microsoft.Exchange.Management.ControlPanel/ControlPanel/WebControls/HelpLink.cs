using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Security.Application;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("HelpLink", "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	[ToolboxData("<{0}:HelpLink runat=\"server\" />")]
	public class HelpLink : ScriptControlBase
	{
		public static Func<string, string> HelpUrlBuilder { get; set; } = new Func<string, string>(HelpUtil.BuildEhcHref);

		public HelpLink() : base(HtmlTextWriterTag.Span)
		{
		}

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

		[DefaultValue("")]
		[Category("Behavior")]
		[Bindable(true)]
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}

		[DefaultValue(false)]
		[Category("Behavior")]
		public bool TextIsFormatString
		{
			get
			{
				return this.textIsFormatString;
			}
			set
			{
				this.textIsFormatString = value;
			}
		}

		protected override void RenderChildren(HtmlTextWriter writer)
		{
			if (this.TextIsFormatString)
			{
				Literal literal = new Literal();
				literal.Text = string.Format(this.Text, this.GetHref());
				this.Controls.Add(literal);
			}
			else
			{
				HyperLink hyperLink = new HyperLink();
				hyperLink.NavigateUrl = this.GetHrefNoEncoding();
				hyperLink.Text = this.Text;
				hyperLink.Attributes.Add("onclick", "PopupWindowManager.showHelpClient(this.href); return false;");
				this.Controls.Add(hyperLink);
			}
			base.RenderChildren(writer);
		}

		private string GetHref()
		{
			return HelpLink.GetHref(this.HelpId);
		}

		private string GetHrefNoEncoding()
		{
			return HelpLink.GetHrefNoEncoding(this.HelpId);
		}

		internal static string GetHref(string helpId)
		{
			return Encoder.HtmlEncode(HelpLink.HelpUrlBuilder(helpId));
		}

		internal static string GetHrefNoEncoding(string helpId)
		{
			return HelpLink.HelpUrlBuilder(helpId);
		}

		internal const string ShowHelpClientScript = "PopupWindowManager.showHelpClient(this.href); return false;";

		private string helpId = EACHelpId.Default.ToString();

		private string text = string.Empty;

		private bool textIsFormatString;
	}
}
