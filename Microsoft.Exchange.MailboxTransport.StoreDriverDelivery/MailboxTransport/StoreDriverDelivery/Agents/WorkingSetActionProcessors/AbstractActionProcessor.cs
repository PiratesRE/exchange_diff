using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.WorkingSet;
using Microsoft.Exchange.WorkingSet.SignalApi;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents.WorkingSetActionProcessors
{
	internal abstract class AbstractActionProcessor : IActionProcessor
	{
		protected AbstractActionProcessor(ActionProcessorFactory actionProcessorFactory)
		{
			this.actionProcessorFactory = actionProcessorFactory;
		}

		public abstract void Process(StoreDriverDeliveryEventArgsImpl argsImpl, Action action, int traceId);

		protected static readonly Trace Tracer = ExTraceGlobals.WorkingSetAgentTracer;

		protected ActionProcessorFactory actionProcessorFactory;
	}
}
