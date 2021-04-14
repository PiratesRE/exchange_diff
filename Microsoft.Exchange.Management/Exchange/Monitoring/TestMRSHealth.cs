using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Test", "MRSHealth", SupportsShouldProcess = true)]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class TestMRSHealth : SystemConfigurationObjectActionTask<ServerIdParameter, Server>
	{
		public MonitoringData MonitoringData { get; private set; }

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		[Alias(new string[]
		{
			"Server"
		})]
		public override ServerIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MonitoringContext
		{
			get
			{
				return (bool)(base.Fields["MonitoringContext"] ?? false);
			}
			set
			{
				base.Fields["MonitoringContext"] = value;
			}
		}

		[ValidateRange(1, 2147483647)]
		[Parameter(Mandatory = false)]
		public int MaxQueueScanAgeSeconds
		{
			get
			{
				return (int)(base.Fields["MaxQueueScanAgeSeconds"] ?? 3600);
			}
			set
			{
				base.Fields["MaxQueueScanAgeSeconds"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNull]
		public Fqdn MRSProxyServer
		{
			get
			{
				return (Fqdn)base.Fields["MRSProxyServer"];
			}
			set
			{
				base.Fields["MRSProxyServer"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNull]
		public PSCredential MRSProxyCredentials
		{
			get
			{
				return (PSCredential)base.Fields["MRSProxyCredentials"];
			}
			set
			{
				base.Fields["MRSProxyCredentials"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestMRSHealth(this.Identity.ToString());
			}
		}

		public void WriteErrorAndMonitoringEvent(Exception exception, ErrorCategory errorCategory, int eventId)
		{
			this.MonitoringData.Events.Add(new MonitoringEvent("MSExchange Monitoring MRSHealth", eventId, EventTypeEnumeration.Error, CommonUtils.FullExceptionMessage(exception, true)));
			base.WriteError(exception, errorCategory, this.Identity);
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || DataAccessHelper.IsDataAccessKnownException(exception) || MonitoringHelper.IsKnownExceptionForMonitoring(exception);
		}

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
			this.MonitoringData = new MonitoringData();
			this.serverName = null;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				if (this.Identity == null)
				{
					this.Identity = ServerIdParameter.Parse(Environment.MachineName);
				}
				base.InternalValidate();
				this.serverName = this.DataObject.Name;
				if (!this.DataObject.IsClientAccessServer)
				{
					this.WriteErrorAndMonitoringEvent(this.DataObject.GetServerRoleError(ServerRole.ClientAccess), ErrorCategory.InvalidOperation, 1001);
				}
				if (!this.DataObject.IsE14OrLater)
				{
					this.WriteErrorAndMonitoringEvent(new ServerConfigurationException(this.serverName, Strings.ErrorServerNotE14OrLater(this.serverName)), ErrorCategory.InvalidOperation, 1001);
				}
			}
			finally
			{
				if (base.HasErrors && this.MonitoringContext)
				{
					base.WriteObject(this.MonitoringData);
				}
				TaskLogger.LogExit();
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				MRSHealthCheckOutcome mrshealthCheckOutcome = MRSHealth.VerifyServiceIsUp(this.serverName, this.DataObject.Fqdn, this);
				base.WriteObject(mrshealthCheckOutcome);
				bool flag = mrshealthCheckOutcome.Passed;
				if (flag)
				{
					mrshealthCheckOutcome = MRSHealth.VerifyServiceIsRespondingToRPCPing(this.serverName, this);
					base.WriteObject(mrshealthCheckOutcome);
					if (mrshealthCheckOutcome.Passed)
					{
						try
						{
							mrshealthCheckOutcome = MRSHealth.VerifyMRSProxyIsRespondingToWCFPing(this.serverName, this.MRSProxyServer, (this.MRSProxyCredentials == null) ? null : this.MRSProxyCredentials.GetNetworkCredential(), this);
						}
						catch (UnsupportedRemoteServerVersionWithOperationPermanentException)
						{
							mrshealthCheckOutcome = new MRSHealthCheckOutcome(this.serverName, MRSHealthCheckId.MRSProxyPingCheck, true, Strings.MRSProxyPingSkipped(this.serverName));
						}
						base.WriteObject(mrshealthCheckOutcome);
					}
					flag = mrshealthCheckOutcome.Passed;
					mrshealthCheckOutcome = MRSHealth.VerifyServiceIsScanningForJobs(this.serverName, (long)this.MaxQueueScanAgeSeconds, this);
					base.WriteObject(mrshealthCheckOutcome);
					flag &= mrshealthCheckOutcome.Passed;
				}
				if (flag)
				{
					this.MonitoringData.Events.Add(new MonitoringEvent("MSExchange Monitoring MRSHealth", 1000, EventTypeEnumeration.Success, Strings.MRSHealthPassed));
				}
			}
			catch (ConfigurationSettingsException exception)
			{
				this.WriteErrorAndMonitoringEvent(exception, ErrorCategory.InvalidOperation, 1001);
			}
			finally
			{
				if (this.MonitoringContext)
				{
					base.WriteObject(this.MonitoringData);
				}
				TaskLogger.LogExit();
			}
		}

		public const string CmdletMonitoringEventSource = "MSExchange Monitoring MRSHealth";

		public const string MonitoringScanElapsedTimePerfCounter = "Last Scan Age (secs)";

		private const string CmdletNoun = "MRSHealth";

		private string serverName;

		public static class EventId
		{
			internal const int Success = 1000;

			internal const int ServiceOperationError = 1001;

			internal const int ServiceBadResponse = 1002;

			internal const int ServiceNotScanningForJobs = 1003;

			internal const int MRSProxyBadResponse = 1004;
		}
	}
}
