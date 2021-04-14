using System;
using System.Web.UI;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("NewConnectSubscription", "Microsoft.Exchange.Management.ControlPanel.Client.Connect.js")]
	public class NewConnectSubscription : ScriptControlBase
	{
		public bool CreateFacebook { get; set; }

		public bool CreateLinkedIn { get; set; }

		public bool CloseWindowWithoutCreatingSubscription { get; set; }

		public string AppAuthorizationCode { get; set; }

		public string RedirectUri { get; set; }

		public string RequestToken { get; set; }

		public string RequestSecret { get; set; }

		public string Verifier { get; set; }

		public NewConnectSubscription() : base(HtmlTextWriterTag.Div)
		{
			Util.RequireUpdateProgressPopUp(this);
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			descriptor.AddProperty("CreateFacebook", this.CreateFacebook);
			descriptor.AddProperty("CreateLinkedIn", this.CreateLinkedIn);
			descriptor.AddProperty("CloseWindowWithoutCreatingSubscription", this.CloseWindowWithoutCreatingSubscription);
			descriptor.AddProperty("AppAuthorizationCode", this.AppAuthorizationCode);
			descriptor.AddProperty("RedirectUri", this.RedirectUri);
			descriptor.AddProperty("RequestToken", this.RequestToken);
			descriptor.AddProperty("RequestSecret", this.RequestSecret);
			descriptor.AddProperty("Verifier", this.Verifier);
		}

		private const string NewConnectSubscriptionScriptComponent = "NewConnectSubscription";
	}
}
