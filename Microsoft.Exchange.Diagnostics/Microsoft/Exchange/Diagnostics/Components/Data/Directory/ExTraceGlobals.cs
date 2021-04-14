using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.Data.Directory
{
	public static class ExTraceGlobals
	{
		public static Trace TopologyProviderTracer
		{
			get
			{
				if (ExTraceGlobals.topologyProviderTracer == null)
				{
					ExTraceGlobals.topologyProviderTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.topologyProviderTracer;
			}
		}

		public static Trace ADTopologyTracer
		{
			get
			{
				if (ExTraceGlobals.aDTopologyTracer == null)
				{
					ExTraceGlobals.aDTopologyTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.aDTopologyTracer;
			}
		}

		public static Trace ConnectionTracer
		{
			get
			{
				if (ExTraceGlobals.connectionTracer == null)
				{
					ExTraceGlobals.connectionTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.connectionTracer;
			}
		}

		public static Trace ConnectionDetailsTracer
		{
			get
			{
				if (ExTraceGlobals.connectionDetailsTracer == null)
				{
					ExTraceGlobals.connectionDetailsTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.connectionDetailsTracer;
			}
		}

		public static Trace GetConnectionTracer
		{
			get
			{
				if (ExTraceGlobals.getConnectionTracer == null)
				{
					ExTraceGlobals.getConnectionTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.getConnectionTracer;
			}
		}

		public static Trace ADFindTracer
		{
			get
			{
				if (ExTraceGlobals.aDFindTracer == null)
				{
					ExTraceGlobals.aDFindTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.aDFindTracer;
			}
		}

		public static Trace ADReadTracer
		{
			get
			{
				if (ExTraceGlobals.aDReadTracer == null)
				{
					ExTraceGlobals.aDReadTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.aDReadTracer;
			}
		}

		public static Trace ADReadDetailsTracer
		{
			get
			{
				if (ExTraceGlobals.aDReadDetailsTracer == null)
				{
					ExTraceGlobals.aDReadDetailsTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.aDReadDetailsTracer;
			}
		}

		public static Trace ADSaveTracer
		{
			get
			{
				if (ExTraceGlobals.aDSaveTracer == null)
				{
					ExTraceGlobals.aDSaveTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.aDSaveTracer;
			}
		}

		public static Trace ADSaveDetailsTracer
		{
			get
			{
				if (ExTraceGlobals.aDSaveDetailsTracer == null)
				{
					ExTraceGlobals.aDSaveDetailsTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.aDSaveDetailsTracer;
			}
		}

		public static Trace ADDeleteTracer
		{
			get
			{
				if (ExTraceGlobals.aDDeleteTracer == null)
				{
					ExTraceGlobals.aDDeleteTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.aDDeleteTracer;
			}
		}

		public static Trace ValidationTracer
		{
			get
			{
				if (ExTraceGlobals.validationTracer == null)
				{
					ExTraceGlobals.validationTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.validationTracer;
			}
		}

		public static Trace ADNotificationsTracer
		{
			get
			{
				if (ExTraceGlobals.aDNotificationsTracer == null)
				{
					ExTraceGlobals.aDNotificationsTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.aDNotificationsTracer;
			}
		}

		public static Trace DirectoryExceptionTracer
		{
			get
			{
				if (ExTraceGlobals.directoryExceptionTracer == null)
				{
					ExTraceGlobals.directoryExceptionTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.directoryExceptionTracer;
			}
		}

		public static Trace LdapFilterBuilderTracer
		{
			get
			{
				if (ExTraceGlobals.ldapFilterBuilderTracer == null)
				{
					ExTraceGlobals.ldapFilterBuilderTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.ldapFilterBuilderTracer;
			}
		}

		public static Trace ADPropertyRequestTracer
		{
			get
			{
				if (ExTraceGlobals.aDPropertyRequestTracer == null)
				{
					ExTraceGlobals.aDPropertyRequestTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.aDPropertyRequestTracer;
			}
		}

		public static Trace ADObjectTracer
		{
			get
			{
				if (ExTraceGlobals.aDObjectTracer == null)
				{
					ExTraceGlobals.aDObjectTracer = new Trace(ExTraceGlobals.componentGuid, 17);
				}
				return ExTraceGlobals.aDObjectTracer;
			}
		}

		public static Trace ContentTypeMappingTracer
		{
			get
			{
				if (ExTraceGlobals.contentTypeMappingTracer == null)
				{
					ExTraceGlobals.contentTypeMappingTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.contentTypeMappingTracer;
			}
		}

		public static Trace LcidMapperTracer
		{
			get
			{
				if (ExTraceGlobals.lcidMapperTracer == null)
				{
					ExTraceGlobals.lcidMapperTracer = new Trace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.lcidMapperTracer;
			}
		}

		public static Trace RecipientUpdateServiceTracer
		{
			get
			{
				if (ExTraceGlobals.recipientUpdateServiceTracer == null)
				{
					ExTraceGlobals.recipientUpdateServiceTracer = new Trace(ExTraceGlobals.componentGuid, 21);
				}
				return ExTraceGlobals.recipientUpdateServiceTracer;
			}
		}

		public static Trace UMAutoAttendantTracer
		{
			get
			{
				if (ExTraceGlobals.uMAutoAttendantTracer == null)
				{
					ExTraceGlobals.uMAutoAttendantTracer = new Trace(ExTraceGlobals.componentGuid, 22);
				}
				return ExTraceGlobals.uMAutoAttendantTracer;
			}
		}

		public static Trace ExchangeTopologyTracer
		{
			get
			{
				if (ExTraceGlobals.exchangeTopologyTracer == null)
				{
					ExTraceGlobals.exchangeTopologyTracer = new Trace(ExTraceGlobals.componentGuid, 23);
				}
				return ExTraceGlobals.exchangeTopologyTracer;
			}
		}

		public static Trace PerfCountersTracer
		{
			get
			{
				if (ExTraceGlobals.perfCountersTracer == null)
				{
					ExTraceGlobals.perfCountersTracer = new Trace(ExTraceGlobals.componentGuid, 24);
				}
				return ExTraceGlobals.perfCountersTracer;
			}
		}

		public static Trace ClientThrottlingTracer
		{
			get
			{
				if (ExTraceGlobals.clientThrottlingTracer == null)
				{
					ExTraceGlobals.clientThrottlingTracer = new Trace(ExTraceGlobals.componentGuid, 25);
				}
				return ExTraceGlobals.clientThrottlingTracer;
			}
		}

		public static Trace ServerSettingsProviderTracer
		{
			get
			{
				if (ExTraceGlobals.serverSettingsProviderTracer == null)
				{
					ExTraceGlobals.serverSettingsProviderTracer = new Trace(ExTraceGlobals.componentGuid, 27);
				}
				return ExTraceGlobals.serverSettingsProviderTracer;
			}
		}

		public static Trace RetryManagerTracer
		{
			get
			{
				if (ExTraceGlobals.retryManagerTracer == null)
				{
					ExTraceGlobals.retryManagerTracer = new Trace(ExTraceGlobals.componentGuid, 29);
				}
				return ExTraceGlobals.retryManagerTracer;
			}
		}

		public static Trace SystemConfigurationCacheTracer
		{
			get
			{
				if (ExTraceGlobals.systemConfigurationCacheTracer == null)
				{
					ExTraceGlobals.systemConfigurationCacheTracer = new Trace(ExTraceGlobals.componentGuid, 30);
				}
				return ExTraceGlobals.systemConfigurationCacheTracer;
			}
		}

		public static Trace FederatedIdentityTracer
		{
			get
			{
				if (ExTraceGlobals.federatedIdentityTracer == null)
				{
					ExTraceGlobals.federatedIdentityTracer = new Trace(ExTraceGlobals.componentGuid, 31);
				}
				return ExTraceGlobals.federatedIdentityTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 32);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace AddressListTracer
		{
			get
			{
				if (ExTraceGlobals.addressListTracer == null)
				{
					ExTraceGlobals.addressListTracer = new Trace(ExTraceGlobals.componentGuid, 33);
				}
				return ExTraceGlobals.addressListTracer;
			}
		}

		public static Trace NspiRpcClientConnectionTracer
		{
			get
			{
				if (ExTraceGlobals.nspiRpcClientConnectionTracer == null)
				{
					ExTraceGlobals.nspiRpcClientConnectionTracer = new Trace(ExTraceGlobals.componentGuid, 34);
				}
				return ExTraceGlobals.nspiRpcClientConnectionTracer;
			}
		}

		public static Trace ScopeVerificationTracer
		{
			get
			{
				if (ExTraceGlobals.scopeVerificationTracer == null)
				{
					ExTraceGlobals.scopeVerificationTracer = new Trace(ExTraceGlobals.componentGuid, 35);
				}
				return ExTraceGlobals.scopeVerificationTracer;
			}
		}

		public static Trace SchemaInitializationTracer
		{
			get
			{
				if (ExTraceGlobals.schemaInitializationTracer == null)
				{
					ExTraceGlobals.schemaInitializationTracer = new Trace(ExTraceGlobals.componentGuid, 36);
				}
				return ExTraceGlobals.schemaInitializationTracer;
			}
		}

		public static Trace IsMemberOfResolverTracer
		{
			get
			{
				if (ExTraceGlobals.isMemberOfResolverTracer == null)
				{
					ExTraceGlobals.isMemberOfResolverTracer = new Trace(ExTraceGlobals.componentGuid, 37);
				}
				return ExTraceGlobals.isMemberOfResolverTracer;
			}
		}

		public static Trace OwaSegmentationTracer
		{
			get
			{
				if (ExTraceGlobals.owaSegmentationTracer == null)
				{
					ExTraceGlobals.owaSegmentationTracer = new Trace(ExTraceGlobals.componentGuid, 39);
				}
				return ExTraceGlobals.owaSegmentationTracer;
			}
		}

		public static Trace ADPerformanceTracer
		{
			get
			{
				if (ExTraceGlobals.aDPerformanceTracer == null)
				{
					ExTraceGlobals.aDPerformanceTracer = new Trace(ExTraceGlobals.componentGuid, 40);
				}
				return ExTraceGlobals.aDPerformanceTracer;
			}
		}

		public static Trace ResourceHealthManagerTracer
		{
			get
			{
				if (ExTraceGlobals.resourceHealthManagerTracer == null)
				{
					ExTraceGlobals.resourceHealthManagerTracer = new Trace(ExTraceGlobals.componentGuid, 41);
				}
				return ExTraceGlobals.resourceHealthManagerTracer;
			}
		}

		public static Trace BudgetDelayTracer
		{
			get
			{
				if (ExTraceGlobals.budgetDelayTracer == null)
				{
					ExTraceGlobals.budgetDelayTracer = new Trace(ExTraceGlobals.componentGuid, 42);
				}
				return ExTraceGlobals.budgetDelayTracer;
			}
		}

		public static Trace GLSTracer
		{
			get
			{
				if (ExTraceGlobals.gLSTracer == null)
				{
					ExTraceGlobals.gLSTracer = new Trace(ExTraceGlobals.componentGuid, 43);
				}
				return ExTraceGlobals.gLSTracer;
			}
		}

		public static Trace MServTracer
		{
			get
			{
				if (ExTraceGlobals.mServTracer == null)
				{
					ExTraceGlobals.mServTracer = new Trace(ExTraceGlobals.componentGuid, 44);
				}
				return ExTraceGlobals.mServTracer;
			}
		}

		public static Trace TenantRelocationTracer
		{
			get
			{
				if (ExTraceGlobals.tenantRelocationTracer == null)
				{
					ExTraceGlobals.tenantRelocationTracer = new Trace(ExTraceGlobals.componentGuid, 45);
				}
				return ExTraceGlobals.tenantRelocationTracer;
			}
		}

		public static Trace StateManagementTracer
		{
			get
			{
				if (ExTraceGlobals.stateManagementTracer == null)
				{
					ExTraceGlobals.stateManagementTracer = new Trace(ExTraceGlobals.componentGuid, 46);
				}
				return ExTraceGlobals.stateManagementTracer;
			}
		}

		public static Trace ServerComponentStateManagerTracer
		{
			get
			{
				if (ExTraceGlobals.serverComponentStateManagerTracer == null)
				{
					ExTraceGlobals.serverComponentStateManagerTracer = new Trace(ExTraceGlobals.componentGuid, 48);
				}
				return ExTraceGlobals.serverComponentStateManagerTracer;
			}
		}

		public static Trace SessionSettingsTracer
		{
			get
			{
				if (ExTraceGlobals.sessionSettingsTracer == null)
				{
					ExTraceGlobals.sessionSettingsTracer = new Trace(ExTraceGlobals.componentGuid, 49);
				}
				return ExTraceGlobals.sessionSettingsTracer;
			}
		}

		public static Trace ADConfigLoaderTracer
		{
			get
			{
				if (ExTraceGlobals.aDConfigLoaderTracer == null)
				{
					ExTraceGlobals.aDConfigLoaderTracer = new Trace(ExTraceGlobals.componentGuid, 50);
				}
				return ExTraceGlobals.aDConfigLoaderTracer;
			}
		}

		public static Trace SlimTenantTracer
		{
			get
			{
				if (ExTraceGlobals.slimTenantTracer == null)
				{
					ExTraceGlobals.slimTenantTracer = new Trace(ExTraceGlobals.componentGuid, 51);
				}
				return ExTraceGlobals.slimTenantTracer;
			}
		}

		public static Trace TenantUpgradeServiceletTracer
		{
			get
			{
				if (ExTraceGlobals.tenantUpgradeServiceletTracer == null)
				{
					ExTraceGlobals.tenantUpgradeServiceletTracer = new Trace(ExTraceGlobals.componentGuid, 52);
				}
				return ExTraceGlobals.tenantUpgradeServiceletTracer;
			}
		}

		public static Trace DirectoryTasksTracer
		{
			get
			{
				if (ExTraceGlobals.directoryTasksTracer == null)
				{
					ExTraceGlobals.directoryTasksTracer = new Trace(ExTraceGlobals.componentGuid, 53);
				}
				return ExTraceGlobals.directoryTasksTracer;
			}
		}

		public static Trace ComplianceTracer
		{
			get
			{
				if (ExTraceGlobals.complianceTracer == null)
				{
					ExTraceGlobals.complianceTracer = new Trace(ExTraceGlobals.componentGuid, 54);
				}
				return ExTraceGlobals.complianceTracer;
			}
		}

		private static Guid componentGuid = new Guid("0c6a4049-bb65-4ea6-9f0c-12808260c2f1");

		private static Trace topologyProviderTracer = null;

		private static Trace aDTopologyTracer = null;

		private static Trace connectionTracer = null;

		private static Trace connectionDetailsTracer = null;

		private static Trace getConnectionTracer = null;

		private static Trace aDFindTracer = null;

		private static Trace aDReadTracer = null;

		private static Trace aDReadDetailsTracer = null;

		private static Trace aDSaveTracer = null;

		private static Trace aDSaveDetailsTracer = null;

		private static Trace aDDeleteTracer = null;

		private static Trace validationTracer = null;

		private static Trace aDNotificationsTracer = null;

		private static Trace directoryExceptionTracer = null;

		private static Trace ldapFilterBuilderTracer = null;

		private static Trace aDPropertyRequestTracer = null;

		private static Trace aDObjectTracer = null;

		private static Trace contentTypeMappingTracer = null;

		private static Trace lcidMapperTracer = null;

		private static Trace recipientUpdateServiceTracer = null;

		private static Trace uMAutoAttendantTracer = null;

		private static Trace exchangeTopologyTracer = null;

		private static Trace perfCountersTracer = null;

		private static Trace clientThrottlingTracer = null;

		private static Trace serverSettingsProviderTracer = null;

		private static Trace retryManagerTracer = null;

		private static Trace systemConfigurationCacheTracer = null;

		private static Trace federatedIdentityTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace addressListTracer = null;

		private static Trace nspiRpcClientConnectionTracer = null;

		private static Trace scopeVerificationTracer = null;

		private static Trace schemaInitializationTracer = null;

		private static Trace isMemberOfResolverTracer = null;

		private static Trace owaSegmentationTracer = null;

		private static Trace aDPerformanceTracer = null;

		private static Trace resourceHealthManagerTracer = null;

		private static Trace budgetDelayTracer = null;

		private static Trace gLSTracer = null;

		private static Trace mServTracer = null;

		private static Trace tenantRelocationTracer = null;

		private static Trace stateManagementTracer = null;

		private static Trace serverComponentStateManagerTracer = null;

		private static Trace sessionSettingsTracer = null;

		private static Trace aDConfigLoaderTracer = null;

		private static Trace slimTenantTracer = null;

		private static Trace tenantUpgradeServiceletTracer = null;

		private static Trace directoryTasksTracer = null;

		private static Trace complianceTracer = null;
	}
}
