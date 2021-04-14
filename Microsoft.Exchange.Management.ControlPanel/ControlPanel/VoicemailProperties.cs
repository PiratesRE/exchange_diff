using System;
using System.Web.UI;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("VoicemailProperties", "Microsoft.Exchange.Management.ControlPanel.Client.Voicemail.js")]
	public sealed class VoicemailProperties : VoicemailPropertiesBase
	{
		public WebServiceMethod ClearSettingsWebServiceMethod { get; private set; }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			CommonMaster commonMaster = (CommonMaster)this.Page.Master;
			commonMaster.AddCssFiles("VoicemailSprite.css");
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.ClearSettingsWebServiceMethod = new WebServiceMethod();
			this.ClearSettingsWebServiceMethod.ServiceUrl = base.ServiceUrl;
			this.ClearSettingsWebServiceMethod.ID = "ClearSettings";
			this.ClearSettingsWebServiceMethod.Method = "ClearSettings";
			this.ClearSettingsWebServiceMethod.ParameterNames = WebServiceParameterNames.Identity;
			this.Controls.Add(this.ClearSettingsWebServiceMethod);
		}

		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.Type = "VoicemailProperties";
			scriptDescriptor.AddComponentProperty("ClearSettingsWebServiceMethod", this.ClearSettingsWebServiceMethod.ClientID);
			scriptDescriptor.AddProperty("LCID", Util.GetLCID());
			return scriptDescriptor;
		}
	}
}
