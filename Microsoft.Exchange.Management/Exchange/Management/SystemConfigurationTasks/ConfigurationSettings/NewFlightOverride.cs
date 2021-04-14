using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks.ConfigurationSettings
{
	[Cmdlet("New", "FlightOverride", SupportsShouldProcess = true)]
	public sealed class NewFlightOverride : NewOverrideBase
	{
		protected override bool IsFlight
		{
			get
			{
				return true;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
		public string Flight
		{
			get
			{
				return base.Fields["Flight"] as string;
			}
			set
			{
				base.Fields["Flight"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public new Version FixVersion
		{
			get
			{
				return base.FixVersion;
			}
			set
			{
				base.FixVersion = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.MaxVersion == null && this.FixVersion == null)
			{
				base.WriteError(new SettingOverrideMaxVersionOrFixVersionRequiredException(), ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}

		protected override SettingOverrideXml GetXml()
		{
			SettingOverrideXml xml = base.GetXml();
			xml.FlightName = this.Flight;
			return xml;
		}

		protected override VariantConfigurationOverride GetOverride()
		{
			return new VariantConfigurationFlightOverride(this.Flight, base.Parameters.ToArray());
		}
	}
}
