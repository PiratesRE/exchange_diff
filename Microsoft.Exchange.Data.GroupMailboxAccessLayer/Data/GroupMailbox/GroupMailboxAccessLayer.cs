using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GroupMailboxAccessLayer
	{
		internal GroupMailboxAccessLayer(IRecipientSession adSession, IStoreBuilder storeProviderBuilder, IMailboxAssociationPerformanceTracker performanceTracker, IExtensibleLogger logger, string clientString)
		{
			ArgumentValidator.ThrowIfNull("adSession", adSession);
			ArgumentValidator.ThrowIfNull("storeProviderBuilder", storeProviderBuilder);
			ArgumentValidator.ThrowIfNull("performanceTracker", performanceTracker);
			ArgumentValidator.ThrowIfNull("logger", logger);
			this.adSession = adSession;
			this.storeProviderBuilder = storeProviderBuilder;
			this.PerformanceTracker = performanceTracker;
			this.Logger = logger;
			this.mailboxCollectionBuilder = new MailboxCollectionBuilder(adSession);
			this.clientString = clientString;
		}

		public IMailboxAssociationPerformanceTracker PerformanceTracker { get; private set; }

		public IExtensibleLogger Logger { get; private set; }

		public static void Execute(string operationDescription, IRecipientSession adSession, MailboxSession localStoreSession, Action<GroupMailboxAccessLayer> action)
		{
			ArgumentValidator.ThrowIfNull("localStoreSession", localStoreSession);
			GroupMailboxAccessLayer.InternalExecute(operationDescription, adSession, localStoreSession, localStoreSession.MailboxGuid, localStoreSession.OrganizationId, localStoreSession.ClientInfoString, action);
		}

		public static void Execute(string operationDescription, IRecipientSession adSession, Guid logCorrelationMailboxGuid, OrganizationId organizationId, string clientInfoString, Action<GroupMailboxAccessLayer> action)
		{
			GroupMailboxAccessLayer.InternalExecute(operationDescription, adSession, null, logCorrelationMailboxGuid, organizationId, clientInfoString, action);
		}

		public IEnumerable<GroupMailbox> GetPinnedGroups(UserMailboxLocator user, bool loadAllDetails = false)
		{
			using (IAssociationStore storeProvider = this.storeProviderBuilder.Create(user, this.PerformanceTracker))
			{
				GroupAssociationAdaptor adaptor = new GroupAssociationAdaptor(storeProvider, this.adSession, user);
				GetPinAssociations command = new GetPinAssociations(adaptor);
				IEnumerable<MailboxAssociation> pinAssociations = command.Execute(null);
				IEnumerable<GroupMailbox> mailboxes = this.mailboxCollectionBuilder.BuildGroupMailboxes(pinAssociations, loadAllDetails);
				foreach (GroupMailbox mailbox in mailboxes)
				{
					yield return mailbox;
				}
			}
			yield break;
		}

		public IEnumerable<UserMailbox> GetUnseenMembers(GroupMailboxLocator group, IEnumerable<UserMailboxLocator> users)
		{
			IEnumerable<UserMailbox> result;
			using (IAssociationStore associationStore = this.storeProviderBuilder.Create(group, this.PerformanceTracker))
			{
				UnseenDataUserAssociationAdaptor adaptor = new UnseenDataUserAssociationAdaptor(associationStore, this.adSession, group);
				GetMembershipAssociations getMembershipAssociations = new GetMembershipAssociations(adaptor);
				IEnumerable<MailboxAssociation> enumerable = getMembershipAssociations.Execute(null);
				Dictionary<string, MailboxAssociation> dictionary = new Dictionary<string, MailboxAssociation>(enumerable.Count<MailboxAssociation>());
				foreach (MailboxAssociation mailboxAssociation in enumerable)
				{
					if (string.IsNullOrEmpty(mailboxAssociation.User.ExternalId))
					{
						GroupMailboxAccessLayer.LogWarning(this.Logger, "GetUnseenMembers", string.Format("External Id is null or empty. LegacyDn - {0}", mailboxAssociation.User.LegacyDn));
					}
					else if (dictionary.ContainsKey(mailboxAssociation.User.ExternalId))
					{
						GroupMailboxAccessLayer.LogWarning(this.Logger, "GetUnseenMembers", string.Format("Duplicate External Id. ExternalId - {0}. LegacyDn - {1}", mailboxAssociation.User.ExternalId, mailboxAssociation.User.LegacyDn));
					}
					else
					{
						dictionary.Add(mailboxAssociation.User.ExternalId, mailboxAssociation);
					}
				}
				List<UserMailbox> list = new List<UserMailbox>();
				foreach (UserMailboxLocator userMailboxLocator in users)
				{
					MailboxAssociation association = null;
					if (!dictionary.TryGetValue(userMailboxLocator.ExternalId, out association))
					{
						association = this.CreateMailboxAssociationWithDefaultValues(userMailboxLocator, group);
					}
					UserMailboxBuilder userMailboxBuilder = new UserMailboxBuilder(userMailboxLocator, null);
					list.Add(userMailboxBuilder.BuildFromAssociation(association).Mailbox);
				}
				result = list;
			}
			return result;
		}

		public IEnumerable<UserMailbox> GetMembershipChanges(GroupMailboxLocator group, ExDateTime changeDate, bool loadAllDetails = false, int? maxItems = null)
		{
			using (IAssociationStore storeProvider = this.storeProviderBuilder.Create(group, this.PerformanceTracker))
			{
				UserAssociationAdaptor adaptor = new UserAssociationAdaptor(storeProvider, this.adSession, group);
				GetAssociationCommand command = new GetMembershipChangedAfterAssociations(adaptor, changeDate);
				IEnumerable<MailboxAssociation> members = command.Execute(maxItems);
				RpcAssociationReplicator rpcReplicator = new RpcAssociationReplicator(this.Logger, storeProvider.ServerFullyQualifiedDomainName);
				ReplicatorEnabledAssociationEnumerator eventualConsistencyEnumerator = new ReplicatorEnabledAssociationEnumerator(rpcReplicator, members, storeProvider);
				try
				{
					IEnumerable<UserMailbox> mailboxes = this.mailboxCollectionBuilder.BuildUserMailboxes(group, eventualConsistencyEnumerator, loadAllDetails);
					foreach (UserMailbox mailbox in mailboxes)
					{
						yield return mailbox;
					}
				}
				finally
				{
					eventualConsistencyEnumerator.TriggerReplication(adaptor);
				}
			}
			yield break;
		}

		public IEnumerable<UserMailbox> GetMembers(GroupMailboxLocator group, bool loadAllDetails = false, int? maxItems = null)
		{
			using (IAssociationStore storeProvider = this.storeProviderBuilder.Create(group, this.PerformanceTracker))
			{
				UserAssociationAdaptor adaptor = new UserAssociationAdaptor(storeProvider, this.adSession, group);
				GetAssociationCommand command = new GetMembershipAssociations(adaptor);
				IEnumerable<MailboxAssociation> members = command.Execute(maxItems);
				RpcAssociationReplicator rpcReplicator = new RpcAssociationReplicator(this.Logger, storeProvider.ServerFullyQualifiedDomainName);
				ReplicatorEnabledAssociationEnumerator eventualConsistencyEnumerator = new ReplicatorEnabledAssociationEnumerator(rpcReplicator, members, storeProvider);
				try
				{
					IEnumerable<UserMailbox> mailboxes = this.mailboxCollectionBuilder.BuildUserMailboxes(group, eventualConsistencyEnumerator, loadAllDetails);
					foreach (UserMailbox mailbox in mailboxes)
					{
						yield return mailbox;
					}
				}
				finally
				{
					eventualConsistencyEnumerator.TriggerReplication(adaptor);
				}
			}
			yield break;
		}

		public IEnumerable<UserMailbox> GetMembers(GroupMailboxLocator group, IEnumerable<UserMailboxLocator> users, bool loadAllDetails = false)
		{
			using (IAssociationStore storeProvider = this.storeProviderBuilder.Create(group, this.PerformanceTracker))
			{
				UserAssociationAdaptor adaptor = new UserAssociationAdaptor(storeProvider, this.adSession, group);
				IEnumerable<UserMailbox> mailboxes = this.mailboxCollectionBuilder.BuildUserMailboxes(group, from u in users
				select new GetMemberAssociation(adaptor, u).Execute(), loadAllDetails);
				foreach (UserMailbox mailbox in mailboxes)
				{
					yield return mailbox;
				}
			}
			yield break;
		}

		public UserMailbox GetMember(GroupMailboxLocator group, UserMailboxLocator user, bool loadAllDetails = false)
		{
			UserMailbox result;
			using (IAssociationStore associationStore = this.storeProviderBuilder.Create(group, this.PerformanceTracker))
			{
				UserAssociationAdaptor adaptor = new UserAssociationAdaptor(associationStore, this.adSession, group);
				GetMemberAssociation getMemberAssociation = new GetMemberAssociation(adaptor, user);
				MailboxAssociation item = getMemberAssociation.Execute();
				result = this.mailboxCollectionBuilder.BuildUserMailboxes(group, new List<MailboxAssociation>(1)
				{
					item
				}, loadAllDetails).FirstOrDefault<UserMailbox>();
			}
			return result;
		}

		public IEnumerable<UserMailbox> GetEscalatedMembers(GroupMailboxLocator group, bool loadAllDetails = false)
		{
			using (IAssociationStore storeProvider = this.storeProviderBuilder.Create(group, this.PerformanceTracker))
			{
				foreach (UserMailbox mailbox in this.GetEscalatedMembersInternal(storeProvider, group, loadAllDetails))
				{
					yield return mailbox;
				}
			}
			yield break;
		}

		public IEnumerable<GroupMailbox> GetJoinedGroups(UserMailboxLocator user, bool loadAllDetails = false)
		{
			using (IAssociationStore storeProvider = this.storeProviderBuilder.Create(user, this.PerformanceTracker))
			{
				GroupAssociationAdaptor adaptor = new GroupAssociationAdaptor(storeProvider, this.adSession, user);
				GetMembershipAssociations command = new GetMembershipAssociations(adaptor);
				IEnumerable<MailboxAssociation> joinAssociations = command.Execute(null);
				IEnumerable<GroupMailbox> mailboxes = this.mailboxCollectionBuilder.BuildGroupMailboxes(joinAssociations, loadAllDetails);
				foreach (GroupMailbox mailbox in mailboxes)
				{
					yield return mailbox;
				}
			}
			yield break;
		}

		public void SetGroupPinState(UserMailboxLocator user, GroupMailboxLocator group, bool isPinned, bool isModernGroupsNewArchitecture)
		{
			using (IAssociationStore associationStore = this.storeProviderBuilder.Create(user, this.PerformanceTracker))
			{
				this.LogCommandExecution(isPinned ? "PinGroup" : "UnpinGroup", group, new UserMailboxLocator[]
				{
					user
				});
				GroupAssociationAdaptor masterAdaptor = new GroupAssociationAdaptor(associationStore, this.adSession, user);
				SetGroupPinState setGroupPinState = new SetGroupPinState(this.Logger, null, isPinned, masterAdaptor, group, this.PerformanceTracker, isModernGroupsNewArchitecture);
				setGroupPinState.Execute();
			}
		}

		public IEnumerable<UserMailbox> GetGroupPinners(GroupMailboxLocator group, bool loadAllDetails = false)
		{
			using (IAssociationStore storeProvider = this.storeProviderBuilder.Create(group, this.PerformanceTracker))
			{
				UserAssociationAdaptor adaptor = new UserAssociationAdaptor(storeProvider, this.adSession, group);
				GetPinAssociations command = new GetPinAssociations(adaptor);
				IEnumerable<MailboxAssociation> pinAssociations = command.Execute(null);
				IEnumerable<UserMailbox> mailboxes = this.mailboxCollectionBuilder.BuildUserMailboxes(group, pinAssociations, loadAllDetails);
				foreach (UserMailbox mailbox in mailboxes)
				{
					yield return mailbox;
				}
			}
			yield break;
		}

		public void SetLastVisitedDate(UserMailboxLocator user, GroupMailboxLocator group, ExDateTime lastVisitedDate)
		{
			using (IAssociationStore associationStore = this.storeProviderBuilder.Create(group, this.PerformanceTracker))
			{
				this.LogCommandExecution("SetLastVisitedDate", group, new UserMailboxLocator[]
				{
					user
				});
				UserAssociationAdaptor masterAdaptor = new UserAssociationAdaptor(associationStore, this.adSession, group);
				SetLastVisitedDate setLastVisitedDate = new SetLastVisitedDate(this.Logger, lastVisitedDate, masterAdaptor, user);
				setLastVisitedDate.Execute();
			}
		}

		public void SetEscalate(UserMailboxLocator user, GroupMailboxLocator group, bool shouldEscalate, SmtpAddress userSmtpAddress, int maxNumberOfSubscribers = 400)
		{
			using (IAssociationStore associationStore = this.storeProviderBuilder.Create(group, this.PerformanceTracker))
			{
				this.LogCommandExecution("SetEscalate", group, new UserMailboxLocator[]
				{
					user
				});
				UserAssociationAdaptor masterAdaptor = new UserAssociationAdaptor(associationStore, this.adSession, group);
				SetEscalate setEscalate = new SetEscalate(this.Logger, shouldEscalate, userSmtpAddress, masterAdaptor, user, maxNumberOfSubscribers);
				setEscalate.Execute();
			}
		}

		public void SetMembershipState(ADRecipient joinedBy, UserMailboxLocator[] users, GroupMailboxLocator group, bool isMember)
		{
			UserMailboxLocator[] joiningUsers = isMember ? users : null;
			UserMailboxLocator[] departingUsers = (!isMember) ? users : null;
			this.SetMembershipState(joinedBy, joiningUsers, departingUsers, group);
		}

		public void SetMembershipState(ADRecipient joinedBy, UserMailboxLocator[] joiningUsers, UserMailboxLocator[] departingUsers, GroupMailboxLocator group)
		{
			string joinedBy2 = string.Empty;
			if (joinedBy != null)
			{
				joinedBy2 = ((SmtpAddress)joinedBy[ADRecipientSchema.PrimarySmtpAddress]).ToString();
			}
			using (IAssociationStore associationStore = this.storeProviderBuilder.Create(group, this.PerformanceTracker))
			{
				ADUser groupMailbox = group.FindAdUser();
				WelcomeToGroupMessageTemplate messageComposerBuilder = new WelcomeToGroupMessageTemplate(groupMailbox, associationStore.MailboxOwner, joinedBy);
				EmailNotificationHandler joinNotificationHandler = new EmailNotificationHandler(groupMailbox, associationStore.MailboxOwner, this.clientString, messageComposerBuilder);
				QueuedInProcessAssociationReplicator associationReplicator = new QueuedInProcessAssociationReplicator(group, this.adSession, associationStore.ServerFullyQualifiedDomainName, this.clientString);
				try
				{
					UserAssociationAdaptor userAssociationAdaptor = new UserAssociationAdaptor(associationStore, this.adSession, group);
					if (departingUsers != null && departingUsers.Length > 0)
					{
						this.LogCommandExecution("LeaveGroup", group, departingUsers);
						SetUserMembershipState setUserMembershipState = new SetUserMembershipState(this.Logger, associationReplicator, false, joinedBy2, userAssociationAdaptor, this.adSession, departingUsers);
						setUserMembershipState.Execute();
					}
					if (joiningUsers != null && joiningUsers.Length > 0)
					{
						this.LogCommandExecution("JoinGroup", group, joiningUsers);
						userAssociationAdaptor.OnAfterJoin += joinNotificationHandler.AddNotificationRecipient;
						SetUserMembershipState setUserMembershipState2 = new SetUserMembershipState(this.Logger, associationReplicator, true, joinedBy2, userAssociationAdaptor, this.adSession, joiningUsers);
						setUserMembershipState2.Execute();
						userAssociationAdaptor.OnAfterJoin -= joinNotificationHandler.AddNotificationRecipient;
					}
				}
				finally
				{
					Task.Run(delegate()
					{
						associationReplicator.ReplicateQueuedAssociations();
					}).ContinueWith(delegate(Task t)
					{
						joinNotificationHandler.SendNotification();
					});
				}
			}
		}

		public void UpdateSlaveDataFromMailboxAssociation(MasterMailboxType masterMailboxData, MailboxAssociationType associationType)
		{
			GroupMailboxAccessLayer.Tracer.TraceDebug<MasterMailboxType, MailboxAssociationType>((long)this.GetHashCode(), "GroupMailboxAccessLayer::UpdateSlaveDataFromMailboxAssociation. Replicating data from mailbox: {0}. Association data: {1}", masterMailboxData, associationType);
			if (masterMailboxData.MailboxType != UserMailboxLocator.MailboxLocatorType && masterMailboxData.MailboxType != GroupMailboxLocator.MailboxLocatorType)
			{
				throw new InvalidOperationException("UpdateSlaveDataFromMailboxAssociation: Invalid MasterMailboxType");
			}
			MailboxAssociation mailboxAssociation = EwsAssociationDataConverter.Convert(associationType, this.adSession);
			IMailboxLocator mailboxLocator;
			if (!(masterMailboxData.MailboxType == GroupMailboxLocator.MailboxLocatorType))
			{
				IMailboxLocator group = mailboxAssociation.Group;
				mailboxLocator = group;
			}
			else
			{
				mailboxLocator = mailboxAssociation.User;
			}
			IMailboxLocator targetMailbox = mailboxLocator;
			using (IAssociationStore associationStore = this.storeProviderBuilder.Create(targetMailbox, this.PerformanceTracker))
			{
				this.LogCommandExecution("UpdateSlaveDataFromMailboxAssociation", mailboxAssociation.Group, new UserMailboxLocator[]
				{
					mailboxAssociation.User
				});
				BaseAssociationAdaptor baseAssociationAdaptor = (masterMailboxData.MailboxType == GroupMailboxLocator.MailboxLocatorType) ? new GroupAssociationAdaptor(associationStore, this.adSession, mailboxAssociation.User) : new UserAssociationAdaptor(associationStore, this.adSession, mailboxAssociation.Group);
				baseAssociationAdaptor.UseAlternateLocatorLookup = true;
				baseAssociationAdaptor.MasterMailboxData = masterMailboxData;
				baseAssociationAdaptor.ReplicateAssociation(mailboxAssociation);
				if (masterMailboxData.MailboxType == GroupMailboxLocator.MailboxLocatorType && mailboxAssociation.IsMember)
				{
					TimeSpan latency = ExDateTime.UtcNow - mailboxAssociation.JoinDate;
					this.LogPerformanceCounter(MailboxAssociationLogSchema.PerformanceCounterName.JoinGroupAssociationReplication, latency, "Group={0}, User={1}", new object[]
					{
						mailboxAssociation.Group.ExternalId,
						mailboxAssociation.User.ExternalId
					});
				}
			}
		}

		public void ReplicateOutOfSyncAssociation(GroupMailboxLocator masterLocator, params UserMailboxLocator[] slaveLocators)
		{
			this.ReplicateOutOfSyncAssociation((IAssociationStore storeProvider) => new UserAssociationAdaptor(storeProvider, this.adSession, masterLocator), masterLocator, slaveLocators);
		}

		public void ReplicateOutOfSyncAssociation(GroupMailboxLocator masterLocator)
		{
			this.ReplicateOutOfSyncAssociation((IAssociationStore storeProvider) => new UserAssociationAdaptor(storeProvider, this.adSession, masterLocator), masterLocator);
		}

		public void ReplicateOutOfSyncAssociation(UserMailboxLocator masterLocator, params GroupMailboxLocator[] slaveLocators)
		{
			this.ReplicateOutOfSyncAssociation((IAssociationStore storeProvider) => new GroupAssociationAdaptor(storeProvider, this.adSession, masterLocator), masterLocator, slaveLocators);
		}

		public void ReplicateOutOfSyncAssociation(UserMailboxLocator masterLocator)
		{
			this.ReplicateOutOfSyncAssociation((IAssociationStore storeProvider) => new GroupAssociationAdaptor(storeProvider, this.adSession, masterLocator), masterLocator);
		}

		internal GroupMailbox GetGroupMailbox(GroupMailboxLocator group, UserMailboxLocator user, bool loadAllDetails = false)
		{
			GroupMailbox mailbox;
			using (IAssociationStore associationStore = this.storeProviderBuilder.Create(group, this.PerformanceTracker))
			{
				UserAssociationAdaptor userAssociationAdaptor = new UserAssociationAdaptor(associationStore, this.adSession, group);
				MailboxAssociation association = userAssociationAdaptor.GetAssociation(user);
				GroupMailboxBuilder groupMailboxBuilder = new GroupMailboxBuilder(group);
				groupMailboxBuilder.BuildFromAssociation(association);
				if (loadAllDetails)
				{
					ADUser aduser = group.FindAdUser();
					if (aduser == null)
					{
						GroupMailboxAccessLayer.Tracer.TraceWarning<string>((long)this.GetHashCode(), "GroupMailboxAccessLayer::GetGroupMailbox. Unable to find group. LegacyDN={0}", group.LegacyDn);
						throw new MailboxNotFoundException(ServerStrings.ADUserNotFoundId(group.LegacyDn));
					}
					GroupMailboxAccessLayer.Tracer.TraceDebug<string, bool, string>((long)this.GetHashCode(), "GroupMailboxAccessLayer::GetGroupMailbox. Found ADUser for group. LegacyDN={0}, IsCached={1}, OriginatingServer={2}", group.LegacyDn, aduser.IsCached, aduser.OriginatingServer);
					groupMailboxBuilder.BuildFromDirectory(aduser);
				}
				mailbox = groupMailboxBuilder.Mailbox;
			}
			return mailbox;
		}

		private static void LogException(IExtensibleLogger logger, string context, Exception exception)
		{
			GroupMailboxAccessLayer.Tracer.TraceError<string, Exception>(0L, "GroupMailboxAccessLayer.LogException: Exception found while executing {0}. Exception: {1}.", context, exception);
			logger.LogEvent(new SchemaBasedLogEvent<MailboxAssociationLogSchema.Error>
			{
				{
					MailboxAssociationLogSchema.Error.Context,
					context
				},
				{
					MailboxAssociationLogSchema.Error.Exception,
					exception
				}
			});
		}

		private static void LogWarning(IExtensibleLogger logger, string context, string message)
		{
			GroupMailboxAccessLayer.Tracer.TraceWarning<string, string>(0L, "GroupMailboxAccessLayer.LogWarning: Context: {0}. Message: {1}.", context, message);
			logger.LogEvent(new SchemaBasedLogEvent<MailboxAssociationLogSchema.Warning>
			{
				{
					MailboxAssociationLogSchema.Warning.Context,
					context
				},
				{
					MailboxAssociationLogSchema.Warning.Message,
					message
				}
			});
		}

		private static void InternalExecute(string operationDescription, IRecipientSession adSession, IMailboxSession localStoreSession, Guid logCorrelationMailboxGuid, OrganizationId organizationId, string clientInfoString, Action<GroupMailboxAccessLayer> action)
		{
			GroupMailboxAccessLayer.<>c__DisplayClass6f CS$<>8__locals1 = new GroupMailboxAccessLayer.<>c__DisplayClass6f();
			CS$<>8__locals1.operationDescription = operationDescription;
			CS$<>8__locals1.adSession = adSession;
			CS$<>8__locals1.localStoreSession = localStoreSession;
			CS$<>8__locals1.clientInfoString = clientInfoString;
			CS$<>8__locals1.action = action;
			ArgumentValidator.ThrowIfNullOrWhiteSpace("operationDescription", CS$<>8__locals1.operationDescription);
			ArgumentValidator.ThrowIfNull("adSession", CS$<>8__locals1.adSession);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("clientInfoString", CS$<>8__locals1.clientInfoString);
			CS$<>8__locals1.logger = MailboxAssociationDiagnosticsFrameFactory.Default.CreateLogger(logCorrelationMailboxGuid, organizationId);
			CS$<>8__locals1.performanceTracker = MailboxAssociationDiagnosticsFrameFactory.Default.CreatePerformanceTracker(null);
			using (MailboxAssociationDiagnosticsFrameFactory.Default.CreateDiagnosticsFrame("GroupMailboxAccessLayer.InternalExecute", CS$<>8__locals1.operationDescription, CS$<>8__locals1.logger, CS$<>8__locals1.performanceTracker))
			{
				ILUtil.DoTryFilterCatch(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<InternalExecute>b__6b)), new FilterDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<InternalExecute>b__6c)), null);
			}
		}

		private void ReplicateOutOfSyncAssociation(Func<IAssociationStore, IAssociationAdaptor> associationAdaptorCreator, IMailboxLocator masterLocator, params IMailboxLocator[] slaveLocators)
		{
			ArgumentValidator.ThrowIfNull("masterLocator", masterLocator);
			ArgumentValidator.ThrowIfNull("slaveLocators", slaveLocators);
			ArgumentValidator.ThrowIfZeroOrNegative("slaveLocators.Length", slaveLocators.Length);
			GroupMailboxAccessLayer.Tracer.TraceDebug<IMailboxLocator, int>((long)this.GetHashCode(), "GroupMailboxAccessLayer::ReplicateOutOfSyncAssociation. Replicating data from mailbox: {0}. Number of out of sync mailboxes: {1}", masterLocator, slaveLocators.Length);
			using (IAssociationStore associationStore = this.storeProviderBuilder.Create(masterLocator, this.PerformanceTracker))
			{
				IAssociationAdaptor associationAdaptor = associationAdaptorCreator(associationStore);
				InProcessAssociationReplicator replicator = new InProcessAssociationReplicator(this.Logger, this.PerformanceTracker, OpenAsAdminOrSystemServiceBudgetTypeType.RunAsBackgroundLoad);
				ICollection<MailboxAssociation> associations = slaveLocators.Select(new Func<IMailboxLocator, MailboxAssociation>(associationAdaptor.GetAssociation));
				this.ReplicateAssociations(replicator, associationAdaptor, associations);
			}
		}

		private void ReplicateOutOfSyncAssociation(Func<IAssociationStore, IAssociationAdaptor> associationAdaptorCreator, IMailboxLocator masterLocator)
		{
			ArgumentValidator.ThrowIfNull("masterLocator", masterLocator);
			GroupMailboxAccessLayer.Tracer.TraceDebug<IMailboxLocator>((long)this.GetHashCode(), "GroupMailboxAccessLayer::ReplicateOutOfSyncAssociation. Replicating data from mailbox: {0}.", masterLocator);
			using (IAssociationStore storeProvider = this.storeProviderBuilder.Create(masterLocator, this.PerformanceTracker))
			{
				IAssociationAdaptor associationAdaptor = associationAdaptorCreator(storeProvider);
				InProcessAssociationReplicator replicator = new InProcessAssociationReplicator(this.Logger, this.PerformanceTracker, OpenAsAdminOrSystemServiceBudgetTypeType.RunAsBackgroundLoad);
				IEnumerable<MailboxAssociation> associations = from association in associationAdaptor.GetAllAssociations()
				where association.IsOutOfSync(storeProvider.MailboxLocator.IdentityHash)
				select association;
				if (this.ReplicateAssociations(replicator, associationAdaptor, associations))
				{
					GroupMailboxAccessLayer.Tracer.TraceDebug((long)this.GetHashCode(), "GroupMailboxAccessLayer::ReplicateOutOfSyncAssociation. All replication succeeded. Clearing mailbox flags");
					ExDateTime nextReplicationTime = ExDateTime.UtcNow.Add(GroupMailboxAccessLayer.TimeBetweenMailboxReplicationProcessing);
					storeProvider.SaveMailboxSyncStatus(nextReplicationTime);
				}
				else
				{
					GroupMailboxAccessLayer.Tracer.TraceError((long)this.GetHashCode(), "GroupMailboxAccessLayer::ReplicateOutOfSyncAssociation. Not all replication succeeded. Keeping out of sync flags.");
				}
			}
		}

		private bool ReplicateAssociations(InProcessAssociationReplicator replicator, IAssociationAdaptor associationAdaptor, IEnumerable<MailboxAssociation> associations)
		{
			ArgumentValidator.ThrowIfNull("replicator", replicator);
			ArgumentValidator.ThrowIfNull("associationAdaptor", associationAdaptor);
			ArgumentValidator.ThrowIfNull("associations", associations);
			bool flag = true;
			foreach (MailboxAssociation mailboxAssociation in associations)
			{
				GroupMailboxAccessLayer.Tracer.TraceDebug<MailboxAssociation>((long)this.GetHashCode(), "GroupMailboxAccessLayer::ReplicateAssociations. Replicating association {0}", mailboxAssociation);
				flag &= replicator.ReplicateAssociation(associationAdaptor, new MailboxAssociation[]
				{
					mailboxAssociation
				});
			}
			return flag;
		}

		private void LogCommandExecution(string commandDescription, GroupMailboxLocator group, params UserMailboxLocator[] users)
		{
			this.Logger.LogEvent(new SchemaBasedLogEvent<MailboxAssociationLogSchema.CommandExecution>
			{
				{
					MailboxAssociationLogSchema.CommandExecution.Command,
					commandDescription
				},
				{
					MailboxAssociationLogSchema.CommandExecution.GroupMailbox,
					group
				},
				{
					MailboxAssociationLogSchema.CommandExecution.UserMailboxes,
					users
				}
			});
		}

		private void LogPerformanceCounter(MailboxAssociationLogSchema.PerformanceCounterName counterName, TimeSpan latency, string context, params object[] contextArgs)
		{
			this.Logger.LogEvent(new SchemaBasedLogEvent<MailboxAssociationLogSchema.PerformanceCounter>
			{
				{
					MailboxAssociationLogSchema.PerformanceCounter.CounterName,
					counterName
				},
				{
					MailboxAssociationLogSchema.PerformanceCounter.Context,
					string.Format(context, contextArgs)
				},
				{
					MailboxAssociationLogSchema.PerformanceCounter.Latency,
					latency
				}
			});
		}

		private IEnumerable<UserMailbox> GetEscalatedMembersInternal(IAssociationStore storeProvider, GroupMailboxLocator group, bool loadAllDetails = false)
		{
			UserAssociationAdaptor adaptor = new UserAssociationAdaptor(storeProvider, this.adSession, group);
			GetEscalatedAssociations getEscalatedAssociations = new GetEscalatedAssociations(adaptor);
			IEnumerable<MailboxAssociation> associations = getEscalatedAssociations.Execute(null);
			return this.mailboxCollectionBuilder.BuildUserMailboxes(group, associations, loadAllDetails);
		}

		private MailboxAssociation CreateMailboxAssociationWithDefaultValues(UserMailboxLocator user, GroupMailboxLocator group)
		{
			MailboxAssociation mailboxAssociation = new MailboxAssociation
			{
				User = user,
				Group = group,
				UserSmtpAddress = SmtpAddress.Empty,
				IsMember = false,
				ShouldEscalate = false,
				IsPin = false,
				JoinedBy = string.Empty,
				JoinDate = default(ExDateTime),
				LastVisitedDate = default(ExDateTime)
			};
			GroupMailboxAccessLayer.Tracer.TraceDebug<MailboxAssociation>((long)this.GetHashCode(), "GroupMailboxAccessLayer.CreateMailboxAssociationWithDefaultValues: Creating new association with default values. Association={0}", mailboxAssociation);
			return mailboxAssociation;
		}

		private static readonly Trace Tracer = ExTraceGlobals.GroupMailboxAccessLayerTracer;

		private static readonly TimeSpan TimeBetweenMailboxReplicationProcessing = TimeSpan.FromDays(60.0);

		private readonly IRecipientSession adSession;

		private readonly IStoreBuilder storeProviderBuilder;

		private readonly MailboxCollectionBuilder mailboxCollectionBuilder;

		private readonly string clientString;
	}
}
