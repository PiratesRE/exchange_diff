using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Test", "MonitoringServiceBasicCmdlet")]
	public sealed class TestMonitoringServiceBasicCmdletTask : Task
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter MonitoringContext
		{
			get
			{
				return (SwitchParameter)(base.Fields["MonitoringContext"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["MonitoringContext"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Default")]
		public ServerIdParameter Server
		{
			get
			{
				return ((ServerIdParameter)base.Fields["Server"]) ?? new ServerIdParameter();
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		public PSCredential MailboxCredential { get; set; }

		protected override bool IsKnownException(Exception e)
		{
			return base.IsKnownException(e) || MonitoringHelper.IsKnownExceptionForMonitoring(e);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				base.InternalValidate();
				if (base.HasErrors)
				{
				}
			}
			catch (LocalizedException exception)
			{
				this.WriteError(exception, ErrorCategory.OperationStopped, this, true);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void InternalBeginProcessing()
		{
			if (this.MonitoringContext)
			{
				this.monitoringData = new MonitoringData();
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalBeginProcessing();
			TaskLogger.LogEnter();
			try
			{
				MonitoringServiceBasicCmdletOutcome sendToPipeline = new MonitoringServiceBasicCmdletOutcome(this.Server.ToString());
				this.PerformMonitoringServiceBasicCmdletTest(ref sendToPipeline);
				base.WriteObject(sendToPipeline);
			}
			catch (LocalizedException e)
			{
				this.HandleException(e);
			}
			finally
			{
				if (this.MonitoringContext)
				{
					base.WriteObject(this.monitoringData);
				}
				TaskLogger.LogExit();
			}
		}

		private void PerformMonitoringServiceBasicCmdletTest(ref MonitoringServiceBasicCmdletOutcome result)
		{
			SmtpAddress? multiTenantAutomatedTaskUser = TestConnectivityCredentialsManager.GetMultiTenantAutomatedTaskUser(this, null, null, DatacenterUserType.EDU);
			if (multiTenantAutomatedTaskUser == null)
			{
				throw new MailboxNotFoundException(new MailboxIdParameter(), null);
			}
			result.Update(MonitoringServiceBasicCmdletResultEnum.Success, null);
			if (this.MonitoringContext)
			{
				this.monitoringData.Events.Add(new MonitoringEvent(TestMonitoringServiceBasicCmdletTask.CmdletMonitoringEventSource, 1000, EventTypeEnumeration.Success, string.Format("TestMonitoringServiceBasicCmdlet succeeded. Test user returned: [{0}]", multiTenantAutomatedTaskUser.ToString())));
			}
		}

		private Server ServerObject { get; set; }

		private void HandleException(LocalizedException e)
		{
			if (!this.MonitoringContext)
			{
				this.WriteError(e, ErrorCategory.OperationStopped, this, true);
				return;
			}
			this.monitoringData.Events.Add(new MonitoringEvent(TestMonitoringServiceBasicCmdletTask.CmdletMonitoringEventSource, 3006, EventTypeEnumeration.Error, e.ToString()));
		}

		private const string ServerParam = "Server";

		private const string MonitoringContextParam = "MonitoringContext";

		private const int FailedEventIdBase = 2000;

		private const int SuccessEventIdBase = 1000;

		internal const string MonitoringServiceBasicCmdlet = "MonitoringServiceBasicCmdlet";

		private MonitoringData monitoringData;

		public static readonly string CmdletMonitoringEventSource = "MSExchange Monitoring MonitoringServiceBasicCmdlet";

		public static readonly string PerformanceCounter = "MonitoringServiceBasicCmdlet Latency";

		public enum ScenarioId
		{
			PlaceHolderNoException = 1006,
			ExceptionThrown = 3006,
			AllTransactionsSucceeded = 3001
		}
	}
}
