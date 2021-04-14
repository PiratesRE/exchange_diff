using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks.ConfigurationSettings
{
	[Cmdlet("Get", "FlightOverride", DefaultParameterSetName = "Identity")]
	public sealed class GetFlightOverride : GetOverrideBase
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
