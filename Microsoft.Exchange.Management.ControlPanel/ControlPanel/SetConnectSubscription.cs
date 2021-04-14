using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("SetConnectSubscription", "Microsoft.Exchange.Management.ControlPanel.Client.Connect.js")]
	public class SetConnectSubscription : ScriptControlBase
	{
		public bool SetFacebook { get; set; }

		public bool CloseWindowWithoutUpdatingSubscription { get; set; }

		public string AppAuthorizationCode { get; set; }

		public string RedirectUri { get; set; }

		public SetConnectSubscription() : base(HtmlTextWriterTag.Div)
		{
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			descriptor.AddProperty("SetFacebook", this.SetFacebook);
			descriptor.AddProperty("CloseWindowWithoutUpdatingSubscription", this.CloseWindowWithoutUpdatingSubscription);
			descriptor.AddProperty("AppAuthorizationCode", this.AppAuthorizationCode);
			descriptor.AddProperty("RedirectUri", this.RedirectUri);
		}

		private const string SetConnectSubscriptionScriptComponent = "SetConnectSubscription";
	}
}
