using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Contract;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Instrumentation;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;
using Microsoft.Exchange.Compliance.TaskDistributionFabric.Dataflow;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Endpoint
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	internal class MessageProcessor : IMessageProcessor
	{
		public async Task<byte[]> ProcessMessageAsync(byte[] message)
		{
			ComplianceMessage response = null;
			ComplianceMessage request;
			FaultDefinition faultDefinition;
			MessageProcessorBase messageProcessor;
			if (ComplianceSerializer.TryDeserialize<ComplianceMessage>(ComplianceMessage.Description, message, out request, out faultDefinition, "ProcessMessageAsync", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Endpoint\\MessageProcessor.cs", 49) && Registry.Instance.TryGetInstance<MessageProcessorBase>(RegistryComponent.MessageProcessor, request.ComplianceMessageType, out messageProcessor, out faultDefinition, "ProcessMessageAsync", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Endpoint\\MessageProcessor.cs", 51))
			{
				MessageLogger.Instance.LogMessageReceived(request);
				response = await messageProcessor.ProcessMessage(request);
			}
			if (faultDefinition != null)
			{
				response = new ComplianceMessage
				{
					WorkDefinitionType = WorkDefinitionType.Fault,
					Payload = faultDefinition.ToPayload()
				};
			}
			return ComplianceSerializer.Serialize<ComplianceMessage>(ComplianceMessage.Description, response);
		}
	}
}
