using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "ExchangeServerAccessLicense")]
	public sealed class GetExchangeServerAccessLicense : Task
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			foreach (ExchangeServerAccessLicense sendToPipeline in GetExchangeServerAccessLicense.SupportedLicenses)
			{
				base.WriteObject(sendToPipeline);
			}
			TaskLogger.LogExit();
		}

		private static readonly ExchangeServerAccessLicense[] SupportedLicenses = new ExchangeServerAccessLicense[]
		{
			new ExchangeServerAccessLicense(ExchangeServerAccessLicense.ServerVersionMajor.E15, ExchangeServerAccessLicense.AccessLicenseType.Standard, ExchangeServerAccessLicense.UnitLabelType.Server),
			new ExchangeServerAccessLicense(ExchangeServerAccessLicense.ServerVersionMajor.E15, ExchangeServerAccessLicense.AccessLicenseType.Enterprise, ExchangeServerAccessLicense.UnitLabelType.Server),
			new ExchangeServerAccessLicense(ExchangeServerAccessLicense.ServerVersionMajor.E15, ExchangeServerAccessLicense.AccessLicenseType.Standard, ExchangeServerAccessLicense.UnitLabelType.CAL),
			new ExchangeServerAccessLicense(ExchangeServerAccessLicense.ServerVersionMajor.E15, ExchangeServerAccessLicense.AccessLicenseType.Enterprise, ExchangeServerAccessLicense.UnitLabelType.CAL)
		};
	}
}
