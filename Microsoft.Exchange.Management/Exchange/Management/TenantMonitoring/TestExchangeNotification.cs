using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Monitoring;

namespace Microsoft.Exchange.Management.TenantMonitoring
{
	[Cmdlet("Test", "ExchangeNotification", SupportsShouldProcess = true)]
	public sealed class TestExchangeNotification : DataAccessTask<ExchangeConfigurationUnit>
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true)]
		[ValidateNotNullOrEmpty]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		[ValidateCount(0, 100)]
		[Parameter(Mandatory = false)]
		public string[] InsertionStrings
		{
			get
			{
				string[] result;
				if ((result = (string[])base.Fields["InsertionStrings"]) == null)
				{
					result = new string[]
					{
						string.Empty,
						string.Empty
					};
				}
				return result;
			}
			set
			{
				base.Fields["InsertionStrings"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PeriodicKey
		{
			get
			{
				return ((string)base.Fields["PeriodicKey"]) ?? string.Empty;
			}
			set
			{
				base.Fields["PeriodicKey"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			OrganizationId organizationId = this.FindOrganizationId();
			ExEventLog exEventLog = new ExEventLog(TestExchangeNotification.ComponentId, "MSExchange Common");
			exEventLog.LogEvent(organizationId, CommonEventLogConstants.Tuple_TenantMonitoringTestEvent, this.PeriodicKey, this.InsertionStrings);
			base.WriteObject(new MonitoringData
			{
				Events = 
				{
					this.CreateSuccessEvent()
				}
			});
		}

		protected override IConfigDataProvider CreateSession()
		{
			ADObjectId rootOrgContainerIdForLocalForest = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(rootOrgContainerIdForLocalForest, base.CurrentOrganizationId ?? base.ExecutingUserOrganizationId, base.ExecutingUserOrganizationId, true);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 133, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\TenantMonitoring\\TestExchangeNotification.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = true;
			tenantOrTopologyConfigurationSession.UseGlobalCatalog = false;
			return tenantOrTopologyConfigurationSession;
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.TenantNotificationTestConfirmationPrompt(this.Organization.ToString());
			}
		}

		private OrganizationId FindOrganizationId()
		{
			ExchangeConfigurationUnit exchangeConfigurationUnit = (ExchangeConfigurationUnit)base.GetDataObject<ExchangeConfigurationUnit>(this.Organization, base.DataSession, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
			if (exchangeConfigurationUnit.OrganizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				this.WriteError(new TenantNotificationTestFirstOrgNotSupportedException(), (ErrorCategory)1000, this, true);
				return null;
			}
			return exchangeConfigurationUnit.OrganizationId;
		}

		private MonitoringEvent CreateSuccessEvent()
		{
			string arg = string.Empty;
			if (this.InsertionStrings != null && this.InsertionStrings.Length > 0)
			{
				arg = string.Join(", ", this.InsertionStrings);
			}
			return new MonitoringEvent("MSExchange Monitoring ExchangeNotification", 100, EventTypeEnumeration.Success, string.Format("Test event with periodic-key='{0}' and insertion-strings='{1}' was successfully logged.", this.PeriodicKey, arg));
		}

		private const int MonitoringEventSuccessId = 100;

		private const string MonitoringEventSource = "MSExchange Monitoring ExchangeNotification";

		private static readonly Guid ComponentId = new Guid("2f07db75-cff9-4e7e-9195-e8c1991aa251");
	}
}
