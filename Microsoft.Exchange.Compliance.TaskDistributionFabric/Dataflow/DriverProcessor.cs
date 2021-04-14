using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionFabric.Blocks;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Dataflow
{
	internal class DriverProcessor : MessageProcessorBase
	{
		private DriverProcessor()
		{
		}

		public static DriverProcessor Instance
		{
			get
			{
				return DriverProcessor.instance;
			}
		}

		protected override async Task<ComplianceMessage> ProcessMessageInternal(ComplianceMessage message)
		{
			TransformBlock<ComplianceMessage, ComplianceMessage> transformer = null;
			switch (message.ComplianceMessageType)
			{
			case ComplianceMessageType.RecordResult:
				transformer = this.storeBlock.GetDataflowBlock(null);
				goto IL_B1;
			case ComplianceMessageType.RetrieveRequest:
				transformer = this.retrievePayloadBlock.GetDataflowBlock(null);
				goto IL_B1;
			case ComplianceMessageType.EchoRequest:
				transformer = this.echoBlock.GetDataflowBlock(null);
				goto IL_B1;
			}
			throw new ArgumentException(string.Format("MessageType:{0} is not supported", message.ComplianceMessageType));
			IL_B1:
			await DataflowBlock.SendAsync<ComplianceMessage>(transformer, message);
			ComplianceMessage response = await DataflowBlock.ReceiveAsync<ComplianceMessage>(transformer);
			transformer.Complete();
			return response;
		}

		private static DriverProcessor instance = new DriverProcessor();

		private EchoRequestBlock echoBlock = new EchoRequestBlock();

		private RetrievePayloadBlock retrievePayloadBlock = new RetrievePayloadBlock();

		private StoreResultsBlock storeBlock = new StoreResultsBlock();
	}
}
