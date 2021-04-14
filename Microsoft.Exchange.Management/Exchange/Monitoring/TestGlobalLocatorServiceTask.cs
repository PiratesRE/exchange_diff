using System;
using System.Diagnostics;
using System.Management.Automation;
using System.ServiceModel;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.GlobalLocatorService;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Test", "GlobalLocatorService", SupportsShouldProcess = true)]
	public sealed class TestGlobalLocatorServiceTask : Task
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

		internal ITopologyConfigurationSession SystemConfigurationSession
		{
			get
			{
				if (this.systemConfigurationSession == null)
				{
					this.systemConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 190, "SystemConfigurationSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\ActiveDirectory\\TestGlobalLocatorServiceTask.cs");
				}
				return this.systemConfigurationSession;
			}
		}

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
				GlobalLocatorServiceOutcome sendToPipeline = new GlobalLocatorServiceOutcome(new ServerIdParameter().ToString());
				this.PerformGlobalLocatorServiceTest(ref sendToPipeline);
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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestGlobalLocatorServiceIdentity;
			}
		}

		private void PerformGlobalLocatorServiceTest(ref GlobalLocatorServiceOutcome result)
		{
			bool flag = true;
			string error = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			GlobalLocatorServiceError globalLocatorServiceError = GlobalLocatorServiceError.None;
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				SmtpAddress? multiTenantAutomatedTaskUser = TestConnectivityCredentialsManager.GetMultiTenantAutomatedTaskUser(this, this.SystemConfigurationSession, this.SystemConfigurationSession.GetLocalSite(), DatacenterUserType.EDU);
				if (multiTenantAutomatedTaskUser == null)
				{
					throw new MailboxNotFoundException(new MailboxIdParameter(), null);
				}
				IGlobalLocatorServiceReader globalLocatorServiceReader = LocatorServiceClientReader.Create(GlsCallerId.Exchange);
				FindDomainResult findDomainResult = globalLocatorServiceReader.FindDomain(new SmtpDomain(multiTenantAutomatedTaskUser.Value.Domain), GlsDirectorySession.AllExoDomainProperties, GlsDirectorySession.AllExoTenantProperties);
				if (string.IsNullOrEmpty(findDomainResult.Domain))
				{
					flag = false;
					error = "Domain not found";
				}
				else
				{
					stringBuilder.AppendLine("Domain Found");
				}
			}
			catch (CommunicationException ex)
			{
				flag = false;
				error = ex.Message;
				globalLocatorServiceError = GlobalLocatorServiceError.CommunicationException;
			}
			catch (Exception ex2)
			{
				flag = false;
				error = ex2.Message;
				globalLocatorServiceError = GlobalLocatorServiceError.OtherException;
			}
			stopwatch.Stop();
			result.Update(flag ? GlobalLocatorServiceResultEnum.Success : GlobalLocatorServiceResultEnum.Failure, stopwatch.Elapsed, error, stringBuilder.ToString());
			if (this.MonitoringContext)
			{
				this.monitoringData.Events.Add(new MonitoringEvent(TestGlobalLocatorServiceTask.CmdletMonitoringEventSource, flag ? 1000 : 2000, flag ? EventTypeEnumeration.Success : EventTypeEnumeration.Error, flag ? Strings.GlobalLocatorServiceSuccess : (Strings.GlobalLocatorServiceFailed(error) + " " + globalLocatorServiceError)));
			}
		}

		private bool IsExplicitlySet(string param)
		{
			return base.Fields.Contains(param);
		}

		private TimeSpan TotalLatency { get; set; }

		private Server ServerObject { get; set; }

		private void HandleException(LocalizedException e)
		{
			if (!this.MonitoringContext)
			{
				this.WriteError(e, ErrorCategory.OperationStopped, this, true);
				return;
			}
			this.monitoringData.Events.Add(new MonitoringEvent(TestGlobalLocatorServiceTask.CmdletMonitoringEventSource, 3006, EventTypeEnumeration.Error, Strings.LiveIdConnectivityExceptionThrown(e.ToString())));
		}

		private const string MonitoringContextParam = "MonitoringContext";

		private const int FailedEventIdBase = 2000;

		private const int SuccessEventIdBase = 1000;

		internal const string GlobalLocatorService = "GlobalLocatorService";

		private MonitoringData monitoringData;

		private ITopologyConfigurationSession systemConfigurationSession;

		public static readonly string CmdletMonitoringEventSource = "MSExchange Monitoring GlobalLocatorService";

		public static readonly string PerformanceCounter = "GlobalLocatorService Latency";

		public enum ScenarioId
		{
			PlaceHolderNoException = 1006,
			ExceptionThrown = 3006,
			AllTransactionsSucceeded = 3001
		}
	}
}
