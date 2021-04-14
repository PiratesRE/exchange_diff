using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage.WorkingSet.SignalApiEx;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.WorkingSet;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.WorkingSet.SignalApi;

namespace Microsoft.Exchange.Data.Storage.WorkingSet.Publisher
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class WorkingSetPublisher
	{
		internal WorkingSetPublisher()
		{
			this.groupsCache = new Dictionary<Guid, WorkingSetPublisher.GroupMailboxData>();
		}

		internal Exception PublishGroupPost(CoreItem coreItem, string groupName, string groupId)
		{
			Exception ex = null;
			MailboxSession mailboxSession = coreItem.Session as MailboxSession;
			IExtensibleLogger logger = WorkingSetPublisherDiagnosticsFrameFactory.Default.CreateLogger(mailboxSession.MailboxGuid, mailboxSession.OrganizationId);
			IWorkingSetPublisherPerformanceTracker workingSetPublisherPerformanceTracker = WorkingSetPublisherDiagnosticsFrameFactory.Default.CreatePerformanceTracker(mailboxSession);
			using (WorkingSetPublisherDiagnosticsFrameFactory.Default.CreateDiagnosticsFrame("XSO", "PublishGroupPost", logger, workingSetPublisherPerformanceTracker))
			{
				try
				{
					this.EnsureGroupMailboxData(mailboxSession);
					if (this.groupsCache[mailboxSession.MailboxGuid].TargetUsers.Count == 0)
					{
						WorkingSetPublisher.Tracer.TraceDebug((long)this.GetHashCode(), "WorkingSetPublisher.PublishGroupPost: Skipping publishing as there are no users to publish to");
						return null;
					}
					coreItem.PropertyBag.Load(CoreObjectSchema.AllPropertiesOnStore);
					object obj = coreItem.PropertyBag.TryGetProperty(ItemSchema.From);
					Participant participant = obj as Participant;
					if (participant != null)
					{
						workingSetPublisherPerformanceTracker.OriginalMessageSender = (participant.EmailAddress ?? string.Empty);
						workingSetPublisherPerformanceTracker.OriginalMessageSenderRecipientType = ((participant.Origin != null) ? participant.Origin.ToString() : string.Empty);
						workingSetPublisherPerformanceTracker.OriginalMessageClass = coreItem.PropertyBag.GetValueOrDefault<string>(StoreObjectSchema.ItemClass, string.Empty);
						workingSetPublisherPerformanceTracker.OriginalMessageId = ((coreItem.Id != null) ? coreItem.Id.ToBase64String() : string.Empty);
						workingSetPublisherPerformanceTracker.OriginalInternetMessageId = coreItem.PropertyBag.GetValueOrDefault<string>(ItemSchema.InternetMessageId, string.Empty);
					}
					workingSetPublisherPerformanceTracker.ParticipantsInOriginalMessage = coreItem.Recipients.Count;
					Participant[] participantsOnOriginalMessage = null;
					using (MessageItem messageItem = this.CreateMessageItemForPublishing(coreItem, out participantsOnOriginalMessage, workingSetPublisherPerformanceTracker))
					{
						List<string> targetUsers = this.GetTargetUsers(messageItem, participantsOnOriginalMessage, workingSetPublisherPerformanceTracker);
						workingSetPublisherPerformanceTracker.HasWorkingSetUser = (targetUsers.Count > 0);
						if (targetUsers.Count == 0)
						{
							WorkingSetPublisher.Tracer.TraceDebug((long)this.GetHashCode(), "WorkingSetPublisher.PublishGroupPost: Skipping publishing as there are no additional users to publish to");
							return null;
						}
						Dictionary<string, object> signalProperties = new Dictionary<string, object>
						{
							{
								"WorkingSetSourcePartition",
								groupName
							},
							{
								"WorkingSetSourcePartitionInternal",
								groupId
							}
						};
						workingSetPublisherPerformanceTracker.PublishedMessageId = ((messageItem.Id != null) ? messageItem.Id.ToBase64String() : string.Empty);
						workingSetPublisherPerformanceTracker.PublishedIntnernetMessageId = (messageItem.InternetMessageId ?? string.Empty);
						ex = this.PublishItemAdd(messageItem, targetUsers, "ExchangePinnedGroup", signalProperties);
					}
				}
				catch (StoragePermanentException ex2)
				{
					ex = ex2;
					this.TraceAndLogError(logger, "PublishGroupPost", string.Format("WorkingSetPublisher.PublishGroupPost: Publishing failed failed with store permanent exception: {0}", ex2));
					workingSetPublisherPerformanceTracker.Exception = ex.ToString();
				}
				catch (StorageTransientException ex3)
				{
					ex = ex3;
					this.TraceAndLogError(logger, "PublishGroupPost", string.Format("WorkingSetPublisher.PublishGroupPost: Publishing failed failed with store transient exception: {0}", ex3));
					workingSetPublisherPerformanceTracker.Exception = ex.ToString();
				}
				catch (Exception ex4)
				{
					this.TraceAndLogError(logger, "PublishGroupPost", string.Format("WorkingSetPublisher.PublishGroupPost: Publishing failed failed with unexpected exception: {0}", ex4));
					workingSetPublisherPerformanceTracker.Exception = ex4.ToString();
					throw;
				}
			}
			return ex;
		}

		internal static bool IsGroupWSPublishingEnabled()
		{
			return WorkingSetPublisherConfiguration.PublishModernGroupsSignals;
		}

		private void TraceAndLogError(IExtensibleLogger logger, string context, string errorMessage)
		{
			WorkingSetPublisher.Tracer.TraceError((long)this.GetHashCode(), errorMessage);
			logger.LogEvent(new SchemaBasedLogEvent<WorkingSetPublisherLogSchema.Error>
			{
				{
					WorkingSetPublisherLogSchema.Error.Context,
					context
				},
				{
					WorkingSetPublisherLogSchema.Error.Exception,
					errorMessage
				}
			});
		}

		protected virtual Exception PublishItemAdd(Item item, List<string> workingSets, string sourceSystem, Dictionary<string, object> signalProperties)
		{
			SignalFeederEx signalFeederEx = WorkingSetPublisher.CreateGroupSignalFeeder(item.Session);
			Participant valueOrDefault = item.GetValueOrDefault<Participant>(InternalSchema.Sender, null);
			if (valueOrDefault == null || string.IsNullOrEmpty(valueOrDefault.EmailAddress))
			{
				WorkingSetPublisher.Tracer.TraceDebug<string>((long)this.GetHashCode(), "WorkingSetPublisher::PublishItemAdd. Item missing Sender property. Item={0}", item.InternalObjectId.ToString());
				return new CorruptDataException(new LocalizedString("Missing Sender property"));
			}
			Item item2 = new Item(valueOrDefault.EmailAddress, sourceSystem);
			string itemWorkingSetId = WorkingSetPublisher.GetItemWorkingSetId(item);
			Item item3 = new ExchangeItem(itemWorkingSetId, sourceSystem, false, item);
			if (signalProperties != null)
			{
				foreach (string text in signalProperties.Keys)
				{
					item3.SetProperty(text, signalProperties[text]);
				}
			}
			Action action = new Action("Authored", DateTime.UtcNow, WorkingSetPublisherConfiguration.ModernGroupsItemAddWeight, item3, sourceSystem);
			signalFeederEx.Put(item2, action, workingSets);
			return null;
		}

		private MessageItem CreateMessageItemForPublishing(CoreItem coreItem, out Participant[] participantsOnOriginalMessage, IWorkingSetPublisherPerformanceTracker performanceTracker)
		{
			MailboxSession mailboxSession = coreItem.Session as MailboxSession;
			StoreObjectId storeObjectId = mailboxSession.GetDefaultFolderId(DefaultFolderType.TemporarySaves);
			if (storeObjectId == null)
			{
				storeObjectId = mailboxSession.CreateDefaultFolder(DefaultFolderType.TemporarySaves);
			}
			MessageItem messageItem = MessageItem.Create(mailboxSession, storeObjectId);
			CoreItem.CopyItemContent(coreItem, messageItem.CoreItem);
			participantsOnOriginalMessage = new Participant[messageItem.Recipients.Count];
			int num = 0;
			foreach (Recipient recipient in messageItem.Recipients)
			{
				participantsOnOriginalMessage[num] = recipient.Participant;
				num++;
			}
			Participant participant = new Participant(mailboxSession.MailboxOwner);
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			if (!participant.ExistIn(participantsOnOriginalMessage))
			{
				messageItem.Recipients.Add(participant, RecipientItemType.To);
				performanceTracker.IsGroupParticipantAddedToParticipants = true;
			}
			stopwatch.Stop();
			performanceTracker.EnsureGroupParticipantAddedMilliseconds = stopwatch.ElapsedMilliseconds;
			stopwatch.Reset();
			messageItem.Save(SaveMode.NoConflictResolutionForceSave);
			messageItem.Load();
			return messageItem;
		}

		private static string GetItemWorkingSetId(StoreObject item)
		{
			byte[] longTermIdFromId = item.Session.IdConverter.GetLongTermIdFromId(item.StoreObjectId);
			return Convert.ToBase64String(longTermIdFromId);
		}

		private List<string> GetTargetUsers(MessageItem item, Participant[] participantsOnOriginalMessage, IWorkingSetPublisherPerformanceTracker performanceTracker)
		{
			List<string> list = new List<string>(this.groupsCache[item.Session.MailboxGuid].TargetUsers.Count);
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			if (this.groupsCache[item.Session.MailboxGuid].TargetUsers.Count > 0)
			{
				foreach (string text in this.groupsCache[item.Session.MailboxGuid].TargetUsers)
				{
					Participant participant = new Participant.Builder
					{
						EmailAddress = text,
						RoutingType = "EX"
					}.ToParticipant();
					if (participant.ExistIn(participantsOnOriginalMessage))
					{
						performanceTracker.IncrementParticipantsSkippedInPublishedMessage();
					}
					else
					{
						list.Add(text);
						performanceTracker.IncrementParticipantsAddedToPublishedMessage();
					}
				}
			}
			stopwatch.Stop();
			performanceTracker.DedupeParticipantsMilliseconds = stopwatch.ElapsedMilliseconds;
			return list;
		}

		protected virtual List<string> GetGroupPinners(StoreSession session, GroupMailboxLocator groupMailboxLocator)
		{
			PinnersGetter pinnersGetter = new PinnersGetter(groupMailboxLocator, session as MailboxSession);
			return pinnersGetter.Execute();
		}

		protected virtual List<string> GetGroupSubscribers(StoreSession session, GroupMailboxLocator groupMailboxLocator)
		{
			EscalationGetter escalationGetter = new EscalationGetter(groupMailboxLocator, session as MailboxSession);
			return escalationGetter.Execute();
		}

		protected virtual List<string> GetGroupOwners(ADUser groupAdUser)
		{
			List<string> list = new List<string>();
			ADObjectId[] ids = groupAdUser.Owners.ToArray();
			foreach (Result<ADUser> result in groupAdUser.Session.FindByADObjectIds<ADUser>(ids))
			{
				if (result.Data != null)
				{
					list.Add(result.Data.LegacyExchangeDN);
				}
			}
			return list;
		}

		protected virtual bool IsUserFlighted(IRecipientSession adSession, string userLegDn)
		{
			ADRecipient adrecipient = adSession.FindByLegacyExchangeDN(userLegDn);
			ADUser aduser = adrecipient as ADUser;
			if (adrecipient == null || aduser == null)
			{
				return false;
			}
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(aduser.GetContext(null), null, null);
			return snapshot.OwaClientServer.ModernGroupsWorkingSet.Enabled;
		}

		protected void EnsureGroupMailboxData(StoreSession session)
		{
			this.ClearExpiredCacheData();
			if (this.groupsCache.ContainsKey(session.MailboxGuid))
			{
				return;
			}
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, session.GetADSessionSettings(), 466, "EnsureGroupMailboxData", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\WorkingSet\\Publisher\\WorkingSetPublisher.cs");
			ADUser adUser;
			if (session.MailboxOwner.ObjectId != null)
			{
				adUser = tenantOrRootOrgRecipientSession.FindADUserByObjectId(session.MailboxOwner.ObjectId);
			}
			else
			{
				adUser = (tenantOrRootOrgRecipientSession.FindByLegacyExchangeDN(session.MailboxOwner.LegacyDn) as ADUser);
			}
			WorkingSetPublisher.GroupMailboxData groupMailboxData = new WorkingSetPublisher.GroupMailboxData
			{
				TargetUsers = this.CalculateTargetUsers(adUser, tenantOrRootOrgRecipientSession, session),
				TimeStamp = DateTime.UtcNow
			};
			if (groupMailboxData.TargetUsers.Count > WorkingSetPublisherConfiguration.MaxTargetUsersToCachePerModernGroup)
			{
				groupMailboxData.TimeStamp -= WorkingSetPublisherConfiguration.ModernGroupsDataExpiryTime;
			}
			this.groupsCache[session.MailboxGuid] = groupMailboxData;
		}

		private void ClearExpiredCacheData()
		{
			DateTime utcNow = DateTime.UtcNow;
			List<Guid> list = new List<Guid>();
			foreach (KeyValuePair<Guid, WorkingSetPublisher.GroupMailboxData> keyValuePair in this.groupsCache)
			{
				if (utcNow - keyValuePair.Value.TimeStamp > WorkingSetPublisherConfiguration.ModernGroupsDataExpiryTime)
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (Guid key in list)
			{
				WorkingSetPublisher.GroupMailboxData groupMailboxData = this.groupsCache[key];
				this.groupsCache.Remove(key);
			}
		}

		private List<string> CalculateTargetUsers(ADUser adUser, IRecipientSession adSession, StoreSession session)
		{
			GroupMailboxLocator groupMailboxLocator = GroupMailboxLocator.Instantiate(adSession, new SmtpProxyAddress(session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(), true));
			ADUser aduser = adUser ?? groupMailboxLocator.FindAdUser();
			if (aduser == null)
			{
				return new List<string>();
			}
			List<string> groupPinners = this.GetGroupPinners(session, groupMailboxLocator);
			List<string> groupOwners = this.GetGroupOwners(aduser);
			HashSet<string> hashSet = new HashSet<string>(groupPinners);
			foreach (string item in groupOwners)
			{
				hashSet.Add(item);
			}
			if (hashSet.Count != 0)
			{
				List<string> groupSubscribers = this.GetGroupSubscribers(session, groupMailboxLocator);
				foreach (string item2 in groupSubscribers)
				{
					hashSet.Remove(item2);
				}
				List<string> list = new List<string>();
				foreach (string text in hashSet)
				{
					if (!this.IsUserFlighted(adSession, text))
					{
						list.Add(text);
					}
				}
				foreach (string item3 in list)
				{
					hashSet.Remove(item3);
				}
			}
			return hashSet.ToList<string>();
		}

		private static SignalFeederEx CreateGroupSignalFeeder(StoreSession session)
		{
			StoreId defaultFolderId = session.GetDefaultFolderId(DefaultFolderType.SentItems);
			return new SignalFeederEx(session, defaultFolderId);
		}

		private const string OperationContext = "XSO";

		private const string OperationPublishGroupPost = "PublishGroupPost";

		private static readonly Microsoft.Exchange.Diagnostics.Trace Tracer = ExTraceGlobals.WorkingSetPublisherTracer;

		private Dictionary<Guid, WorkingSetPublisher.GroupMailboxData> groupsCache;

		private class GroupMailboxData
		{
			internal List<string> TargetUsers { get; set; }

			internal DateTime TimeStamp { get; set; }
		}
	}
}
