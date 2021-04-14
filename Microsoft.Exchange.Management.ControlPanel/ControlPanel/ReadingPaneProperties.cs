using System;
using System.Collections.Generic;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("ReadingPaneProperties", "Microsoft.Exchange.Management.ControlPanel.Client.Messaging.js")]
	public sealed class ReadingPaneProperties : Properties, IScriptControl
	{
		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			ScriptControlDescriptor scriptDescriptor = this.GetScriptDescriptor();
			scriptDescriptor.Type = "ReadingPaneProperties";
			return new ScriptControlDescriptor[]
			{
				scriptDescriptor
			};
		}
	}
}
