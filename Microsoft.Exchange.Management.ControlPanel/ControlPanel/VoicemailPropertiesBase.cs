using System;
using System.Web.UI;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public abstract class VoicemailPropertiesBase : Properties
	{
		public VoicemailPropertiesBase()
		{
			Util.RequireUpdateProgressPopUp(this);
		}

		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.AddElementProperty("CaptionLabel", base.GetCaptionLabel().ClientID);
			return scriptDescriptor;
		}
	}
}
