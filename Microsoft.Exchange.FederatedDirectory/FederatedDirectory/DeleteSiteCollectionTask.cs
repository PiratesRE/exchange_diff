using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UnifiedGroups;

namespace Microsoft.Exchange.FederatedDirectory
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DeleteSiteCollectionTask : UnifiedGroupsTask
	{
		public DeleteSiteCollectionTask(ADUser accessingUser, IRecipientSession adSession, Guid activityId) : base(accessingUser, adSession, activityId)
		{
		}

		public string ExternalDirectoryObjectId { get; set; }

		public string SmtpAddress { get; set; }

		protected override string TaskName
		{
			get
			{
				return "DeleteSiteCollectionTask";
			}
		}

		protected override void RunInternal()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			base.CurrentAction = UnifiedGroupsTask.UnifiedGroupsAction.SharePointDelete;
			UnifiedGroupsTask.Tracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "ActivityId={0}. DeleteSiteCollectionTask.RunInternal: Notifying SharePoint about group deletion: {1}", base.ActivityId, this.ExternalDirectoryObjectId ?? this.SmtpAddress);
			this.DeleteSiteCollection();
			UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. DeleteSiteCollectionTask.RunInternal: Finished notifying SharePoint about group deletion", base.ActivityId);
			string value = string.Format("Notified SharePoint about group deletion;Group={0};ElapsedTime={1}", this.ExternalDirectoryObjectId ?? this.SmtpAddress, stopwatch.ElapsedMilliseconds);
			FederatedDirectoryLogger.AppendToLog(new SchemaBasedLogEvent<FederatedDirectoryLogSchema.TraceTag>
			{
				{
					FederatedDirectoryLogSchema.TraceTag.TaskName,
					this.TaskName
				},
				{
					FederatedDirectoryLogSchema.TraceTag.ActivityId,
					base.ActivityId
				},
				{
					FederatedDirectoryLogSchema.TraceTag.CurrentAction,
					base.CurrentAction
				},
				{
					FederatedDirectoryLogSchema.TraceTag.Message,
					value
				}
			});
			base.CurrentAction = UnifiedGroupsTask.UnifiedGroupsAction.Completed;
		}

		private void DeleteSiteCollection()
		{
			if (string.IsNullOrEmpty(this.ExternalDirectoryObjectId))
			{
				ObjectIdMapping objectIdMapping = new ObjectIdMapping(base.ADSession);
				objectIdMapping.Prefetch(new string[]
				{
					this.SmtpAddress
				});
				this.ExternalDirectoryObjectId = objectIdMapping.GetIdentityFromSmtpAddress(this.SmtpAddress);
			}
			using (SharePointNotification sharePointNotification = new SharePointNotification(SharePointNotification.NotificationType.Delete, this.ExternalDirectoryObjectId, base.AccessingUser.OrganizationId, base.ActAsUserCredentials, base.ActivityId))
			{
				sharePointNotification.Execute();
			}
		}
	}
}
