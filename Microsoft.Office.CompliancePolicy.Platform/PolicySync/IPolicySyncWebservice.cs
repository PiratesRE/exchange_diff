using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[ServiceKnownType(typeof(UnifiedPolicyStatus))]
	[ServiceKnownType(typeof(AssociationConfiguration))]
	[ServiceKnownType(typeof(PolicyConfiguration))]
	[ServiceKnownType(typeof(RuleConfiguration))]
	[ServiceKnownType(typeof(TenantCookieCollection))]
	[ServiceKnownType(typeof(BindingConfiguration))]
	[ServiceContract]
	public interface IPolicySyncWebservice
	{
		[FaultContract(typeof(PolicySyncTransientFault))]
		[FaultContract(typeof(PolicySyncPermanentFault))]
		[WebInvoke(Method = "POST", UriTemplate = "/GetChanges")]
		[OperationContract]
		PolicyChangeBatch GetChanges(GetChangesRequest request);

		[FaultContract(typeof(PolicySyncTransientFault))]
		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "/GetObject?tenantId={tenantId}&objectType={objectType}&objectId={objectId}&includeDeletedObjects={includeDeletedObjects}", BodyStyle = WebMessageBodyStyle.Wrapped)]
		[FaultContract(typeof(PolicySyncPermanentFault))]
		PolicyConfigurationBase GetObject(SyncCallerContext callerContext, Guid tenantId, ConfigurationObjectType objectType, Guid objectId, bool includeDeletedObjects = false);

		[FaultContract(typeof(PolicySyncPermanentFault))]
		[FaultContract(typeof(PolicySyncTransientFault))]
		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "/PublishStatus")]
		void PublishStatus(PublishStatusRequest request);
	}
}
