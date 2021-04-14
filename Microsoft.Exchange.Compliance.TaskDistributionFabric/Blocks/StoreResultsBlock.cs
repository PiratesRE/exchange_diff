using System;
using System.Text;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Blocks
{
	internal class StoreResultsBlock : BlockBase<ComplianceMessage, ComplianceMessage>
	{
		public override ComplianceMessage Process(ComplianceMessage input)
		{
			StatusPayload statusPayload = new StatusPayload();
			if (input == null)
			{
				return new ComplianceMessage
				{
					Payload = ComplianceSerializer.Serialize<StatusPayload>(StatusPayload.Description, statusPayload)
				};
			}
			statusPayload.QueuedMessages.Add(input.MessageId);
			WorkPayload workPayload;
			FaultDefinition faultDefinition;
			OrganizationId orgId;
			if (ComplianceSerializer.TryDeserialize<WorkPayload>(WorkPayload.Description, input.Payload, out workPayload, out faultDefinition, "Process", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Blocks\\StoreResultsBlock.cs", 47) && OrganizationId.TryCreateFromBytes(input.TenantId, Encoding.UTF8, out orgId))
			{
				Guid correlationId = input.CorrelationId;
				ComplianceJobProvider complianceJobProvider = new ComplianceJobProvider(orgId);
				ComplianceJobStatus status = ComplianceJobStatus.StatusUnknown;
				switch (workPayload.WorkDefinitionType)
				{
				case WorkDefinitionType.EDiscovery:
					status = ComplianceJobStatus.Succeeded;
					break;
				case WorkDefinitionType.Fault:
					status = ComplianceJobStatus.Failed;
					break;
				}
				complianceJobProvider.UpdateWorkloadResults(correlationId, input.Payload, ComplianceBindingType.ExchangeBinding, status);
			}
			return new ComplianceMessage
			{
				Payload = ComplianceSerializer.Serialize<StatusPayload>(StatusPayload.Description, statusPayload)
			};
		}
	}
}
