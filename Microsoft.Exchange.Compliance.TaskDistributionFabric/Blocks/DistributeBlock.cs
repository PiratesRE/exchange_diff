using System;
using System.Collections.Generic;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Diagnostics;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Utility;
using Microsoft.Exchange.Compliance.TaskDistributionFabric.Payload;
using Microsoft.Exchange.Compliance.TaskDistributionFabric.Utility;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Blocks
{
	internal class DistributeBlock : BlockBase<ComplianceMessage, IEnumerable<ComplianceMessage>>
	{
		public override IEnumerable<ComplianceMessage> Process(ComplianceMessage input)
		{
			IPayloadRetriever retriever;
			FaultDefinition faultDefinition;
			JobPayload job;
			if (Registry.Instance.TryGetInstance<IPayloadRetriever>(RegistryComponent.TaskDistribution, TaskDistributionComponent.PayloadRetriever, out retriever, out faultDefinition, "Process", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Blocks\\DistributeBlock.cs", 38) && retriever.TryGetPayload<JobPayload>(JobPayload.Description, input.Payload, out job, out faultDefinition))
			{
				WorkPayload workPayload;
				WorkPayload inputObject;
				if (MessageHelper.IsFromDriver(input) && retriever.TryGetPayload<WorkPayload>(WorkPayload.Description, job.Payload, out workPayload, out faultDefinition) && ApplicationHelper.TryPreprocess(input, workPayload, out inputObject, out faultDefinition, "Process", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Blocks\\DistributeBlock.cs", 48))
				{
					job.Payload = ComplianceSerializer.Serialize<WorkPayload>(WorkPayload.Description, inputObject);
				}
				if (faultDefinition == null && !ExceptionHandler.IsFaulted(input))
				{
					foreach (byte[] childBlob in job.Children)
					{
						foreach (JobPayload childJob in retriever.GetAllPayloads<JobPayload>(JobPayload.Description, childBlob, out faultDefinition))
						{
							ComplianceMessage childMessage = input.Clone();
							childMessage.MessageTarget = childJob.Target;
							childMessage.MessageId = childJob.JobId;
							childMessage.MessageSource = input.MessageTarget;
							childMessage.MessageSourceId = input.MessageId;
							if (childJob.Children.Count == 0)
							{
								childMessage.ComplianceMessageType = ComplianceMessageType.StartWork;
								childMessage.Payload = job.Payload;
							}
							else
							{
								childJob.Payload = job.Payload;
								childMessage.Payload = ComplianceSerializer.Serialize<JobPayload>(JobPayload.Description, childJob);
							}
							yield return childMessage;
						}
					}
				}
			}
			if (faultDefinition != null)
			{
				ExceptionHandler.FaultMessage(input, faultDefinition, true);
			}
			yield break;
		}
	}
}
