using System;
using System.ServiceModel;

namespace Microsoft.Exchange.DxStore.Common
{
	[ServiceContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	public interface IDxStoreManager
	{
		[OperationContract]
		void StartInstance(string groupName, bool isForce = false);

		[OperationContract]
		void RestartInstance(string groupName, bool isForce = false);

		[OperationContract]
		void RemoveInstance(string groupName);

		[OperationContract]
		void StopInstance(string groupName, bool isDisable = true);

		[OperationContract]
		InstanceGroupConfig GetInstanceConfig(string groupName, bool isForce = false);

		[OperationContract]
		void TriggerRefresh(string reason, bool isForceRefreshCache);
	}
}
