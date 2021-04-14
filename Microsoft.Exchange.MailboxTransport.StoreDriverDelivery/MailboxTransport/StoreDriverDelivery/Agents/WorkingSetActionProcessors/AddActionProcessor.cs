using System;
using Microsoft.Exchange.Data.Storage.WorkingSet.SignalApiEx;
using Microsoft.Exchange.WorkingSet.SignalApi;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents.WorkingSetActionProcessors
{
	internal class AddActionProcessor : AbstractActionProcessor
	{
		public AddActionProcessor(ActionProcessorFactory actionProcessorFactory) : base(actionProcessorFactory)
		{
		}

		public override void Process(StoreDriverDeliveryEventArgsImpl argsImpl, Action action, int traceId)
		{
			AbstractActionProcessor.Tracer.TraceDebug((long)traceId, "WorkingSetAgent.AddActionProcessor.Process: entering");
			if (action.Type == "DeletePartition")
			{
				this.actionProcessorFactory.GetActionProcessor(ActionProcessorType.DeletePartitionProcesor).Process(argsImpl, action, traceId);
				return;
			}
			if (action.Item.GetType() == typeof(ExchangeItem))
			{
				this.actionProcessorFactory.GetActionProcessor(ActionProcessorType.AddExchangeItemProcessor).Process(argsImpl, action, traceId);
				return;
			}
			WorkingSet.ItemsNotSupported.Increment();
			throw new NotSupportedException(string.Format("Item type not supported - {0} - {1}", action.Item.Identifier, action.Item.GetType()));
		}
	}
}
