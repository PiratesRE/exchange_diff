using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionFabric.Blocks;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Dataflow
{
	internal class JobReceiver : MessageReceiverBase
	{
		private JobReceiver()
		{
		}

		public static JobReceiver Instance
		{
			get
			{
				return JobReceiver.instance;
			}
		}

		protected override Task ReceiveMessageInternal(ComplianceMessage message)
		{
			TransformBlock<ComplianceMessage, IEnumerable<ComplianceMessage>> dataflowBlock = this.distributeBlock.GetDataflowBlock(null);
			TransformBlock<IEnumerable<ComplianceMessage>, object> dataflowBlock2 = this.sendBlock.GetDataflowBlock(null);
			dataflowBlock.LinkTo(dataflowBlock2, BlockBase<ComplianceMessage, IEnumerable<ComplianceMessage>>.DefaultLinkOptions);
			if (DataflowBlock.Post<ComplianceMessage>(dataflowBlock, message))
			{
				dataflowBlock.Complete();
				return dataflowBlock2.Completion;
			}
			return null;
		}

		private static JobReceiver instance = new JobReceiver();

		private DistributeBlock distributeBlock = new DistributeBlock();

		private SendBlock sendBlock = new SendBlock();
	}
}
