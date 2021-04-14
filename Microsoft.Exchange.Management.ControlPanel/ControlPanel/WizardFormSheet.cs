using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class WizardFormSheet : PropertyPageSheet
	{
		public WizardFormSheet()
		{
			base.ViewModel = "WizardViewModel";
		}
	}
}
