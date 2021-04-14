using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks.ConfigurationSettings
{
	[Cmdlet("Remove", "FlightOverride", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveFlightOverride : RemoveOverrideBase
	{
		protected override bool IsFlight
		{
			get
			{
				return true;
			}
		}
	}
}
