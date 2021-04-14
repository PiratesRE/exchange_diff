using System;
using System.Collections.Generic;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("RegionalProperties", "Microsoft.Exchange.Management.ControlPanel.Client.RegionalProperties.js")]
	public sealed class RegionalProperties : Properties, IScriptControl
	{
		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			ScriptControlDescriptor scriptDescriptor = this.GetScriptDescriptor();
			scriptDescriptor.Type = "RegionalProperties";
			scriptDescriptor.AddProperty("LanguageDateSets", RegionalSettingsSlab.LanguageDateSets);
			scriptDescriptor.AddProperty("LanguageTimeSets", RegionalSettingsSlab.LanguageTimeSets);
			return new ScriptControlDescriptor[]
			{
				scriptDescriptor
			};
		}
	}
}
