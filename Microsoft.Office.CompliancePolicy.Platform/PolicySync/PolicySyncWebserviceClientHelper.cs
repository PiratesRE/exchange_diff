using System;
using System.Linq;
using Microsoft.Office.CompliancePolicy.Monitor;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	internal static class PolicySyncWebserviceClientHelper
	{
		public static PolicyChange GetSingleTenantChanges(this IPolicySyncWebserviceClient syncSvcClient, TenantCookie tenantCookie, SyncMonitorEventTracker monitorTracker)
		{
			PolicyChange result = null;
			int deltaObjectNumber = 0;
			monitorTracker.TrackLatencyWrapper(LatencyType.FfoWsCall, tenantCookie.ObjectType, ref deltaObjectNumber, delegate()
			{
				result = syncSvcClient.GetSingleTenantChanges(tenantCookie);
				deltaObjectNumber = PolicySyncWebserviceClientHelper.CalculateObjectNumber(result, tenantCookie.ObjectType);
			}, true, false);
			return result;
		}

		public static IAsyncResult BeginGetSingleTenantChanges(this IPolicySyncWebserviceClient syncSvcClient, TenantCookie tenantCookie, AsyncCallback userCallback, object stateObject, SyncMonitorEventTracker monitorTracker)
		{
			IAsyncResult result = null;
			monitorTracker.TrackLatencyWrapper(LatencyType.FfoWsCall, tenantCookie.ObjectType, delegate()
			{
				result = syncSvcClient.BeginGetSingleTenantChanges(tenantCookie, userCallback, stateObject);
			}, false);
			return result;
		}

		public static PolicyChange EndGetSingleTenantChanges(this IPolicySyncWebserviceClient syncSvcClient, IAsyncResult asyncResult, SyncMonitorEventTracker monitorTracker, ConfigurationObjectType objectType)
		{
			PolicyChange result = null;
			int deltaObjectNumber = 0;
			monitorTracker.TrackLatencyWrapper(LatencyType.FfoWsCall, objectType, ref deltaObjectNumber, delegate()
			{
				result = syncSvcClient.EndGetSingleTenantChanges(asyncResult);
				deltaObjectNumber = PolicySyncWebserviceClientHelper.CalculateObjectNumber(result, objectType);
			}, true, true);
			return result;
		}

		public static PolicyConfigurationBase GetObject(this IPolicySyncWebserviceClient syncSvcClient, SyncCallerContext callerContext, Guid tenantId, ConfigurationObjectType objectType, Guid objectId, bool includeDeletedObjects, SyncMonitorEventTracker monitorTracker)
		{
			PolicyConfigurationBase result = null;
			int deltaObjectNumber = 0;
			monitorTracker.TrackLatencyWrapper(LatencyType.FfoWsCall, objectType, ref deltaObjectNumber, delegate()
			{
				result = syncSvcClient.GetObject(callerContext, tenantId, objectType, objectId, includeDeletedObjects);
				deltaObjectNumber = PolicySyncWebserviceClientHelper.CalculateObjectNumber(result, objectType);
			}, true, false);
			return result;
		}

		public static IAsyncResult BeginGetObject(this IPolicySyncWebserviceClient syncSvcClient, SyncCallerContext callerContext, Guid tenantId, ConfigurationObjectType objectType, Guid objectId, bool includeDeletedObjects, AsyncCallback userCallback, object stateObject, SyncMonitorEventTracker monitorTracker)
		{
			IAsyncResult result = null;
			monitorTracker.TrackLatencyWrapper(LatencyType.FfoWsCall, objectType, delegate()
			{
				result = syncSvcClient.BeginGetObject(callerContext, tenantId, objectType, objectId, includeDeletedObjects, userCallback, stateObject);
			}, false);
			return result;
		}

		public static PolicyConfigurationBase EndGetObject(this IPolicySyncWebserviceClient syncSvcClient, IAsyncResult asyncResult, SyncMonitorEventTracker monitorTracker, ConfigurationObjectType objectType)
		{
			PolicyConfigurationBase result = null;
			int deltaObjectNumber = 0;
			monitorTracker.TrackLatencyWrapper(LatencyType.FfoWsCall, objectType, ref deltaObjectNumber, delegate()
			{
				result = syncSvcClient.EndGetObject(asyncResult);
				deltaObjectNumber = PolicySyncWebserviceClientHelper.CalculateObjectNumber(result, objectType);
			}, true, true);
			return result;
		}

		private static int CalculateObjectNumber(PolicyConfigurationBase deltaObject, ConfigurationObjectType objectType)
		{
			int result = 0;
			if (deltaObject != null)
			{
				if (objectType != ConfigurationObjectType.Binding)
				{
					result = 1;
				}
				else
				{
					BindingConfiguration bindingConfiguration = (BindingConfiguration)deltaObject;
					if (bindingConfiguration.AppliedScopes != null && bindingConfiguration.AppliedScopes.Changed)
					{
						int num = (bindingConfiguration.AppliedScopes.RemovedValues == null) ? 0 : bindingConfiguration.AppliedScopes.RemovedValues.Count<ScopeConfiguration>();
						int num2 = (bindingConfiguration.AppliedScopes.ChangedValues == null) ? 0 : bindingConfiguration.AppliedScopes.ChangedValues.Count<ScopeConfiguration>();
						result = num + num2;
					}
				}
			}
			return result;
		}

		private static int CalculateObjectNumber(PolicyChange deltaObjects, ConfigurationObjectType objectType)
		{
			int num = 0;
			if (deltaObjects != null && deltaObjects.Changes != null)
			{
				if (objectType != ConfigurationObjectType.Binding)
				{
					num = deltaObjects.Changes.Count<PolicyConfigurationBase>();
				}
				else
				{
					foreach (PolicyConfigurationBase deltaObject in deltaObjects.Changes)
					{
						num += PolicySyncWebserviceClientHelper.CalculateObjectNumber(deltaObject, objectType);
					}
				}
			}
			return num;
		}
	}
}
