using System;
using System.ServiceModel;

namespace Microsoft.Exchange.DxStore.Common
{
	[ServiceContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	public interface IDxStoreAccess
	{
		[OperationContract]
		DxStoreAccessReply.CheckKey CheckKey(DxStoreAccessRequest.CheckKey request);

		[OperationContract]
		DxStoreAccessReply.DeleteKey DeleteKey(DxStoreAccessRequest.DeleteKey request);

		[OperationContract]
		DxStoreAccessReply.SetProperty SetProperty(DxStoreAccessRequest.SetProperty request);

		[OperationContract]
		DxStoreAccessReply.DeleteProperty DeleteProperty(DxStoreAccessRequest.DeleteProperty request);

		[OperationContract]
		DxStoreAccessReply.ExecuteBatch ExecuteBatch(DxStoreAccessRequest.ExecuteBatch request);

		[OperationContract]
		DxStoreAccessReply.GetProperty GetProperty(DxStoreAccessRequest.GetProperty request);

		[OperationContract]
		DxStoreAccessReply.GetAllProperties GetAllProperties(DxStoreAccessRequest.GetAllProperties request);

		[OperationContract]
		DxStoreAccessReply.GetPropertyNames GetPropertyNames(DxStoreAccessRequest.GetPropertyNames request);

		[OperationContract]
		DxStoreAccessReply.GetSubkeyNames GetSubkeyNames(DxStoreAccessRequest.GetSubkeyNames request);
	}
}
