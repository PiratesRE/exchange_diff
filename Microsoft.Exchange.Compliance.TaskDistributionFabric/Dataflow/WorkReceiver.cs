using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionFabric.Blocks;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Dataflow
{
	internal class WorkReceiver : MessageReceiverBase
	{
		private WorkReceiver()
		{
		}

		public static WorkReceiver Instance
		{
			get
			{
				return WorkReceiver.instance;
			}
		}

		protected override Task ReceiveMessageInternal(ComplianceMessage message)
		{
			TaskScheduler @default = TaskScheduler.Default;
			FaultDefinition faultDefinition;
			Registry.Instance.TryGetInstance<TaskScheduler>(RegistryComponent.TaskDistribution, TaskDistributionComponent.ResourceScheduler, out @default, out faultDefinition, "ReceiveMessageInternal", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Dataflow\\WorkReceiver.cs", 77);
			TransformBlock<ComplianceMessage, ComplianceMessage> dataflowBlock = this.workBlock.GetDataflowBlock(new ExecutionDataflowBlockOptions
			{
				TaskScheduler = @default,
				MaxDegreeOfParallelism = -1
			});
			TransformBlock<ComplianceMessage, ComplianceMessage> dataflowBlock2 = this.recordBlock.GetDataflowBlock(null);
			TransformBlock<ComplianceMessage, object> dataflowBlock3 = this.returnBlock.GetDataflowBlock(null);
			dataflowBlock.LinkTo(dataflowBlock2, BlockBase<ComplianceMessage, ComplianceMessage>.DefaultLinkOptions);
			dataflowBlock2.LinkTo(dataflowBlock3, BlockBase<ComplianceMessage, ComplianceMessage>.DefaultLinkOptions);
			if (DataflowBlock.Post<ComplianceMessage>(dataflowBlock, message))
			{
				dataflowBlock.Complete();
				return dataflowBlock3.Completion;
			}
			return null;
		}

		private static WorkReceiver instance = new WorkReceiver();

		private WorkBlock workBlock = new WorkBlock();

		private RecordBlock recordBlock = new RecordBlock();

		private ReturnBlock returnBlock = new ReturnBlock();
	}
}
