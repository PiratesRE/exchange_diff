using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents.WorkingSetActionProcessors
{
	internal class ActionProcessorFactory
	{
		internal ActionProcessorFactory(WorkingSetAgent workingSetAgent)
		{
			this.actionsProcessors = new Dictionary<ActionProcessorType, IActionProcessor>
			{
				{
					ActionProcessorType.AddActionProcessor,
					new AddActionProcessor(this)
				},
				{
					ActionProcessorType.AddExchangeItemProcessor,
					new AddExchangeItemProcessor(this)
				},
				{
					ActionProcessorType.DeletePartitionProcesor,
					new DeletePartitionProcessor(this)
				}
			};
			this.WorkingSetAgent = workingSetAgent;
		}

		internal IActionProcessor GetActionProcessor(ActionProcessorType actionProcessorType)
		{
			if (this.actionsProcessors.ContainsKey(actionProcessorType))
			{
				return this.actionsProcessors[actionProcessorType];
			}
			throw new NotSupportedException(string.Format("Action processor type not supported - {0}", actionProcessorType));
		}

		private readonly IDictionary<ActionProcessorType, IActionProcessor> actionsProcessors;

		internal readonly WorkingSetAgent WorkingSetAgent;
	}
}
