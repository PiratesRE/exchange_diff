using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ToolboxData("<{0}:OwaScriptsUtil runat=server></{0}:OwaScriptsUtil>")]
	[ClientScriptResource("OwaScriptsUtil", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	public class OwaScriptsUtil : ScriptControlBase
	{
		public OwaScriptsUtil() : base(HtmlTextWriterTag.Div)
		{
			this.iframeElement = new HtmlGenericControl(HtmlTextWriterTag.Iframe.ToString());
			this.iframeElement.ID = "iframe";
			this.iframeElement.Style.Add(HtmlTextWriterStyle.Display, "none");
			this.iframeElement.Style.Add(HtmlTextWriterStyle.Width, "0px");
			this.iframeElement.Style.Add(HtmlTextWriterStyle.Height, "0px");
			this.iframeElement.Attributes["tabindex"] = "-1";
			this.iframeElement.Attributes["class"] = "HiddenForScreenReader";
			if (Util.IsIE())
			{
				this.iframeElement.Attributes["src"] = ThemeResource.BlankHtmlPath;
			}
			this.Controls.Add(this.iframeElement);
			Util.RequireUpdateProgressPopUp(this);
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("NameSpace", this.NameSpace, true);
			descriptor.AddProperty("EsoFullAccess", this.EsoFullAccess, true);
			descriptor.AddElementProperty("IframeElement", this.IframeElementID, this);
		}

		public string NameSpace { get; set; }

		public string IframeElementID
		{
			get
			{
				return this.iframeElement.ClientID;
			}
		}

		public bool EsoFullAccess
		{
			get
			{
				return HttpContext.Current.IsExplicitSignOn() && RbacPrincipal.Current.IsInRole("MailboxFullAccess");
			}
		}

		private HtmlGenericControl iframeElement;
	}
}
