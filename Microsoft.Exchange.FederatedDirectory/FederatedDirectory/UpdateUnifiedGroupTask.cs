using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Net.AAD;
using Microsoft.Exchange.UnifiedGroups;

namespace Microsoft.Exchange.FederatedDirectory
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UpdateUnifiedGroupTask : UnifiedGroupsTask
	{
		public UpdateUnifiedGroupTask(ADUser accessingUser, ExchangePrincipal accessingPrincipal, IRecipientSession adSession) : base(accessingUser, adSession)
		{
			this.accessingPrincipal = accessingPrincipal;
		}

		public string ExternalDirectoryObjectId { get; set; }

		public string SmtpAddress { get; set; }

		public string Description { get; set; }

		public string DisplayName { get; set; }

		public string[] AddedOwners { get; set; }

		public string[] RemovedOwners { get; set; }

		public string[] AddedMembers { get; set; }

		public string[] RemovedMembers { get; set; }

		public string[] AddedPendingMembers { get; set; }

		public string[] RemovedPendingMembers { get; set; }

		public bool? RequireSenderAuthenticationEnabled { get; set; }

		public bool? AutoSubscribeNewGroupMembers { get; set; }

		public CultureInfo Language { get; set; }

		protected override string TaskName
		{
			get
			{
				return "UpdateUnifiedGroupTask";
			}
		}

		protected override void RunInternal()
		{
			UnifiedGroupsTask.Tracer.TraceDebug<Guid, string, string>((long)this.GetHashCode(), "ActivityId={0}. UpdateUnifiedGroupTask.Run: User {1} is updating group {2}", base.ActivityId, this.accessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString(), this.ExternalDirectoryObjectId ?? this.SmtpAddress);
			base.CurrentAction = UnifiedGroupsTask.UnifiedGroupsAction.ResolveExternalIdentities;
			if (base.IsAADEnabled || base.IsSharePointEnabled)
			{
				this.GetIdentitiesForParameters();
			}
			UpdateUnifiedGroupTask.UpdateAADLinkResults updateAADLinkResults = null;
			base.CurrentAction = UnifiedGroupsTask.UnifiedGroupsAction.AADUpdate;
			if (base.IsAADEnabled)
			{
				UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. UpdateUnifiedGroupTask.Run: Updating group in AAD", base.ActivityId);
				updateAADLinkResults = this.UpdateAAD();
				UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. UpdateUnifiedGroupTask.Run: Finished updating group in AAD", base.ActivityId);
				base.CurrentAction = UnifiedGroupsTask.UnifiedGroupsAction.SharePointUpdate;
				if (base.IsSharePointEnabled)
				{
					UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. UpdateUnifiedGroupTask.Run: Enqueueing job to notify SharePoint about group update", base.ActivityId);
					UpdateSiteCollectionTask task = new UpdateSiteCollectionTask(base.AccessingUser, base.ADSession, base.ActivityId)
					{
						Description = this.Description,
						DisplayName = this.DisplayName,
						AddedOwners = this.GetSucceededLinkExternalIds(this.addedOwnersIdentities, updateAADLinkResults.FailedAddedOwners),
						RemovedOwners = this.GetSucceededLinkExternalIds(this.removedOwnersIdentities, updateAADLinkResults.FailedRemovedOwners),
						AddedMembers = this.GetSucceededLinkExternalIds(this.addedMembersIdentities, updateAADLinkResults.FailedAddedMembers),
						RemovedMembers = this.GetSucceededLinkExternalIds(this.removedMembersIdentities, updateAADLinkResults.FailedRemovedMembers),
						ExternalDirectoryObjectId = this.ExternalDirectoryObjectId
					};
					bool flag = UnifiedGroupsTask.QueueTask(task);
					UnifiedGroupsTask.Tracer.TraceDebug<Guid, bool>((long)this.GetHashCode(), "ActivityId={0}. UpdateUnifiedGroupTask.Run: Finished enqueueing job to notify SharePoint about group update. queued: {1}", base.ActivityId, flag);
					if (!flag)
					{
						UnifiedGroupsTask.Tracer.TraceError<Guid>((long)this.GetHashCode(), "ActivityId={0}. UpdateUnifiedGroupTask.Run: Failed to queue job to notify SharePoint about group update", base.ActivityId);
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
								"Failed to queue job to notify SharePoint about group update. ExternalDirectoryObjectId: " + this.ExternalDirectoryObjectId
							}
						});
					}
				}
				else
				{
					UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. UpdateUnifiedGroupTask.Run: SharePoint is not enabled, skipping notification about group creation", base.ActivityId);
				}
			}
			base.CurrentAction = UnifiedGroupsTask.UnifiedGroupsAction.ExchangeUpdate;
			UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. UpdateUnifiedGroupTask.Run: Updating group in Exchange", base.ActivityId);
			try
			{
				this.UpdateGroupMailbox(updateAADLinkResults);
			}
			catch (ExchangeAdaptorException arg)
			{
				if (updateAADLinkResults == null || !updateAADLinkResults.ContainsFailure())
				{
					throw;
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
						string.Format("AAD partially failed and Exchange threw an exception. ExternalDirectoryObjectId: {0}, {1}", this.ExternalDirectoryObjectId ?? this.SmtpAddress, arg)
					}
				});
			}
			UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. UpdateUnifiedGroupTask.Run: Finished updating group in Exchange", base.ActivityId);
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
					string.Format("Updated group. ExternalDirectoryObjectId: {0}, By: {1}", this.ExternalDirectoryObjectId ?? this.SmtpAddress, this.accessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString())
				}
			});
			this.ThrowIfPartialSuccess(updateAADLinkResults);
			base.CurrentAction = UnifiedGroupsTask.UnifiedGroupsAction.Completed;
		}

		private void GetIdentitiesForParameters()
		{
			this.identityMapping = new ObjectIdMapping(base.ADSession);
			this.identityMapping.Prefetch(new string[]
			{
				this.SmtpAddress
			});
			this.identityMapping.Prefetch(this.AddedOwners);
			this.identityMapping.Prefetch(this.RemovedOwners);
			this.identityMapping.Prefetch(this.AddedMembers);
			this.identityMapping.Prefetch(this.RemovedMembers);
			this.identityMapping.Prefetch(this.AddedPendingMembers);
			this.identityMapping.Prefetch(this.RemovedPendingMembers);
			if (string.IsNullOrEmpty(this.ExternalDirectoryObjectId))
			{
				this.ExternalDirectoryObjectId = this.identityMapping.GetIdentityFromSmtpAddress(this.SmtpAddress);
			}
			this.addedMembersIdentities = this.identityMapping.GetIdentitiesFromSmtpAddresses(this.AddedMembers);
			this.removedMembersIdentities = this.identityMapping.GetIdentitiesFromSmtpAddresses(this.RemovedMembers);
			this.addedOwnersIdentities = this.identityMapping.GetIdentitiesFromSmtpAddresses(this.AddedOwners);
			this.removedOwnersIdentities = this.identityMapping.GetIdentitiesFromSmtpAddresses(this.RemovedOwners);
			this.addedPendingMembersIdentities = this.identityMapping.GetIdentitiesFromSmtpAddresses(this.AddedPendingMembers);
			this.removedPendingMembersIdentities = this.identityMapping.GetIdentitiesFromSmtpAddresses(this.RemovedPendingMembers);
			if (this.identityMapping.InvalidSmtpAddresses.Count > 0)
			{
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
						string.Format("Unable to parse identities as SMTP Addresses: {0}", string.Join(",", this.identityMapping.InvalidSmtpAddresses))
					}
				});
			}
		}

		private UpdateUnifiedGroupTask.UpdateAADLinkResults UpdateAAD()
		{
			UpdateUnifiedGroupTask.UpdateAADLinkResults updateAADLinkResults = new UpdateUnifiedGroupTask.UpdateAADLinkResults();
			if (this.Description != null || this.DisplayName != null)
			{
				UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "Activityid={0}. UpdateUnifiedGroupTask.UpdateAAD: Calling UpdateGroup", base.ActivityId);
				base.AADClient.UpdateGroup(this.ExternalDirectoryObjectId, this.Description, null, this.DisplayName, null);
				UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "Activityid={0}. UpdateUnifiedGroupTask.UpdateAAD: Finished calling UpdateGroup", base.ActivityId);
			}
			if (this.addedMembersIdentities != null && this.addedMembersIdentities.Length != 0)
			{
				UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. UpdateUnifiedGroupTask.UpdateAAD: Adding members", base.ActivityId);
				updateAADLinkResults.FailedAddedMembers = base.AADClient.AddMembers(this.ExternalDirectoryObjectId, this.addedMembersIdentities);
				UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. UpdateUnifiedGroupTask.UpdateAAD: Finished adding members", base.ActivityId);
			}
			if (this.removedMembersIdentities != null && this.removedMembersIdentities.Length != 0)
			{
				UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. UpdateUnifiedGroupTask.UpdateAAD: Removing members", base.ActivityId);
				updateAADLinkResults.FailedRemovedMembers = base.AADClient.RemoveMembers(this.ExternalDirectoryObjectId, this.removedMembersIdentities);
				UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. UpdateUnifiedGroupTask.UpdateAAD: Finished removing members", base.ActivityId);
			}
			if (this.addedOwnersIdentities != null && this.addedOwnersIdentities.Length != 0)
			{
				UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. UpdateUnifiedGroupTask.UpdateAAD: Adding owners", base.ActivityId);
				updateAADLinkResults.FailedAddedOwners = base.AADClient.AddOwners(this.ExternalDirectoryObjectId, this.addedOwnersIdentities);
				UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. UpdateUnifiedGroupTask.UpdateAAD: Finished adding owners", base.ActivityId);
			}
			if (this.removedOwnersIdentities != null && this.removedOwnersIdentities.Length != 0)
			{
				UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. UpdateUnifiedGroupTask.UpdateAAD: Removing owners", base.ActivityId);
				updateAADLinkResults.FailedRemovedOwners = base.AADClient.RemoveOwners(this.ExternalDirectoryObjectId, this.removedOwnersIdentities);
				UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. UpdateUnifiedGroupTask.UpdateAAD: Finished removing owners", base.ActivityId);
			}
			if (this.addedPendingMembersIdentities != null && this.addedPendingMembersIdentities.Length != 0)
			{
				UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. UpdateUnifiedGroupTask.UpdateAAD: Adding pending members", base.ActivityId);
				updateAADLinkResults.FailedAddedPendingMembers = base.AADClient.AddPendingMembers(this.ExternalDirectoryObjectId, this.addedPendingMembersIdentities);
				UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. UpdateUnifiedGroupTask.UpdateAAD: Finished adding pending members", base.ActivityId);
			}
			if (this.removedPendingMembersIdentities != null && this.removedPendingMembersIdentities.Length != 0)
			{
				UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. UpdateUnifiedGroupTask.UpdateAAD: Removing pending members", base.ActivityId);
				updateAADLinkResults.FailedRemovedPendingMembers = base.AADClient.RemovePendingMembers(this.ExternalDirectoryObjectId, this.removedPendingMembersIdentities);
				UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. UpdateUnifiedGroupTask.UpdateAAD: Finished removing pending members", base.ActivityId);
			}
			return updateAADLinkResults;
		}

		private void UpdateGroupMailbox(UpdateUnifiedGroupTask.UpdateAADLinkResults results)
		{
			using (PSLocalTask<SetGroupMailbox, object> pslocalTask = CmdletTaskFactory.Instance.CreateSetGroupMailboxTask(this.accessingPrincipal))
			{
				pslocalTask.CaptureAdditionalIO = true;
				pslocalTask.Task.ExecutingUser = new RecipientIdParameter(this.accessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
				pslocalTask.Task.Identity = new RecipientIdParameter(this.ExternalDirectoryObjectId ?? this.SmtpAddress);
				if (this.Description != null)
				{
					pslocalTask.Task.Description = this.Description;
				}
				if (!string.IsNullOrEmpty(this.DisplayName))
				{
					pslocalTask.Task.DisplayName = this.DisplayName;
				}
				string[] array = (results != null) ? this.GetSucceededLinkSmtpAddresses(this.addedMembersIdentities, results.FailedAddedMembers) : this.AddedMembers;
				if (array != null && array.Length != 0)
				{
					pslocalTask.Task.AddedMembers = (from o in array
					select new RecipientIdParameter(o)).ToArray<RecipientIdParameter>();
				}
				string[] array2 = (results != null) ? this.GetSucceededLinkSmtpAddresses(this.removedMembersIdentities, results.FailedRemovedMembers) : this.RemovedMembers;
				if (array2 != null && array2.Length != 0)
				{
					pslocalTask.Task.RemovedMembers = (from o in array2
					select new RecipientIdParameter(o)).ToArray<RecipientIdParameter>();
				}
				string[] array3 = (results != null) ? this.GetSucceededLinkSmtpAddresses(this.addedOwnersIdentities, results.FailedAddedOwners) : this.AddedOwners;
				if (array3 != null && array3.Length != 0)
				{
					pslocalTask.Task.AddOwners = (from o in array3
					select new RecipientIdParameter(o)).ToArray<RecipientIdParameter>();
				}
				string[] array4 = (results != null) ? this.GetSucceededLinkSmtpAddresses(this.removedOwnersIdentities, results.FailedRemovedOwners) : this.RemovedOwners;
				if (array4 != null && array4.Length != 0)
				{
					pslocalTask.Task.RemoveOwners = (from o in array4
					select new RecipientIdParameter(o)).ToArray<RecipientIdParameter>();
				}
				if (this.RequireSenderAuthenticationEnabled != null)
				{
					pslocalTask.Task.RequireSenderAuthenticationEnabled = this.RequireSenderAuthenticationEnabled.Value;
				}
				if (this.AutoSubscribeNewGroupMembers != null)
				{
					pslocalTask.Task.AutoSubscribeNewGroupMembers = this.AutoSubscribeNewGroupMembers.Value;
				}
				if (this.Language != null)
				{
					pslocalTask.Task.Language = this.Language;
				}
				UnifiedGroupsTask.Tracer.TraceDebug<Guid, PSLocalTaskLogging.SetGroupMailboxToString>((long)this.GetHashCode(), "ActivityId={0}. {1}", base.ActivityId, new PSLocalTaskLogging.SetGroupMailboxToString(pslocalTask.Task));
				pslocalTask.Task.Execute();
				UnifiedGroupsTask.Tracer.TraceDebug<Guid, PSLocalTaskLogging.TaskOutputToString>((long)this.GetHashCode(), "ActivityId={0}. {1}", base.ActivityId, new PSLocalTaskLogging.TaskOutputToString(pslocalTask.AdditionalIO));
				if (pslocalTask.Error != null)
				{
					UnifiedGroupsTask.Tracer.TraceError<Guid, string>((long)this.GetHashCode(), "ActivityId={0}. UpdateUnifiedGroupTask.UpdateGroupMailbox() failed: {1}", base.ActivityId, pslocalTask.ErrorMessage);
					throw new ExchangeAdaptorException(Strings.GroupMailboxFailedUpdate(this.ExternalDirectoryObjectId ?? this.SmtpAddress, pslocalTask.ErrorMessage));
				}
			}
		}

		private void ThrowIfPartialSuccess(UpdateUnifiedGroupTask.UpdateAADLinkResults results)
		{
			if (results != null && results.ContainsFailure())
			{
				base.CurrentAction = UnifiedGroupsTask.UnifiedGroupsAction.AADUpdate;
				AADPartialFailureException ex = new AADPartialFailureException(string.Format("Partially failed to update group: {0}", this.ExternalDirectoryObjectId ?? this.SmtpAddress))
				{
					FailedAddedMembers = this.GetOriginalFailedLinks(results.FailedAddedMembers),
					FailedRemovedMembers = this.GetOriginalFailedLinks(results.FailedRemovedMembers),
					FailedAddedOwners = this.GetOriginalFailedLinks(results.FailedAddedOwners),
					FailedRemovedOwners = this.GetOriginalFailedLinks(results.FailedRemovedOwners),
					FailedAddedPendingMembers = this.GetOriginalFailedLinks(results.FailedAddedPendingMembers),
					FailedRemovedPendingMembers = this.GetOriginalFailedLinks(results.FailedRemovedPendingMembers)
				};
				throw ex;
			}
		}

		private string[] GetSucceededLinkExternalIds(string[] allLinks, AADClient.LinkResult[] failedLinkResults)
		{
			if (allLinks == null)
			{
				return null;
			}
			if (failedLinkResults == null)
			{
				return allLinks;
			}
			string[] second = (from linkResult in failedLinkResults
			select linkResult.FailedLink).ToArray<string>();
			string[] array = allLinks.Except(second).ToArray<string>();
			if (array.Length <= 0)
			{
				return null;
			}
			return array;
		}

		private string[] GetSucceededLinkSmtpAddresses(string[] allLinks, AADClient.LinkResult[] failedLinkResults)
		{
			string[] succeededLinkExternalIds = this.GetSucceededLinkExternalIds(allLinks, failedLinkResults);
			if (succeededLinkExternalIds == null)
			{
				return null;
			}
			return (from link in succeededLinkExternalIds
			select this.identityMapping.GetSmtpAddressFromIdentity(link)).ToArray<string>();
		}

		private AADPartialFailureException.FailedLink[] GetOriginalFailedLinks(AADClient.LinkResult[] failedLinkResults)
		{
			if (failedLinkResults == null)
			{
				return null;
			}
			List<AADPartialFailureException.FailedLink> list = new List<AADPartialFailureException.FailedLink>(failedLinkResults.Length);
			foreach (AADClient.LinkResult linkResult in failedLinkResults)
			{
				list.Add(new AADPartialFailureException.FailedLink
				{
					Link = this.identityMapping.GetSmtpAddressFromIdentity(linkResult.FailedLink),
					Exception = linkResult.Exception
				});
			}
			if (list.Count <= 0)
			{
				return null;
			}
			return list.ToArray();
		}

		private string[] addedOwnersIdentities;

		private string[] removedOwnersIdentities;

		private string[] addedMembersIdentities;

		private string[] removedMembersIdentities;

		private string[] addedPendingMembersIdentities;

		private string[] removedPendingMembersIdentities;

		private ExchangePrincipal accessingPrincipal;

		private ObjectIdMapping identityMapping;

		private class UpdateAADLinkResults
		{
			public AADClient.LinkResult[] FailedAddedMembers { get; set; }

			public AADClient.LinkResult[] FailedRemovedMembers { get; set; }

			public AADClient.LinkResult[] FailedAddedOwners { get; set; }

			public AADClient.LinkResult[] FailedRemovedOwners { get; set; }

			public AADClient.LinkResult[] FailedAddedPendingMembers { get; set; }

			public AADClient.LinkResult[] FailedRemovedPendingMembers { get; set; }

			public bool ContainsFailure()
			{
				return this.FailedAddedMembers != null || this.FailedRemovedMembers != null || this.FailedAddedOwners != null || this.FailedRemovedOwners != null || this.FailedAddedPendingMembers != null || this.FailedRemovedPendingMembers != null;
			}
		}
	}
}
