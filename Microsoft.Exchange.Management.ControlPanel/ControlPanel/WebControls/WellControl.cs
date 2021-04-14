using System;
using System.ComponentModel;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:WellControl runat=server></{0}:WellControl>")]
	[ClientScriptResource("WellControl", "Microsoft.Exchange.Management.ControlPanel.Client.Pickers.js")]
	[AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class WellControl : ScriptControlBase
	{
		public WellControl()
		{
			this.RemoveLinkText = Strings.WellControlRemoveLinkText;
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("RemoveLinkText", this.RemoveLinkText, true);
			descriptor.AddProperty("IdentityProperty", this.IdentityProperty, true);
			descriptor.AddProperty("DisplayProperty", this.DisplayProperty, true);
		}

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Div;
			}
		}

		[Localizable(true)]
		public string RemoveLinkText
		{
			get
			{
				return this.removeLinkText;
			}
			set
			{
				this.removeLinkText = value;
			}
		}

		public string IdentityProperty { get; set; }

		public string DisplayProperty { get; set; }

		private string removeLinkText;
	}
}
