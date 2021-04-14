using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UnifiedGroups;

namespace Microsoft.Exchange.FederatedDirectory
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UpdateSiteCollectionTask : UnifiedGroupsTask
	{
		public UpdateSiteCollectionTask(ADUser accessingUser, IRecipientSession adSession, Guid activityId) : base(accessingUser, adSession, activityId)
		{
		}

		public string Description { get; set; }

		public string DisplayName { get; set; }

		public string[] AddedOwners { get; set; }

		public string[] RemovedOwners { get; set; }

		public string[] AddedMembers { get; set; }

		public string[] RemovedMembers { get; set; }

		public string ExternalDirectoryObjectId { get; set; }

		protected override string TaskName
		{
			get
			{
				return "UpdateSiteCollectionTask";
			}
		}

		protected override void RunInternal()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			base.CurrentAction = UnifiedGroupsTask.UnifiedGroupsAction.SharePointUpdate;
			UnifiedGroupsTask.Tracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "ActivityId={0}. UpdateSiteCollectionTask.RunInternal: Notifying SharePoint about group update: {1}", base.ActivityId, this.ExternalDirectoryObjectId);
			this.UpdateSiteCollection();
			UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. UpdateSiteCollectionTask.RunInternal: Finished notifying SharePoint about group update", base.ActivityId);
			string value = string.Format("Notified SharePoint about group update;Group={0};ElapsedTime={1}", this.ExternalDirectoryObjectId, stopwatch.ElapsedMilliseconds);
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

		private void UpdateSiteCollection()
		{
			using (SharePointNotification sharePointNotification = new SharePointNotification(SharePointNotification.NotificationType.Update, this.ExternalDirectoryObjectId, base.AccessingUser.OrganizationId, base.ActAsUserCredentials, base.ActivityId))
			{
				if (this.Description != null)
				{
					sharePointNotification.SetPropertyValue("Description", this.Description, false);
				}
				if (!string.IsNullOrEmpty(this.DisplayName))
				{
					sharePointNotification.SetPropertyValue("DisplayName", this.DisplayName, false);
				}
				if (this.AddedOwners != null && this.AddedOwners.Length != 0)
				{
					sharePointNotification.AddOwners(this.AddedOwners);
				}
				if (this.RemovedOwners != null && this.RemovedOwners.Length != 0)
				{
					sharePointNotification.RemoveOwners(this.RemovedOwners);
				}
				if (this.AddedMembers != null && this.AddedMembers.Length != 0)
				{
					sharePointNotification.AddMembers(this.AddedMembers);
				}
				if (this.RemovedMembers != null && this.RemovedMembers.Length != 0)
				{
					sharePointNotification.RemoveMembers(this.RemovedMembers);
				}
				sharePointNotification.Execute();
			}
		}
	}
}
