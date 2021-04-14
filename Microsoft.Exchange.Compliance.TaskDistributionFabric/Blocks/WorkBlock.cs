using System;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Diagnostics;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;
using Microsoft.Exchange.Compliance.TaskDistributionFabric.Payload;
using Microsoft.Exchange.Compliance.TaskDistributionFabric.Utility;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Blocks
{
	internal class WorkBlock : BlockBase<ComplianceMessage, ComplianceMessage>
	{
		public override ComplianceMessage Process(ComplianceMessage input)
		{
			IPayloadRetriever payloadRetriever;
			FaultDefinition faultDefinition;
			WorkPayload workPayload;
			IApplicationPlugin applicationPlugin;
			if (Registry.Instance.TryGetInstance<IPayloadRetriever>(RegistryComponent.TaskDistribution, TaskDistributionComponent.PayloadRetriever, out payloadRetriever, out faultDefinition, "Process", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Blocks\\WorkBlock.cs", 40) && payloadRetriever.TryGetPayload<WorkPayload>(WorkPayload.Description, input.Payload, out workPayload, out faultDefinition) && Registry.Instance.TryGetInstance<IApplicationPlugin>(RegistryComponent.Application, workPayload.WorkDefinitionType, out applicationPlugin, out faultDefinition, "Process", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Blocks\\WorkBlock.cs", 44))
			{
				ComplianceMessage complianceMessage = input.Clone();
				complianceMessage.MessageId = string.Format("{0}-WORKRESULT", input.MessageId);
				complianceMessage.MessageSourceId = input.MessageId;
				complianceMessage.MessageSource = input.MessageTarget;
				complianceMessage.ComplianceMessageType = ComplianceMessageType.RecordResult;
				WorkPayload inputObject = null;
				if (ApplicationHelper.TryDoWork(complianceMessage, workPayload, out inputObject, out faultDefinition, "Process", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Blocks\\WorkBlock.cs", 54))
				{
					complianceMessage.Payload = ComplianceSerializer.Serialize<WorkPayload>(WorkPayload.Description, inputObject);
				}
				if (faultDefinition != null)
				{
					ExceptionHandler.FaultMessage(complianceMessage, faultDefinition, true);
				}
				return complianceMessage;
			}
			if (faultDefinition != null)
			{
				ExceptionHandler.FaultMessage(input, faultDefinition, true);
			}
			return null;
		}
	}
}
