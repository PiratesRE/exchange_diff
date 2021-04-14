using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks.ConfigurationSettings
{
	[Cmdlet("Set", "FlightOverride", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class SetFlightOverride : SetOverrideBase
	{
		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public new Version MaxVersion
		{
			get
			{
				return base.MaxVersion;
			}
			set
			{
				base.MaxVersion = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public new Version FixVersion
		{
			get
			{
				return base.FixVersion ?? new Version();
			}
			set
			{
				base.FixVersion = value;
			}
		}

		protected override bool IsFlight
		{
			get
			{
				return true;
			}
		}
	}
}
