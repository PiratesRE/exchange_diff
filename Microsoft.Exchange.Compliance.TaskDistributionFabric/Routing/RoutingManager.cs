using System;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Instrumentation;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Routing
{
	internal class RoutingManager : IRoutingManager
	{
		public void SendMessage(ComplianceMessage message)
		{
			RoutingCache.Instance.SendMessage(message);
		}

		public bool ReceiveMessage(ComplianceMessage message)
		{
			MessageLogger.Instance.LogMessageReceived(message);
			bool result = true;
			RoutingCache.Instance.ReceiveMessage(message, out result);
			return result;
		}

		public void ReturnMessage(ComplianceMessage message)
		{
			RoutingCache.Instance.ReturnMessage(message);
		}

		public void ProcessedMessage(ComplianceMessage message)
		{
			MessageLogger.Instance.LogMessageProcessed(message);
			RoutingCache.Instance.ProcessedMessage(message);
		}

		public void DispatchedMessage(ComplianceMessage message)
		{
			MessageLogger.Instance.LogMessageDispatched(message);
			RoutingCache.Instance.DispatchedMessage(message);
		}

		public void RecordResult(ComplianceMessage message, Func<ResultBase, ResultBase> commitFunction)
		{
			RoutingCache.Instance.RecordResult(message, commitFunction);
		}
	}
}
