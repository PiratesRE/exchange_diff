using System;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("ForwardEmailPropertiesWrapper", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	public sealed class ForwardEmailPropertiesWrapper : PropertiesWrapper
	{
		protected override bool IsToolbarRightAlign
		{
			get
			{
				return false;
			}
		}
	}
}
