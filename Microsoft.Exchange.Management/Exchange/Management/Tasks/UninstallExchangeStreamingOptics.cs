using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Uninstall", "ExchangeStreamingOptics")]
	[LocDescription(Strings.IDs.UninstallExchangeStreamingOpticsTask)]
	public class UninstallExchangeStreamingOptics : ManageExchangeStreamingOptics
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
