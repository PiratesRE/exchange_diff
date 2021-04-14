using System;
using System.Collections.Generic;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("AutomaticRepliesProperties", "Microsoft.Exchange.Management.ControlPanel.Client.AutomaticReplies.js")]
	public sealed class AutomaticRepliesProperties : Properties, IScriptControl
	{
		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			ScriptControlDescriptor scriptDescriptor = this.GetScriptDescriptor();
			scriptDescriptor.Type = "AutomaticRepliesProperties";
			this.EnsureChildControls();
			this.AddComponentPropertyIfVisible(scriptDescriptor, "StartTimePicker", "dtpStartTime");
			this.AddComponentPropertyIfVisible(scriptDescriptor, "EndTimePicker", "dtpEndTime");
			this.AddComponentPropertyIfVisible(scriptDescriptor, "InternalMessageEditor", "rteInternalMessage");
			this.AddComponentPropertyIfVisible(scriptDescriptor, "ExternalMessageEditor", "rteExternalMessage");
			return new ScriptControlDescriptor[]
			{
				scriptDescriptor
			};
		}

		private void AddComponentPropertyIfVisible(ScriptControlDescriptor descriptor, string name, string controlName)
		{
			Control control = base.ContentContainer.FindControl(controlName);
			if (control != null && control.Visible)
			{
				descriptor.AddComponentProperty(name, control.ClientID);
			}
		}
	}
}
