using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionFabric.Blocks;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Dataflow
{
	internal class ResultReceiver : MessageReceiverBase
	{
		private ResultReceiver()
		{
		}

		public static ResultReceiver Instance
		{
			get
			{
				return ResultReceiver.instance;
			}
		}

		protected override Task ReceiveMessageInternal(ComplianceMessage message)
		{
			TransformBlock<ComplianceMessage, ComplianceMessage> dataflowBlock = this.recordBlock.GetDataflowBlock(null);
			TransformBlock<ComplianceMessage, object> dataflowBlock2 = this.returnBlock.GetDataflowBlock(null);
			dataflowBlock.LinkTo(dataflowBlock2, BlockBase<ComplianceMessage, ComplianceMessage>.DefaultLinkOptions);
			if (DataflowBlock.Post<ComplianceMessage>(dataflowBlock, message))
			{
				dataflowBlock.Complete();
				return dataflowBlock2.Completion;
			}
			return null;
		}

		private static ResultReceiver instance = new ResultReceiver();

		private RecordBlock recordBlock = new RecordBlock();

		private ReturnBlock returnBlock = new ReturnBlock();
	}
}
