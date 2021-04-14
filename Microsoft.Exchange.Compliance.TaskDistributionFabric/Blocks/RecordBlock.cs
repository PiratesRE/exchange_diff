using System;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Diagnostics;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionFabric.Payload;
using Microsoft.Exchange.Compliance.TaskDistributionFabric.Routing;
using Microsoft.Exchange.Compliance.TaskDistributionFabric.Utility;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Blocks
{
	internal class RecordBlock : BlockBase<ComplianceMessage, ComplianceMessage>
	{
		public override ComplianceMessage Process(ComplianceMessage input)
		{
			FaultDefinition faultDefinition;
			IPayloadRetriever payloadRetriever;
			if (Registry.Instance.TryGetInstance<IPayloadRetriever>(RegistryComponent.TaskDistribution, TaskDistributionComponent.PayloadRetriever, out payloadRetriever, out faultDefinition, "Process", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Blocks\\RecordBlock.cs", 37))
			{
				input.ComplianceMessageType = ComplianceMessageType.RecordResult;
				WorkPayload payload;
				payloadRetriever.TryGetPayload<WorkPayload>(WorkPayload.Description, input.Payload, out payload, out faultDefinition);
				if (ExceptionHandler.IsFaulted(input) || faultDefinition != null)
				{
					payload = (ExceptionHandler.GetFaultDefinition(input) ?? faultDefinition).ToPayload();
				}
				IRoutingManager routingManager;
				if (Registry.Instance.TryGetInstance<IRoutingManager>(RegistryComponent.TaskDistribution, TaskDistributionComponent.RoutingManager, out routingManager, out faultDefinition, "Process", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Blocks\\RecordBlock.cs", 47))
				{
					Func<ResultBase, ResultBase> commitFunction = delegate(ResultBase existing)
					{
						ResultBase result;
						FaultDefinition faultDefinition;
						if (ApplicationHelper.TryRecordResult(input, existing, payload, out result, out faultDefinition, "Process", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Blocks\\RecordBlock.cs", 53))
						{
							return result;
						}
						faultDefinition = faultDefinition;
						return existing;
					};
					routingManager.RecordResult(input, commitFunction);
					return input;
				}
			}
			if (faultDefinition != null)
			{
				ExceptionHandler.FaultMessage(input, faultDefinition, true);
			}
			return null;
		}
	}
}
