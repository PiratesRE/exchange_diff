using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.AAD;
using Microsoft.Exchange.UnifiedGroups;

namespace Microsoft.Exchange.FederatedDirectory
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DeleteUnifiedGroupTask : UnifiedGroupsTask
	{
		public DeleteUnifiedGroupTask(ADUser accessingUser, ExchangePrincipal accessingPrincipal, IRecipientSession adSession) : base(accessingUser, adSession)
		{
			this.accessingPrincipal = accessingPrincipal;
		}

		public string ExternalDirectoryObjectId { get; set; }

		public string SmtpAddress { get; set; }

		protected override string TaskName
		{
			get
			{
				return "DeleteUnifiedGroupTask";
			}
		}

		protected override void RunInternal()
		{
			UnifiedGroupsTask.Tracer.TraceDebug<Guid, string, string>((long)this.GetHashCode(), "ActivityId={0}. DeleteUnifiedGroupTask.Run: User {1} is deleting group {2}", base.ActivityId, this.accessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString(), this.ExternalDirectoryObjectId ?? this.SmtpAddress);
			base.CurrentAction = UnifiedGroupsTask.UnifiedGroupsAction.AADDelete;
			try
			{
				UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. DeleteUnifiedGroupTask.Run: Deleting group in AAD", base.ActivityId);
				this.DeleteAAD();
				UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. DeleteUnifiedGroupTask.Run: Finished deleting group in AAD", base.ActivityId);
			}
			catch (AADDataException ex)
			{
				if (ex.Code != AADDataException.AADCode.Request_ResourceNotFound)
				{
					throw;
				}
				UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. DeleteUnifiedGroupTask.Run: Group not found in AAD", base.ActivityId);
			}
			base.CurrentAction = UnifiedGroupsTask.UnifiedGroupsAction.SharePointDelete;
			if (base.IsSharePointEnabled)
			{
				UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. DeleteUnifiedGroupTask.Run: Enqueuing job to notify SharePoint about group deletion", base.ActivityId);
				DeleteSiteCollectionTask task = new DeleteSiteCollectionTask(base.AccessingUser, base.ADSession, base.ActivityId)
				{
					ExternalDirectoryObjectId = this.ExternalDirectoryObjectId,
					SmtpAddress = this.SmtpAddress
				};
				bool flag = UnifiedGroupsTask.QueueTask(task);
				UnifiedGroupsTask.Tracer.TraceDebug<Guid, bool>((long)this.GetHashCode(), "ActivityId={0}. DeleteUnifiedGroupTask.Run: Finished enqueuing job to notify SharePoint about group deletion. queued: {1}", base.ActivityId, flag);
				if (!flag)
				{
					UnifiedGroupsTask.Tracer.TraceError<Guid>((long)this.GetHashCode(), "ActivityId={0}. Failed to queue job to notify SharePoint about group deletion", base.ActivityId);
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
							"Failed to queue job to notify SharePoint about group deletion. ExternalDirectoryObjectId: " + this.ExternalDirectoryObjectId
						}
					});
				}
			}
			else
			{
				UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. DeleteUnifiedGroupTask.Run: SharePoint is not enabled, skipping notification about group creation", base.ActivityId);
			}
			base.CurrentAction = UnifiedGroupsTask.UnifiedGroupsAction.ExchangeDelete;
			UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. DeleteUnifiedGroupTask.Run: Enqueuing job to delete group in Exchange", base.ActivityId);
			bool flag2 = UnifiedGroupsTask.QueueTask(new DeleteGroupMailboxTask(base.AccessingUser, this.accessingPrincipal, base.ADSession)
			{
				ExternalDirectoryObjectId = this.ExternalDirectoryObjectId,
				SmtpAddress = this.SmtpAddress
			});
			UnifiedGroupsTask.Tracer.TraceDebug<Guid, bool>((long)this.GetHashCode(), "ActivityId={0}. DeleteUnifiedGroupTask.Run: Finished enqueuing job to delete group in Exchange. queued: {1}", base.ActivityId, flag2);
			if (!flag2)
			{
				UnifiedGroupsTask.Tracer.TraceError<Guid>((long)this.GetHashCode(), "ActivityId={0}. Failed to queue job to delete group in Exchange", base.ActivityId);
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
						string.Format("Failed to queue job to delete group in Exchange. ExternalDirectoryObjectId: {0}", this.ExternalDirectoryObjectId)
					}
				});
			}
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
					string.Format("Deleted group. ExternalDirectoryObjectId: {0}, By: {1}", this.ExternalDirectoryObjectId ?? this.SmtpAddress, this.accessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString())
				}
			});
			base.CurrentAction = UnifiedGroupsTask.UnifiedGroupsAction.Completed;
		}

		private void DeleteAAD()
		{
			if (!base.IsAADEnabled)
			{
				return;
			}
			string objectId;
			if (!string.IsNullOrEmpty(this.ExternalDirectoryObjectId))
			{
				objectId = this.ExternalDirectoryObjectId;
			}
			else
			{
				ObjectIdMapping objectIdMapping = new ObjectIdMapping(base.ADSession);
				objectIdMapping.Prefetch(new string[]
				{
					this.SmtpAddress
				});
				objectId = objectIdMapping.GetIdentityFromSmtpAddress(this.SmtpAddress);
			}
			base.AADClient.DeleteGroup(objectId);
		}

		private readonly ExchangePrincipal accessingPrincipal;
	}
}
