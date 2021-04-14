using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Monitoring;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Set", "UMConnectivityTestCredentials")]
	public class SetUMConnectivityTestCredentials : Task
	{
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

		[Parameter]
		public Fqdn DomainController
		{
			get
			{
				return (Fqdn)base.Fields["DomainController"];
			}
			set
			{
				base.Fields["DomainController"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			this.DoOwnValidate();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 148, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\um\\setUMConnectivityTestCredentials.cs");
			ADSite localSite = this.configurationSession.GetLocalSite();
			UmConnectivityCredentialsHelper umConnectivityCredentialsHelper = new UmConnectivityCredentialsHelper(localSite, this.serv);
			umConnectivityCredentialsHelper.InitializeUser(false);
			if (!umConnectivityCredentialsHelper.IsUserFound || !umConnectivityCredentialsHelper.IsUserUMEnabled)
			{
				this.HandleSuccess();
			}
			if (umConnectivityCredentialsHelper.IsExchangePrincipalFound)
			{
				if (umConnectivityCredentialsHelper.SuccessfullyGotPin)
				{
					this.SaveThePin(umConnectivityCredentialsHelper);
					this.HandleSuccess();
				}
				else
				{
					SUC_CouldnotRetreivePasswd localizedException = new SUC_CouldnotRetreivePasswd();
					this.HandleError(localizedException, SetUMConnectivityTestCredentials.EventId.ADError, "MSExchange Monitoring UMConnectivityTestCredentials");
				}
			}
			else
			{
				SUC_ExchangePrincipalError localizedException2 = new SUC_ExchangePrincipalError();
				this.HandleError(localizedException2, SetUMConnectivityTestCredentials.EventId.ADError, "MSExchange Monitoring UMConnectivityTestCredentials");
			}
			TaskLogger.LogExit();
		}

		private void DoOwnValidate()
		{
			try
			{
				IConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 198, "DoOwnValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\um\\setUMConnectivityTestCredentials.cs");
				this.serv = Utility.GetServerFromName(Utils.GetLocalHostName(), session);
				if (this.serv == null)
				{
					ADError localizedException = new ADError();
					this.HandleError(localizedException, SetUMConnectivityTestCredentials.EventId.ADError, "MSExchange Monitoring UMConnectivityTestCredentials");
				}
			}
			catch (ADTransientException innerException)
			{
				ADError localizedException2 = new ADError(innerException);
				this.HandleError(localizedException2, SetUMConnectivityTestCredentials.EventId.ADError, "MSExchange Monitoring UMConnectivityTestCredentials");
			}
			catch (DataSourceOperationException innerException2)
			{
				ADError localizedException3 = new ADError(innerException2);
				this.HandleError(localizedException3, SetUMConnectivityTestCredentials.EventId.ADError, "MSExchange Monitoring UMConnectivityTestCredentials");
			}
			catch (DataValidationException innerException3)
			{
				ADError localizedException4 = new ADError(innerException3);
				this.HandleError(localizedException4, SetUMConnectivityTestCredentials.EventId.ADError, "MSExchange Monitoring UMConnectivityTestCredentials");
			}
			if (this.serv != null)
			{
				if (!UmConnectivityCredentialsHelper.IsMailboxServer(this.serv))
				{
					SUC_NotMailboxServer localizedException5 = new SUC_NotMailboxServer();
					this.HandleError(localizedException5, SetUMConnectivityTestCredentials.EventId.ADError, "MSExchange Monitoring UMConnectivityTestCredentials");
					return;
				}
			}
			else
			{
				ADError localizedException6 = new ADError();
				this.HandleError(localizedException6, SetUMConnectivityTestCredentials.EventId.ADError, "MSExchange Monitoring UMConnectivityTestCredentials");
			}
		}

		private void SaveThePin(UmConnectivityCredentialsHelper help)
		{
			LocalizedException ex = UmConnectivityCredentialsHelper.SaveUMPin(help.User, help.UMPin);
			if (ex != null)
			{
				this.HandleError(ex, SetUMConnectivityTestCredentials.EventId.SavePinFailure, "MSExchange Monitoring UMConnectivityTestCredentials");
			}
		}

		private void HandleError(LocalizedException localizedException, SetUMConnectivityTestCredentials.EventId id, string eventSource)
		{
			this.WriteErrorAndMonitoringEvent(localizedException, ErrorCategory.NotSpecified, null, (int)id, eventSource);
			if (this.MonitoringContext)
			{
				base.WriteObject(this.monitoringData);
			}
		}

		private void HandleSuccess()
		{
			this.monitoringData.Events.Add(new MonitoringEvent("MSExchange Monitoring UMConnectivityTestCredentials", 1000, EventTypeEnumeration.Success, Strings.OperationSuccessful));
			if (this.MonitoringContext)
			{
				base.WriteObject(this.monitoringData);
			}
		}

		private void WriteErrorAndMonitoringEvent(LocalizedException localizedException, ErrorCategory errorCategory, object target, int eventId, string eventSource)
		{
			this.monitoringData.Events.Add(new MonitoringEvent(eventSource, eventId, EventTypeEnumeration.Error, localizedException.LocalizedString));
			base.WriteError(localizedException, errorCategory, target);
		}

		private const string TaskMonitoringEventSource = "MSExchange Monitoring UMConnectivityTestCredentials";

		private MonitoringData monitoringData = new MonitoringData();

		private Server serv;

		private ITopologyConfigurationSession configurationSession;

		private enum EventId
		{
			OperationSuccessFul = 1000,
			ADError,
			SavePinFailure
		}
	}
}
