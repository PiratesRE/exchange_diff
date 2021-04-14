using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationSettings
	{
		internal VariantConfigurationSettings()
		{
			this.Add(new VariantConfigurationActiveMonitoringComponent());
			this.Add(new VariantConfigurationActiveSyncComponent());
			this.Add(new VariantConfigurationADComponent());
			this.Add(new VariantConfigurationAutodiscoverComponent());
			this.Add(new VariantConfigurationBoomerangComponent());
			this.Add(new VariantConfigurationCafeComponent());
			this.Add(new VariantConfigurationCalendarLoggingComponent());
			this.Add(new VariantConfigurationClientAccessRulesCommonComponent());
			this.Add(new VariantConfigurationCmdletInfraComponent());
			this.Add(new VariantConfigurationCompliancePolicyComponent());
			this.Add(new VariantConfigurationDataStorageComponent());
			this.Add(new VariantConfigurationDiagnosticsComponent());
			this.Add(new VariantConfigurationDiscoveryComponent());
			this.Add(new VariantConfigurationE4EComponent());
			this.Add(new VariantConfigurationEacComponent());
			this.Add(new VariantConfigurationEwsComponent());
			this.Add(new VariantConfigurationGlobalComponent());
			this.Add(new VariantConfigurationHighAvailabilityComponent());
			this.Add(new VariantConfigurationHolidayCalendarsComponent());
			this.Add(new VariantConfigurationHxComponent());
			this.Add(new VariantConfigurationImapComponent());
			this.Add(new VariantConfigurationInferenceComponent());
			this.Add(new VariantConfigurationIpaedComponent());
			this.Add(new VariantConfigurationMailboxAssistantsComponent());
			this.Add(new VariantConfigurationMailboxPlansComponent());
			this.Add(new VariantConfigurationMailboxTransportComponent());
			this.Add(new VariantConfigurationMalwareAgentComponent());
			this.Add(new VariantConfigurationMessageTrackingComponent());
			this.Add(new VariantConfigurationMexAgentsComponent());
			this.Add(new VariantConfigurationMrsComponent());
			this.Add(new VariantConfigurationNotificationBrokerServiceComponent());
			this.Add(new VariantConfigurationOABComponent());
			this.Add(new VariantConfigurationOfficeGraphComponent());
			this.Add(new VariantConfigurationOwaClientComponent());
			this.Add(new VariantConfigurationOwaClientServerComponent());
			this.Add(new VariantConfigurationOwaServerComponent());
			this.Add(new VariantConfigurationOwaDeploymentComponent());
			this.Add(new VariantConfigurationPopComponent());
			this.Add(new VariantConfigurationRpcClientAccessComponent());
			this.Add(new VariantConfigurationSearchComponent());
			this.Add(new VariantConfigurationSharedCacheComponent());
			this.Add(new VariantConfigurationSharedMailboxComponent());
			this.Add(new VariantConfigurationTestComponent());
			this.Add(new VariantConfigurationTest2Component());
			this.Add(new VariantConfigurationTransportComponent());
			this.Add(new VariantConfigurationUCCComponent());
			this.Add(new VariantConfigurationUMComponent());
			this.Add(new VariantConfigurationVariantConfigComponent());
			this.Add(new VariantConfigurationWorkingSetComponent());
			this.Add(new VariantConfigurationWorkloadManagementComponent());
		}

		public VariantConfigurationActiveMonitoringComponent ActiveMonitoring
		{
			get
			{
				return (VariantConfigurationActiveMonitoringComponent)this["ActiveMonitoring"];
			}
		}

		public VariantConfigurationActiveSyncComponent ActiveSync
		{
			get
			{
				return (VariantConfigurationActiveSyncComponent)this["ActiveSync"];
			}
		}

		public VariantConfigurationADComponent AD
		{
			get
			{
				return (VariantConfigurationADComponent)this["AD"];
			}
		}

		public VariantConfigurationAutodiscoverComponent Autodiscover
		{
			get
			{
				return (VariantConfigurationAutodiscoverComponent)this["Autodiscover"];
			}
		}

		public VariantConfigurationBoomerangComponent Boomerang
		{
			get
			{
				return (VariantConfigurationBoomerangComponent)this["Boomerang"];
			}
		}

		public VariantConfigurationCafeComponent Cafe
		{
			get
			{
				return (VariantConfigurationCafeComponent)this["Cafe"];
			}
		}

		public VariantConfigurationCalendarLoggingComponent CalendarLogging
		{
			get
			{
				return (VariantConfigurationCalendarLoggingComponent)this["CalendarLogging"];
			}
		}

		public VariantConfigurationClientAccessRulesCommonComponent ClientAccessRulesCommon
		{
			get
			{
				return (VariantConfigurationClientAccessRulesCommonComponent)this["ClientAccessRulesCommon"];
			}
		}

		public VariantConfigurationCmdletInfraComponent CmdletInfra
		{
			get
			{
				return (VariantConfigurationCmdletInfraComponent)this["CmdletInfra"];
			}
		}

		public VariantConfigurationCompliancePolicyComponent CompliancePolicy
		{
			get
			{
				return (VariantConfigurationCompliancePolicyComponent)this["CompliancePolicy"];
			}
		}

		public VariantConfigurationDataStorageComponent DataStorage
		{
			get
			{
				return (VariantConfigurationDataStorageComponent)this["DataStorage"];
			}
		}

		public VariantConfigurationDiagnosticsComponent Diagnostics
		{
			get
			{
				return (VariantConfigurationDiagnosticsComponent)this["Diagnostics"];
			}
		}

		public VariantConfigurationDiscoveryComponent Discovery
		{
			get
			{
				return (VariantConfigurationDiscoveryComponent)this["Discovery"];
			}
		}

		public VariantConfigurationE4EComponent E4E
		{
			get
			{
				return (VariantConfigurationE4EComponent)this["E4E"];
			}
		}

		public VariantConfigurationEacComponent Eac
		{
			get
			{
				return (VariantConfigurationEacComponent)this["Eac"];
			}
		}

		public VariantConfigurationEwsComponent Ews
		{
			get
			{
				return (VariantConfigurationEwsComponent)this["Ews"];
			}
		}

		public VariantConfigurationGlobalComponent Global
		{
			get
			{
				return (VariantConfigurationGlobalComponent)this["Global"];
			}
		}

		public VariantConfigurationHighAvailabilityComponent HighAvailability
		{
			get
			{
				return (VariantConfigurationHighAvailabilityComponent)this["HighAvailability"];
			}
		}

		public VariantConfigurationHolidayCalendarsComponent HolidayCalendars
		{
			get
			{
				return (VariantConfigurationHolidayCalendarsComponent)this["HolidayCalendars"];
			}
		}

		public VariantConfigurationHxComponent Hx
		{
			get
			{
				return (VariantConfigurationHxComponent)this["Hx"];
			}
		}

		public VariantConfigurationImapComponent Imap
		{
			get
			{
				return (VariantConfigurationImapComponent)this["Imap"];
			}
		}

		public VariantConfigurationInferenceComponent Inference
		{
			get
			{
				return (VariantConfigurationInferenceComponent)this["Inference"];
			}
		}

		public VariantConfigurationIpaedComponent Ipaed
		{
			get
			{
				return (VariantConfigurationIpaedComponent)this["Ipaed"];
			}
		}

		public VariantConfigurationMailboxAssistantsComponent MailboxAssistants
		{
			get
			{
				return (VariantConfigurationMailboxAssistantsComponent)this["MailboxAssistants"];
			}
		}

		public VariantConfigurationMailboxPlansComponent MailboxPlans
		{
			get
			{
				return (VariantConfigurationMailboxPlansComponent)this["MailboxPlans"];
			}
		}

		public VariantConfigurationMailboxTransportComponent MailboxTransport
		{
			get
			{
				return (VariantConfigurationMailboxTransportComponent)this["MailboxTransport"];
			}
		}

		public VariantConfigurationMalwareAgentComponent MalwareAgent
		{
			get
			{
				return (VariantConfigurationMalwareAgentComponent)this["MalwareAgent"];
			}
		}

		public VariantConfigurationMessageTrackingComponent MessageTracking
		{
			get
			{
				return (VariantConfigurationMessageTrackingComponent)this["MessageTracking"];
			}
		}

		public VariantConfigurationMexAgentsComponent MexAgents
		{
			get
			{
				return (VariantConfigurationMexAgentsComponent)this["MexAgents"];
			}
		}

		public VariantConfigurationMrsComponent Mrs
		{
			get
			{
				return (VariantConfigurationMrsComponent)this["Mrs"];
			}
		}

		public VariantConfigurationNotificationBrokerServiceComponent NotificationBrokerService
		{
			get
			{
				return (VariantConfigurationNotificationBrokerServiceComponent)this["NotificationBrokerService"];
			}
		}

		public VariantConfigurationOABComponent OAB
		{
			get
			{
				return (VariantConfigurationOABComponent)this["OAB"];
			}
		}

		public VariantConfigurationOfficeGraphComponent OfficeGraph
		{
			get
			{
				return (VariantConfigurationOfficeGraphComponent)this["OfficeGraph"];
			}
		}

		public VariantConfigurationOwaClientComponent OwaClient
		{
			get
			{
				return (VariantConfigurationOwaClientComponent)this["OwaClient"];
			}
		}

		public VariantConfigurationOwaClientServerComponent OwaClientServer
		{
			get
			{
				return (VariantConfigurationOwaClientServerComponent)this["OwaClientServer"];
			}
		}

		public VariantConfigurationOwaServerComponent OwaServer
		{
			get
			{
				return (VariantConfigurationOwaServerComponent)this["OwaServer"];
			}
		}

		public VariantConfigurationOwaDeploymentComponent OwaDeployment
		{
			get
			{
				return (VariantConfigurationOwaDeploymentComponent)this["OwaDeployment"];
			}
		}

		public VariantConfigurationPopComponent Pop
		{
			get
			{
				return (VariantConfigurationPopComponent)this["Pop"];
			}
		}

		public VariantConfigurationRpcClientAccessComponent RpcClientAccess
		{
			get
			{
				return (VariantConfigurationRpcClientAccessComponent)this["RpcClientAccess"];
			}
		}

		public VariantConfigurationSearchComponent Search
		{
			get
			{
				return (VariantConfigurationSearchComponent)this["Search"];
			}
		}

		public VariantConfigurationSharedCacheComponent SharedCache
		{
			get
			{
				return (VariantConfigurationSharedCacheComponent)this["SharedCache"];
			}
		}

		public VariantConfigurationSharedMailboxComponent SharedMailbox
		{
			get
			{
				return (VariantConfigurationSharedMailboxComponent)this["SharedMailbox"];
			}
		}

		public VariantConfigurationTestComponent Test
		{
			get
			{
				return (VariantConfigurationTestComponent)this["Test"];
			}
		}

		public VariantConfigurationTest2Component Test2
		{
			get
			{
				return (VariantConfigurationTest2Component)this["Test2"];
			}
		}

		public VariantConfigurationTransportComponent Transport
		{
			get
			{
				return (VariantConfigurationTransportComponent)this["Transport"];
			}
		}

		public VariantConfigurationUCCComponent UCC
		{
			get
			{
				return (VariantConfigurationUCCComponent)this["UCC"];
			}
		}

		public VariantConfigurationUMComponent UM
		{
			get
			{
				return (VariantConfigurationUMComponent)this["UM"];
			}
		}

		public VariantConfigurationVariantConfigComponent VariantConfig
		{
			get
			{
				return (VariantConfigurationVariantConfigComponent)this["VariantConfig"];
			}
		}

		public VariantConfigurationWorkingSetComponent WorkingSet
		{
			get
			{
				return (VariantConfigurationWorkingSetComponent)this["WorkingSet"];
			}
		}

		public VariantConfigurationWorkloadManagementComponent WorkloadManagement
		{
			get
			{
				return (VariantConfigurationWorkloadManagementComponent)this["WorkloadManagement"];
			}
		}

		public VariantConfigurationComponent this[string name]
		{
			get
			{
				return this.components[name];
			}
		}

		public IEnumerable<string> GetComponents(bool includeInternal)
		{
			if (includeInternal)
			{
				return this.components.Keys;
			}
			return from component in this.components.Keys
			where this[component].GetSections(includeInternal).Any<string>()
			select component;
		}

		public bool Contains(string name, bool includeInternal)
		{
			return this.components.ContainsKey(name) && (includeInternal || this[name].GetSections(false).Any<string>());
		}

		private void Add(VariantConfigurationComponent component)
		{
			this.components.Add(component.ComponentName, component);
		}

		private Dictionary<string, VariantConfigurationComponent> components = new Dictionary<string, VariantConfigurationComponent>(StringComparer.OrdinalIgnoreCase);
	}
}
