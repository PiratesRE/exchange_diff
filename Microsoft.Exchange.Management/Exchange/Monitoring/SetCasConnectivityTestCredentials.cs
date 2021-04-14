using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("set", "CasConnectivityTestCredentials")]
	public class SetCasConnectivityTestCredentials : TestCasConnectivity
	{
		protected override string MonitoringEventSource
		{
			get
			{
				return "MSExchange Monitoring CasConnectivityTestCredentials";
			}
		}

		protected override uint GetDefaultTimeOut()
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		protected override List<CasTransactionOutcome> BuildPerformanceOutcomes(TestCasConnectivity.TestCasConnectivityRunInstance instance, string mailboxFqdn)
		{
			throw new NotSupportedException("BuildPerformanceOutcomes() should not be called from SetCasConnectivityCredentials");
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InitializeTopologyInformation();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				LocalizedException ex = null;
				if (!Datacenter.IsMultiTenancyEnabled())
				{
					ex = base.ResetAutomatedCredentialsOnMailboxServer(this.localServer, true);
				}
				if (ex == null)
				{
					base.WriteMonitoringEvent(1000, this.MonitoringEventSource, EventTypeEnumeration.Success, Strings.AllActiveSyncTransactionsSucceeded);
				}
				else if (ex is CasHealthUserNotFoundException)
				{
					base.WriteMonitoringEvent(1008, this.MonitoringEventSource, EventTypeEnumeration.Warning, Strings.InstructResetCredentials(base.ShortErrorMsgFromException(ex)));
				}
				else
				{
					base.WriteMonitoringEvent(1001, this.MonitoringEventSource, EventTypeEnumeration.Error, base.ShortErrorMsgFromException(ex));
				}
			}
			finally
			{
				if (base.MonitoringContext)
				{
					base.WriteMonitoringData();
				}
				TaskLogger.LogExit();
			}
		}
	}
}
