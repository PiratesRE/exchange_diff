using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.InstallExchangeStreamingOpticsTask)]
	[Cmdlet("Install", "ExchangeStreamingOptics")]
	public class InstallExchangeStreamingOptics : ManageExchangeStreamingOptics
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
