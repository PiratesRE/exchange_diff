using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource(null, "Microsoft.Exchange.Management.ControlPanel.Client.SMSProperties.js")]
	public sealed class SmsCommunicationSlabProperties : SmsSlabProperties
	{
		public SmsCommunicationSlabProperties() : base("DisableObject", "EditNotification.aspx")
		{
		}

		private string RedirectionUrl { get; set; }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			string text = this.Page.Request.QueryString["backUr"];
			Uri uri;
			if (!string.IsNullOrEmpty(text) && Uri.TryCreate(text, UriKind.Absolute, out uri) && (string.Equals(uri.Scheme, "http", StringComparison.InvariantCultureIgnoreCase) || string.Equals(uri.Scheme, "https", StringComparison.InvariantCultureIgnoreCase)) && string.Equals(uri.Host, this.Context.GetRequestUrl().Host, StringComparison.InvariantCultureIgnoreCase))
			{
				this.RedirectionUrl = text;
			}
		}

		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.Type = "SmsCommunicationSlabProperties";
			if (this.RedirectionUrl != null)
			{
				scriptDescriptor.AddScriptProperty("RedirectionUrl", this.RedirectionUrl.ToJsonString(null));
			}
			return scriptDescriptor;
		}
	}
}
