using System;
using System.Web.UI.HtmlControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("FacebookUserConsentForm", "Microsoft.Exchange.Management.ControlPanel.Client.Connect.js")]
	[RequiredScript(typeof(CommonToolkitScripts))]
	public class FacebookUserConsentForm : SlabControl
	{
		public string AuthorizationUrl { get; set; }

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.userConsentForm.Attributes.Add("vm-AuthorizationUrl", this.AuthorizationUrl);
		}

		private const string FacebookUserConsentFormComponent = "FacebookUserConsentForm";

		protected HtmlGenericControl userConsentForm;
	}
}
