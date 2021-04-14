using System;
using Microsoft.Exchange.Data.Storage.Compliance.DAR;
using Microsoft.Exchange.Management.CompliancePolicy.DarTasks.Protocol;
using Microsoft.Exchange.Rpc.Dar;

namespace Microsoft.Exchange.Management.CompliancePolicy.DarTasks
{
	internal class HostRpcClient : ExDarHostRpcClient
	{
		public HostRpcClient(string machineName) : base(machineName)
		{
		}

		public void NotifyTaskStoreChange(byte[] tenantId)
		{
			base.SendHostRequest(0, 0, tenantId);
		}

		public void EnsureTenantMonitoring(byte[] tenantId)
		{
			base.SendHostRequest(0, 1, tenantId);
		}

		public DarTaskResult SendDarHostRequest(int version, int type, byte[] inputParameterBytes)
		{
			byte[] data = base.SendHostRequest(version, type, inputParameterBytes);
			return DarTaskResult.GetResultObject(data);
		}

		public TaskStoreObject[] GetDarTask(DarTaskParams darParams)
		{
			DarTaskResult darTaskResult = this.SendDarHostRequest(0, 2, darParams.ToBytes());
			return darTaskResult.DarTasks;
		}

		public void SetDarTask(TaskStoreObject darTaskObject)
		{
			byte[] inputParameterBytes = DarTaskResult.ObjectToBytes<TaskStoreObject>(darTaskObject);
			this.SendDarHostRequest(0, 3, inputParameterBytes);
		}

		public TaskAggregateStoreObject[] GetDarTaskAggregate(DarTaskAggregateParams darTaskAggregateParams)
		{
			DarTaskResult darTaskResult = this.SendDarHostRequest(0, 4, darTaskAggregateParams.ToBytes());
			return darTaskResult.DarTaskAggregates;
		}

		public void SetDarTaskAggregate(TaskAggregateStoreObject darTaskObject)
		{
			byte[] inputParameterBytes = DarTaskResult.ObjectToBytes<TaskAggregateStoreObject>(darTaskObject);
			this.SendDarHostRequest(0, 5, inputParameterBytes);
		}

		public void RemoveCompletedDarTasks(DarTaskParams darParams)
		{
			this.SendDarHostRequest(0, 6, darParams.ToBytes());
		}

		public void RemoveDarTaskAggregate(DarTaskAggregateParams darTaskAggregateParams)
		{
			this.SendDarHostRequest(0, 7, darTaskAggregateParams.ToBytes());
		}

		public string GetDarInfo()
		{
			int version = 0;
			int type = 8;
			byte[] inputParameterBytes = new byte[1];
			DarTaskResult darTaskResult = this.SendDarHostRequest(version, type, inputParameterBytes);
			return darTaskResult.LocalizedInformation;
		}
	}
}
