using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Monitoring.MailFlowTestHelper;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Test", "Mailflow", SupportsShouldProcess = true, DefaultParameterSetName = "SourceServer")]
	public sealed class TestMailFlow : RecipientObjectActionTask<ServerIdParameter, Server>
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ValueFromPipeline = true, Position = 0, ParameterSetName = "TargetDatabase")]
		[Parameter(Mandatory = false, ValueFromPipeline = true, Position = 0, ParameterSetName = "SourceServer")]
		[Parameter(Mandatory = false, ValueFromPipeline = true, Position = 0, ParameterSetName = "TargetEmailAddress")]
		[Parameter(Mandatory = false, ValueFromPipeline = true, Position = 0, ParameterSetName = "TargetMailboxServer")]
		[Parameter(Mandatory = false, ValueFromPipeline = true, Position = 0, ParameterSetName = "AutoDiscoverTargetMailboxServer")]
		[Alias(new string[]
		{
			"SourceMailboxServer"
		})]
		public override ServerIdParameter Identity
		{
			get
			{
				return (ServerIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[ValidateRange(1, 2147483647)]
		[Parameter(Mandatory = false)]
		public int ExecutionTimeout
		{
			get
			{
				return (int)(base.Fields["ExecutionTimeout"] ?? 240);
			}
			set
			{
				base.Fields["ExecutionTimeout"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateRange(1, 2147483647)]
		public int ActiveDirectoryTimeout
		{
			get
			{
				return (int)(base.Fields["ActiveDirectoryTimeout"] ?? 15);
			}
			set
			{
				base.Fields["ActiveDirectoryTimeout"] = value;
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

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "TargetMailboxServer", ValueFromPipeline = false)]
		public ServerIdParameter TargetMailboxServer
		{
			get
			{
				return (ServerIdParameter)base.Fields["TargetMailboxServer"];
			}
			set
			{
				base.Fields["TargetMailboxServer"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "TargetDatabase", ValueFromPipeline = false)]
		public DatabaseIdParameter TargetDatabase
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["TargetDatabase"];
			}
			set
			{
				base.Fields["TargetDatabase"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "TargetEmailAddress", ValueFromPipeline = false)]
		public string TargetEmailAddress
		{
			get
			{
				return (string)base.Fields["TargetEmailAddress"];
			}
			set
			{
				base.Fields["TargetEmailAddress"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "TargetEmailAddress")]
		public string TargetEmailAddressDisplayName
		{
			get
			{
				return (string)base.Fields["TargetEmailAddressDisplayName"];
			}
			set
			{
				base.Fields["TargetEmailAddressDisplayName"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AutoDiscoverTargetMailboxServer")]
		public SwitchParameter AutoDiscoverTargetMailboxServer
		{
			get
			{
				return (SwitchParameter)(base.Fields["AutoDiscoverTargetMailboxServer"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["AutoDiscoverTargetMailboxServer"] = value;
			}
		}

		[ValidateRange(1, 2147483647)]
		[Parameter(Mandatory = false)]
		public int ErrorLatency
		{
			get
			{
				return (int)(base.Fields["ErrorLatency"] ?? 180);
			}
			set
			{
				base.Fields["ErrorLatency"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "CrossPremises", ValueFromPipeline = false)]
		public bool CrossPremises
		{
			get
			{
				return (bool)(base.Fields["CrossPremises"] ?? false);
			}
			set
			{
				base.Fields["CrossPremises"] = value;
			}
		}

		[ValidateRange(1, 2147483647)]
		[Parameter(Mandatory = false, ParameterSetName = "CrossPremises", ValueFromPipeline = false)]
		public int CrossPremisesPendingErrorCount
		{
			get
			{
				return (int)(base.Fields["CrossPremisesPendingErrorCount"] ?? 3);
			}
			set
			{
				base.Fields["CrossPremisesPendingErrorCount"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "CrossPremises", ValueFromPipeline = false)]
		public EnhancedTimeSpan CrossPremisesExpirationTimeout
		{
			get
			{
				return (EnhancedTimeSpan)(base.Fields["CrossPremisesExpirationTimeout"] ?? TestMailFlow.defaultCrossPremisesExpirationTimeout);
			}
			set
			{
				base.Fields["CrossPremisesExpirationTimeout"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestMailflow;
			}
		}

		internal IConfigurable GetAdDataObject<T>(IIdentityParameter id, IConfigDataProvider session, ObjectId rootId, LocalizedString? notFoundError, LocalizedString? multipleFoundError) where T : IConfigurable, new()
		{
			return base.GetDataObject<T>(id, session, rootId, notFoundError, multipleFoundError);
		}

		internal IEnumerable<T> GetAdDataObjects<T>(IIdentityParameter id, IConfigDataProvider session) where T : IConfigurable, new()
		{
			return base.GetDataObjects<T>(id, session, null);
		}

		protected override bool IsKnownException(Exception e)
		{
			return base.IsKnownException(e) || DataAccessHelper.IsDataAccessKnownException(e) || MonitoringHelper.IsKnownExceptionForMonitoring(e);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				if (this.CrossPremises)
				{
					this.helper = new CrossPremiseTestMailFlowHelper(this);
				}
				else
				{
					this.helper = new LegacyTestMailFlowHelper();
				}
				this.helper.SetTask(this);
				this.helper.InternalValidate();
			}
			finally
			{
				if (base.HasErrors && this.MonitoringContext)
				{
					this.helper.OutputMonitoringData();
				}
				TaskLogger.LogExit();
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				this.helper.InternalProcessRecord();
			}
			catch (MapiPermanentException ex)
			{
				this.helper.DiagnoseAndReportMapiException(ex);
			}
			catch (MapiRetryableException ex2)
			{
				this.helper.DiagnoseAndReportMapiException(ex2);
			}
			finally
			{
				if (this.MonitoringContext)
				{
					this.helper.OutputMonitoringData();
				}
				TaskLogger.LogExit();
			}
		}

		protected override void InternalStateReset()
		{
			if (this.helper != null)
			{
				this.helper.InternalStateReset();
			}
		}

		internal const int DefaultErrorLatencySeconds = 180;

		private const int DefaultExecutionTimeoutSeconds = 240;

		private const int DefaultADOperationsTimeoutInSeconds = 15;

		private const int DefaultCrossPremisesPendingErrorCount = 3;

		private static EnhancedTimeSpan defaultCrossPremisesExpirationTimeout = EnhancedTimeSpan.FromHours(48.0);

		private TestMailFlowHelper helper;
	}
}
