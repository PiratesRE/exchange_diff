using System;
using System.Threading.Tasks;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Instrumentation;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Dataflow
{
	internal abstract class MessageProcessorBase
	{
		public virtual Task<ComplianceMessage> ProcessMessage(ComplianceMessage message)
		{
			Task<ComplianceMessage> result;
			try
			{
				MessageLogger.Instance.LogMessageProcessing(message);
				result = this.ProcessMessageInternal(message);
			}
			finally
			{
				MessageLogger.Instance.LogMessageProcessed(message);
			}
			return result;
		}

		protected abstract Task<ComplianceMessage> ProcessMessageInternal(ComplianceMessage message);
	}
}
