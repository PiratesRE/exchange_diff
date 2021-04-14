using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.UnifiedGroups;

namespace Microsoft.Exchange.FederatedDirectory
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CreateSiteCollectionTask : UnifiedGroupsTask
	{
		public CreateSiteCollectionTask(ADUser accessingUser, IRecipientSession adSession, Guid activityId) : base(accessingUser, adSession, activityId)
		{
		}

		public string Name { get; set; }

		public string Alias { get; set; }

		public string Description { get; set; }

		public ModernGroupTypeInfo Type { get; set; }

		public string ExternalDirectoryObjectId { get; set; }

		protected override string TaskName
		{
			get
			{
				return "CreateSiteCollectionTask";
			}
		}

		protected override void RunInternal()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			base.CurrentAction = UnifiedGroupsTask.UnifiedGroupsAction.SharePointCreate;
			UnifiedGroupsTask.Tracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "ActivityId={0}. CreateSiteCollectionTask.RunInternal: Notifying SharePoint about group creation: {1}", base.ActivityId, this.ExternalDirectoryObjectId);
			this.CreateSiteCollection();
			UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. CreateSiteCollectionTask.RunInternal: Finished notifying SharePoint about group creation", base.ActivityId);
			string value = string.Format("Notified SharePoint about group creation;Group={0};ElapsedTime={1}", this.ExternalDirectoryObjectId, stopwatch.ElapsedMilliseconds);
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

		private void CreateSiteCollection()
		{
			using (SharePointNotification sharePointNotification = new SharePointNotification(SharePointNotification.NotificationType.Create, this.ExternalDirectoryObjectId, base.AccessingUser.OrganizationId, base.ActAsUserCredentials, base.ActivityId))
			{
				sharePointNotification.SetPropertyValue("Alias", this.Alias, false);
				sharePointNotification.SetPropertyValue("DisplayName", this.Name, false);
				sharePointNotification.SetPropertyValue("IsPublic", this.Type == ModernGroupTypeInfo.Public, false);
				if (!string.IsNullOrEmpty(this.Description))
				{
					sharePointNotification.SetPropertyValue("Description", this.Description, false);
				}
				sharePointNotification.SetAllowAccessTo(this.Type == ModernGroupTypeInfo.Public);
				sharePointNotification.Execute();
			}
		}
	}
}
