using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("VoicemailConfigurationProperties", "Microsoft.Exchange.Management.ControlPanel.Client.Voicemail.js")]
	public sealed class VoicemailConfigurationProperties : VoicemailPropertiesBase
	{
		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.Type = "VoicemailConfigurationProperties";
			SmsServiceProviders instance = SmsServiceProviders.Instance;
			scriptDescriptor.AddProperty("OperatorIds", instance.VoiceMailCarrierDictionary.Keys);
			scriptDescriptor.AddProperty("VoiceMailCarrierDictionary", instance.VoiceMailCarrierDictionary);
			return scriptDescriptor;
		}
	}
}
