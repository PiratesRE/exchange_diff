using System;
using System.ServiceModel;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[ServiceKnownType(typeof(AssociationConfiguration))]
	[ServiceKnownType(typeof(RuleConfiguration))]
	[ServiceKnownType(typeof(BindingConfiguration))]
	[ServiceKnownType(typeof(PolicyConfiguration))]
	[ServiceContract]
	public interface IPolicySyncWebserviceClient : IDisposable
	{
		[OperationContract]
		[FaultContract(typeof(PolicySyncTransientFault))]
		[FaultContract(typeof(PolicySyncPermanentFault))]
		PolicyChange GetSingleTenantChanges(TenantCookie tenantCookie);

		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetSingleTenantChanges(TenantCookie tenantCookie, AsyncCallback userCallback, object stateObject);

		PolicyChange EndGetSingleTenantChanges(IAsyncResult asyncResult);

		[FaultContract(typeof(PolicySyncTransientFault))]
		[FaultContract(typeof(PolicySyncPermanentFault))]
		[OperationContract(Action = "http://tempuri.org/IPolicySyncWebservice/GetChanges", ReplyAction = "http://tempuri.org/IPolicySyncWebservice/GetChangesResponse")]
		PolicyChangeBatch GetChanges(GetChangesRequest request);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IPolicySyncWebservice/GetChanges", ReplyAction = "http://tempuri.org/IPolicySyncWebservice/GetChangesResponse")]
		IAsyncResult BeginGetChanges(GetChangesRequest request, AsyncCallback userCallback, object stateObject);

		PolicyChangeBatch EndGetChanges(IAsyncResult asyncResult);

		[OperationContract(Action = "http://tempuri.org/IPolicySyncWebservice/GetObject", ReplyAction = "http://tempuri.org/IPolicySyncWebservice/GetObjectResponse")]
		[FaultContract(typeof(PolicySyncTransientFault))]
		[FaultContract(typeof(PolicySyncPermanentFault))]
		PolicyConfigurationBase GetObject(SyncCallerContext callerContext, Guid tenantId, ConfigurationObjectType objectType, Guid objectId, bool includeDeletedObjects);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IPolicySyncWebservice/GetObject", ReplyAction = "http://tempuri.org/IPolicySyncWebservice/GetObjectResponse")]
		IAsyncResult BeginGetObject(SyncCallerContext callerContext, Guid tenantId, ConfigurationObjectType objectType, Guid objectId, bool includeDeletedObjects, AsyncCallback userCallback, object stateObject);

		PolicyConfigurationBase EndGetObject(IAsyncResult asyncResult);

		[FaultContract(typeof(PolicySyncPermanentFault))]
		[OperationContract(Action = "http://tempuri.org/IPolicySyncWebservice/PublishStatus", ReplyAction = "http://tempuri.org/IPolicySyncWebservice/PublishStatusResponse")]
		[FaultContract(typeof(PolicySyncTransientFault))]
		void PublishStatus(PublishStatusRequest request);

		[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IPolicySyncWebservice/PublishStatus", ReplyAction = "http://tempuri.org/IPolicySyncWebservice/PublishStatusResponse")]
		IAsyncResult BeginPublishStatus(PublishStatusRequest request, AsyncCallback userCallback, object stateObject);

		void EndPublishStatus(IAsyncResult asyncResult);
	}
}
