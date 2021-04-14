using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("CrossPremise", "Microsoft.Exchange.Management.ControlPanel.Client.Navigation.js")]
	public class CrossPremise : ScriptComponent
	{
		public string Feature { get; set; }

		public string LogoutHelperPage { get; set; }

		public bool OnPremiseFrameVisible { get; set; }

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("Feature", this.Feature);
			descriptor.AddProperty("OnPremiseFrameVisible", this.OnPremiseFrameVisible);
			descriptor.AddProperty("LogoutHelperPage", this.LogoutHelperPage);
		}
	}
}
