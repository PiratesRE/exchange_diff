using System;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Routing
{
	internal interface IRoutingManager
	{
		bool ReceiveMessage(ComplianceMessage message);

		void SendMessage(ComplianceMessage message);

		void ReturnMessage(ComplianceMessage message);

		void ProcessedMessage(ComplianceMessage message);

		void DispatchedMessage(ComplianceMessage message);

		void RecordResult(ComplianceMessage message, Func<ResultBase, ResultBase> commitFunction);
	}
}
