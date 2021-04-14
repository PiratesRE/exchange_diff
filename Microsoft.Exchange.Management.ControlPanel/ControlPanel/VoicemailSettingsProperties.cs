using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("VoicemailSettingsProperties", "Microsoft.Exchange.Management.ControlPanel.Client.Voicemail.js")]
	public sealed class VoicemailSettingsProperties : VoicemailPropertiesBase
	{
		public WebServiceMethod ResetPINWebServiceMethod { get; private set; }

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.ResetPINWebServiceMethod = new WebServiceMethod();
			this.ResetPINWebServiceMethod.ServiceUrl = base.ServiceUrl;
			this.ResetPINWebServiceMethod.ID = "ResetPIN";
			this.ResetPINWebServiceMethod.Method = "ResetPIN";
			this.ResetPINWebServiceMethod.ParameterNames = WebServiceParameterNames.Identity;
			this.Controls.Add(this.ResetPINWebServiceMethod);
		}

		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.Type = "VoicemailSettingsProperties";
			scriptDescriptor.AddComponentProperty("ResetPINWebServiceMethod", this.ResetPINWebServiceMethod.ClientID);
			return scriptDescriptor;
		}
	}
}
