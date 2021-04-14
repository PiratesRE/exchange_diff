using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Utility;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Blocks
{
	internal class RetrievePayloadBlock : BlockBase<ComplianceMessage, ComplianceMessage>
	{
		public override ComplianceMessage Process(ComplianceMessage input)
		{
			ComplianceMessage complianceMessage = new ComplianceMessage
			{
				CorrelationId = input.CorrelationId,
				TenantId = input.TenantId,
				MessageId = default(Guid).ToString()
			};
			FaultDefinition faultDefinition = null;
			RetrievedPayload payload = new RetrievedPayload();
			PayloadReference payloadReference;
			if (!ComplianceSerializer.TryDeserialize<PayloadReference>(PayloadReference.Description, input.Payload, out payloadReference, out faultDefinition, "Process", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Blocks\\RetrievePayloadBlock.cs", 43))
			{
				throw new BadStructureFormatException(string.Format("Problem in deserializing the payload reference:{0}", input.ComplianceMessageType));
			}
			OrganizationId organizationId;
			if (!OrganizationId.TryCreateFromBytes(input.TenantId, Encoding.UTF8, out organizationId))
			{
				throw new ArgumentException(string.Format("Problem in creating Organization Id from the tenant id:{0}", input.ComplianceMessageType));
			}
			Guid jobRunId;
			int taskId;
			PayloadLevel payloadLevel;
			if (PayloadHelper.TryReadFromPayloadReference(payloadReference, out jobRunId, out taskId, out payloadLevel))
			{
				switch (payloadLevel)
				{
				case PayloadLevel.Job:
				{
					int taskId2;
					if (!int.TryParse(payloadReference.Bookmark, out taskId2))
					{
						taskId2 = -1;
					}
					payload = this.RetrieveJobDistributionPayload(organizationId, jobRunId, taskId2);
					break;
				}
				case PayloadLevel.Task:
					payload = this.RetrieveTaskDistributionPayload(organizationId, jobRunId, taskId);
					break;
				}
			}
			complianceMessage.Payload = payload;
			return complianceMessage;
		}

		protected RetrievedPayload RetrieveJobDistributionPayload(OrganizationId organizationId, Guid jobRunId, int taskId)
		{
			RetrievedPayload retrievedPayload = new RetrievedPayload();
			ComplianceJobProvider complianceJobProvider = new ComplianceJobProvider(organizationId);
			Guid tenantGuid = organizationId.GetTenantGuid();
			retrievedPayload.IsComplete = this.HasMoreTasks(organizationId, jobRunId);
			IEnumerable<CompositeTask> enumerable = from x in complianceJobProvider.FindCompositeTasks(tenantGuid, jobRunId, null, null)
			where x.TaskId > taskId
			select x;
			if (enumerable != null)
			{
				foreach (CompositeTask compositeTask in enumerable)
				{
					JobPayload jobPayload = new JobPayload();
					jobPayload.Target = new Target
					{
						TargetType = Target.Type.MailboxSmtpAddress,
						Identifier = compositeTask.UserMaster
					};
					PayloadReference payloadReference = PayloadHelper.GetPayloadReference(jobRunId, compositeTask.TaskId);
					jobPayload.Children.Add(ComplianceSerializer.Serialize<PayloadReference>(PayloadReference.Description, payloadReference));
					retrievedPayload.Children.Add(jobPayload);
					retrievedPayload.Bookmark = compositeTask.TaskId.ToString();
				}
				return retrievedPayload;
			}
			throw new ArgumentException(string.Format("Not Task Data Found. TenantId-{0} JobId-{1} and TaskId-{2}", tenantGuid, jobRunId, taskId));
		}

		protected RetrievedPayload RetrieveTaskDistributionPayload(OrganizationId organizationId, Guid jobRunId, int taskId)
		{
			RetrievedPayload retrievedPayload = new RetrievedPayload();
			ComplianceJobProvider complianceJobProvider = new ComplianceJobProvider(organizationId);
			Guid tenantGuid = organizationId.GetTenantGuid();
			CompositeTask compositeTask = complianceJobProvider.FindCompositeTasks(tenantGuid, jobRunId, null, new int?(taskId)).SingleOrDefault<CompositeTask>();
			if (compositeTask != null)
			{
				foreach (string identifier in compositeTask.Users)
				{
					JobPayload jobPayload = new JobPayload();
					jobPayload.Target = new Target
					{
						TargetType = Target.Type.MailboxSmtpAddress,
						Identifier = identifier
					};
					retrievedPayload.Children.Add(jobPayload);
				}
				return retrievedPayload;
			}
			throw new ArgumentException(string.Format("No Task data found. TenantId-{0} JobId-{1} and TaskId-{2}", tenantGuid, jobRunId, taskId));
		}

		private ComplianceJob GetComplianceJob(OrganizationId organizationId, Guid jobRunId)
		{
			ComplianceJobProvider complianceJobProvider = new ComplianceJobProvider(organizationId);
			Guid tenantGuid = organizationId.GetTenantGuid();
			ComplianceJob complianceJob = complianceJobProvider.FindComplianceJob(tenantGuid, jobRunId).Single<ComplianceJob>();
			if (complianceJob != null)
			{
				return complianceJob;
			}
			throw new ArgumentException(string.Format("No job data found. TenantId-{0} and JobRunId-{1}", tenantGuid, jobRunId));
		}

		private bool HasMoreTasks(OrganizationId organizationId, Guid jobRunId)
		{
			ComplianceJob complianceJob = this.GetComplianceJob(organizationId, jobRunId);
			return complianceJob.JobStatus != ComplianceJobStatus.NotStarted && complianceJob.JobStatus != ComplianceJobStatus.Starting;
		}
	}
}
