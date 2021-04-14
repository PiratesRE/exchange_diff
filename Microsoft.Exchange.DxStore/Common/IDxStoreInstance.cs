using System;
using System.ServiceModel;
using System.Threading.Tasks;
using FUSE.Paxos;

namespace Microsoft.Exchange.DxStore.Common
{
	[ServiceContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	public interface IDxStoreInstance
	{
		[OperationContract]
		void Stop(bool isFlush = true);

		[OperationContract]
		void Flush();

		[OperationContract]
		void Reconfigure(InstanceGroupMemberConfig[] members);

		[OperationContract]
		InstanceStatusInfo GetStatus();

		[OperationContract]
		InstanceSnapshotInfo AcquireSnapshot(string fullKeyName = null, bool isCompress = true);

		[OperationContract]
		void ApplySnapshot(InstanceSnapshotInfo snapshot, bool isForce = false);

		[OperationContract]
		void TryBecomeLeader();

		[OperationContract]
		void NotifyInitiator(Guid commandId, string sender, int instanceNumber, bool isSucceeded, string errorMessage);

		[OperationContract]
		Task PaxosMessageAsync(string sender, Message message);
	}
}
