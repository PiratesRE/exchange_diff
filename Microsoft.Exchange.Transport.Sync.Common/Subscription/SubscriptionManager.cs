using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.DeltaSync;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Imap;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SubscriptionManager : ISubscriptionManager
	{
		public static void DeleteSubscription(MailboxSession mailboxSession, Guid subscriptionGuid)
		{
			StoreId messageId = null;
			int subscriptionCount = 0;
			SubscriptionManager.ForEachSubscriptionInMailbox(mailboxSession, delegate(object[] item)
			{
				subscriptionCount++;
				if (!(item[2] is PropertyError) && !(item[1] is PropertyError))
				{
					Guid b = (Guid)item[2];
					if (subscriptionGuid == b)
					{
						AggregationSubscriptionType subscriptionType;
						if (!(item[0] is string))
						{
							subscriptionType = AggregationSubscriptionType.Unknown;
						}
						else
						{
							subscriptionType = AggregationSubscription.GetSubscriptionKind((string)item[0]);
						}
						try
						{
							SubscriptionManager.DeleteSubscriptionSyncState(mailboxSession, subscriptionGuid, subscriptionType);
							ReportData reportData = SkippedItemUtilities.GetReportData(subscriptionGuid);
							reportData.Delete(mailboxSession.Mailbox.MapiStore);
						}
						catch (LocalizedException ex)
						{
							CommonLoggingHelper.SyncLogSession.LogError((TSLID)9989UL, ExTraceGlobals.SubscriptionManagerTracer, "Failed to delete report data from subscription with ID {0}, due to error: {1}", new object[]
							{
								subscriptionGuid,
								ex
							});
						}
						messageId = (StoreId)item[1];
						SubscriptionManager.instance.UpdateMailboxTableAndPerformSubscriptionOperation<StoreId>(mailboxSession, messageId, new Action<MailboxSession, StoreId>(SubscriptionManager.instance.messageHelper.DeleteSubscription));
						subscriptionCount--;
					}
				}
				return true;
			});
			if (messageId == null)
			{
				throw new ObjectNotFoundException(Strings.SubscriptionNotFound(subscriptionGuid.ToString()));
			}
			if (subscriptionCount == 0)
			{
				SubscriptionManager.instance.mailboxTableSubscriptionPropertyHelper.UpdateContentAggregationFlags(mailboxSession, ContentAggregationFlags.None);
			}
		}

		public static bool TryDeleteSubscription(MailboxSession mailboxSession, AggregationSubscription subscription)
		{
			SyncUtilities.ThrowIfArgumentNull("mailboxSession", mailboxSession);
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			Exception ex = null;
			try
			{
				SubscriptionManager.Instance.DeleteSubscription(mailboxSession, subscription, true);
			}
			catch (LocalizedException ex2)
			{
				ex = ex2;
			}
			if (ex != null)
			{
				CommonLoggingHelper.SyncLogSession.LogError((TSLID)106UL, ExTraceGlobals.SubscriptionManagerTracer, "Failed to delete subscription with ID {0}, due to error: {1}", new object[]
				{
					subscription.SubscriptionGuid,
					ex
				});
				return false;
			}
			return true;
		}

		public static bool TryDeleteSubscription(MailboxSession mailboxSession, Guid subscriptionGuid)
		{
			Exception ex = null;
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			try
			{
				SubscriptionManager.DeleteSubscription(mailboxSession, subscriptionGuid);
			}
			catch (LocalizedException ex2)
			{
				ex = ex2;
			}
			if (ex != null)
			{
				CommonLoggingHelper.SyncLogSession.LogError((TSLID)68UL, ExTraceGlobals.SubscriptionManagerTracer, "Failed to load delete subscription with ID {0}, due to error: {1}", new object[]
				{
					subscriptionGuid,
					ex
				});
				return false;
			}
			return true;
		}

		public static AggregationSubscription GetSubscription(MailboxSession mailboxSession, Guid subscriptionGuid)
		{
			return SubscriptionManager.GetSubscription(mailboxSession, subscriptionGuid, true);
		}

		public static AggregationSubscription GetSubscription(MailboxSession mailboxSession, Guid subscriptionGuid, bool upgradeIfRequired)
		{
			if (!SyncUtilities.IsDatacenterMode())
			{
				return null;
			}
			AggregationSubscriptionType subscriptionType;
			StoreId storeId = SubscriptionManager.FindSubscription(mailboxSession, subscriptionGuid, out subscriptionType);
			if (storeId == null)
			{
				throw new ObjectNotFoundException(Strings.SubscriptionNotFound(subscriptionGuid.ToString()));
			}
			return SubscriptionManager.LoadSubscription(mailboxSession, storeId, subscriptionType, upgradeIfRequired);
		}

		public static List<AggregationSubscription> GetAllSubscriptions(MailboxSession mailboxSession, AggregationSubscriptionType subscriptionTypeFilter)
		{
			return SubscriptionManager.GetAllSubscriptions(mailboxSession, subscriptionTypeFilter, true);
		}

		public static List<TransactionalRequestJob> GetAllSyncRequests(MailboxSession session)
		{
			Guid databaseGuid = session.MailboxOwner.MailboxInfo.GetDatabaseGuid();
			List<TransactionalRequestJob> result;
			using (RequestJobProvider requestJobProvider = new RequestJobProvider(databaseGuid))
			{
				requestJobProvider.AttachToMDB(databaseGuid);
				RequestIndexEntryQueryFilter filter = new RequestIndexEntryQueryFilter();
				IConfigurable[] array = requestJobProvider.Find<TransactionalRequestJob>(filter, null, true, null);
				List<TransactionalRequestJob> list = new List<TransactionalRequestJob>(array.Length);
				foreach (IConfigurable configurable in array)
				{
					list.Add(configurable as TransactionalRequestJob);
				}
				result = list;
			}
			return result;
		}

		public static List<AggregationSubscription> GetAllSubscriptions(MailboxSession mailboxSession, AggregationSubscriptionType subscriptionTypeFilter, bool upgradeIfRequired)
		{
			if (!SyncUtilities.IsDatacenterMode())
			{
				return new List<AggregationSubscription>();
			}
			List<AggregationSubscription> allSubscriptions = new List<AggregationSubscription>();
			SubscriptionManager.ForEachSubscriptionInMailbox(mailboxSession, delegate(object[] item)
			{
				if (!(item[0] is PropertyError) && !(item[1] is PropertyError))
				{
					string messageClass = (string)item[0];
					if ((AggregationSubscription.GetSubscriptionKind(messageClass) & subscriptionTypeFilter) != AggregationSubscriptionType.Unknown)
					{
						StoreId storeId = (StoreId)item[1];
						AggregationSubscriptionType subscriptionKind = AggregationSubscription.GetSubscriptionKind((string)item[0]);
						AggregationSubscription aggregationSubscription = null;
						Exception ex = null;
						try
						{
							aggregationSubscription = SubscriptionManager.LoadSubscription(mailboxSession, storeId, subscriptionKind, upgradeIfRequired);
						}
						catch (PropertyErrorException ex2)
						{
							ex = ex2;
						}
						catch (CorruptDataException ex3)
						{
							ex = ex3;
						}
						catch (InvalidDataException ex4)
						{
							ex = ex4;
						}
						if (aggregationSubscription != null)
						{
							aggregationSubscription.InstanceKey = (item[4] as byte[]);
							allSubscriptions.Add(aggregationSubscription);
						}
						else if (ex != null)
						{
							CommonLoggingHelper.SyncLogSession.LogError((TSLID)69UL, ExTraceGlobals.SubscriptionManagerTracer, "Failed to load subscription with messageId: {0}, due to error: {1}", new object[]
							{
								storeId.ToBase64String(),
								ex
							});
						}
					}
				}
				return true;
			});
			return allSubscriptions;
		}

		public static SendAsSubscriptionsAndPeopleConnectResult GetAllSendAsSubscriptionsAndPeopleConnect(MailboxSession mailboxSession)
		{
			List<AggregationSubscription> allSubscriptions = SubscriptionManager.GetAllSubscriptions(mailboxSession, AggregationSubscriptionType.AllThatSupportSendAsAndPeopleConnect, true);
			List<PimAggregationSubscription> allSendAsSubscriptions = SubscriptionManager.GetAllSendAsSubscriptions(allSubscriptions);
			bool peopleConnectionsExistInformation = SubscriptionManager.GetPeopleConnectionsExistInformation(allSubscriptions);
			return new SendAsSubscriptionsAndPeopleConnectResult(allSendAsSubscriptions, peopleConnectionsExistInformation);
		}

		public static List<PimAggregationSubscription> GetAllSendAsSubscriptions(MailboxSession mailboxSession)
		{
			return SubscriptionManager.GetAllSendAsSubscriptions(mailboxSession, true);
		}

		public static List<PimAggregationSubscription> GetAllSendAsSubscriptions(MailboxSession mailboxSession, bool upgradeIfRequired)
		{
			List<AggregationSubscription> allSubscriptions = SubscriptionManager.GetAllSubscriptions(mailboxSession, AggregationSubscriptionType.AllEMail, upgradeIfRequired);
			return SubscriptionManager.GetAllSendAsSubscriptions(allSubscriptions);
		}

		private static List<PimAggregationSubscription> GetAllSendAsSubscriptions(List<AggregationSubscription> subscriptions)
		{
			List<PimAggregationSubscription> list = new List<PimAggregationSubscription>();
			foreach (AggregationSubscription aggregationSubscription in subscriptions)
			{
				PimAggregationSubscription pimAggregationSubscription = aggregationSubscription as PimAggregationSubscription;
				if (pimAggregationSubscription == null)
				{
					CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)70UL, ExTraceGlobals.SubscriptionManagerTracer, "Subscription is not a PimSubscription, so it's not valid for sendAs: {0}.", new object[]
					{
						aggregationSubscription.Name
					});
				}
				else if (SubscriptionManager.IsValidForSendAs(pimAggregationSubscription.SendAsState, pimAggregationSubscription.Status))
				{
					list.Add(pimAggregationSubscription);
				}
				else
				{
					CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)71UL, ExTraceGlobals.SubscriptionManagerTracer, "Subscription not valid for sendAs: {0}.", new object[]
					{
						pimAggregationSubscription.Name
					});
				}
			}
			return list;
		}

		private static bool GetPeopleConnectionsExistInformation(List<AggregationSubscription> subscriptions)
		{
			foreach (AggregationSubscription aggregationSubscription in subscriptions)
			{
				if (aggregationSubscription.AggregationType == AggregationType.PeopleConnection)
				{
					return true;
				}
			}
			return false;
		}

		public static bool DoesUserHasAnyActiveConnectedAccounts(MailboxSession mailboxSession, AggregationSubscriptionType subscriptionFilter)
		{
			SyncUtilities.ThrowIfArgumentNull("mailboxSession", mailboxSession);
			return SubscriptionManager.FetchSubscriptionsAndCheckForAnyActiveAggregationSubscriptions(() => SubscriptionManager.GetAllSubscriptions(mailboxSession, subscriptionFilter));
		}

		public static bool IsValidForSendAs(SendAsState state, AggregationStatus status)
		{
			return SendAsState.Enabled == state && AggregationStatus.Disabled != status && AggregationStatus.Poisonous != status;
		}

		public static void CreateSubscription(MailboxSession mailboxSession, AggregationSubscription subscription)
		{
			SubscriptionMailboxSession subMailboxSession = SubscriptionManager.instance.CreateSubscriptionMailboxSession(mailboxSession);
			SubscriptionManager.instance.CreateAndNotifyNewSubscription(subMailboxSession, subscription);
		}

		public static Exception UpdateSubscription(MailboxSession mailboxSession, AggregationSubscription subscription)
		{
			Exception result = null;
			try
			{
				SubscriptionManager.Instance.UpdateSubscriptionToMailbox(mailboxSession, subscription);
			}
			catch (Exception ex)
			{
				result = ex;
			}
			return result;
		}

		public static Exception TryUpdateSubscriptionAndSyncNow(MailboxSession mailboxSession, AggregationSubscription subscription, out bool syncNowRequestSentSuccessfully)
		{
			SubscriptionMailboxSession subMailboxSession = SubscriptionManager.instance.CreateSubscriptionMailboxSession(mailboxSession);
			Exception result;
			SubscriptionManager.instance.TrySaveAndNotifySubscriptionWithSyncNowRequest(subMailboxSession, subscription, out syncNowRequestSentSuccessfully, out result);
			return result;
		}

		public static bool TrySubscriptionSyncNow(MailboxSession mailboxSession, AggregationSubscription subscription)
		{
			SubscriptionMailboxSession subMailboxSession = SubscriptionManager.instance.CreateSubscriptionMailboxSession(mailboxSession);
			return SubscriptionManager.instance.TryRequestSyncNowForSubscription(subMailboxSession, subscription);
		}

		public static void SetSubscription(MailboxSession mailboxSession, AggregationSubscription subscription)
		{
			SubscriptionMailboxSession subMailboxSession = SubscriptionManager.instance.CreateSubscriptionMailboxSession(mailboxSession);
			SubscriptionManager.instance.SaveAndNotifySubscription(subMailboxSession, subscription);
		}

		public static bool SetSubscriptionAndSyncNow(MailboxSession mailboxSession, AggregationSubscription subscription)
		{
			SubscriptionMailboxSession subMailboxSession = SubscriptionManager.instance.CreateSubscriptionMailboxSession(mailboxSession);
			bool result;
			SubscriptionManager.instance.SaveAndNotifySubscriptionWithSyncNowRequest(subMailboxSession, subscription, out result);
			return result;
		}

		public static MailboxSession OpenMailbox(IExchangePrincipal owner, ExchangeMailboxOpenType openAs, string clientInfoString)
		{
			SyncUtilities.ThrowIfArgumentNull("owner", owner);
			SyncUtilities.ThrowIfArgumentNull("clientInfoString", clientInfoString);
			MailboxSession result;
			switch (openAs)
			{
			case ExchangeMailboxOpenType.AsAdministrator:
				result = MailboxSession.OpenAsAdmin(owner, CultureInfo.InvariantCulture, clientInfoString);
				break;
			case ExchangeMailboxOpenType.AsTransport:
				result = MailboxSession.OpenAsTransport(owner, clientInfoString);
				break;
			case ExchangeMailboxOpenType.AsUser:
				result = MailboxSession.Open(owner, new WindowsPrincipal(WindowsIdentity.GetCurrent()), CultureInfo.InvariantCulture, clientInfoString);
				break;
			default:
				throw new ArgumentException("invalid ExchangeMailboxOpenType value " + openAs);
			}
			return result;
		}

		public static string GenerateDeviceId(Guid subscriptionGuid)
		{
			return subscriptionGuid.ToString().Replace("-", "_");
		}

		public static StoreId FindSubscription(MailboxSession mailboxSession, Guid subscriptionGuid)
		{
			AggregationSubscriptionType aggregationSubscriptionType;
			return SubscriptionManager.FindSubscription(mailboxSession, subscriptionGuid, out aggregationSubscriptionType);
		}

		public static AggregationSubscription LoadSubscription(MailboxSession mailboxSession, StoreId messageId, AggregationSubscriptionType subscriptionType)
		{
			return SubscriptionManager.LoadSubscription(mailboxSession, messageId, subscriptionType, true);
		}

		public static AggregationSubscription LoadSubscription(MailboxSession mailboxSession, StoreId messageId, AggregationSubscriptionType subscriptionType, bool upgradeSubscriptionIfRequired)
		{
			if (!SyncUtilities.IsDatacenterMode())
			{
				return null;
			}
			AggregationSubscription aggregationSubscription = null;
			CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)72UL, "LoadSubscription: Will load subscriptions with type mask {0} from messageId: {1}.", new object[]
			{
				(int)subscriptionType,
				messageId
			});
			PropertyDefinition[] propertyDefinitions = SubscriptionManager.GetPropertyDefinitions(subscriptionType);
			using (MessageItem messageItem = MessageItem.Bind(mailboxSession, messageId, propertyDefinitions))
			{
				string className = messageItem.ClassName;
				if ((className.Equals("IPM.Aggregation.Pop", StringComparison.OrdinalIgnoreCase) && (subscriptionType & AggregationSubscriptionType.Pop) != AggregationSubscriptionType.Unknown) || (className.Equals("IPM.Aggregation.DeltaSync", StringComparison.OrdinalIgnoreCase) && (subscriptionType & AggregationSubscriptionType.DeltaSyncMail) != AggregationSubscriptionType.Unknown) || (className.Equals("IPM.Aggregation.IMAP", StringComparison.OrdinalIgnoreCase) && (subscriptionType & AggregationSubscriptionType.IMAP) != AggregationSubscriptionType.Unknown) || (className.Equals("IPM.Aggregation.Facebook", StringComparison.OrdinalIgnoreCase) && (subscriptionType & AggregationSubscriptionType.Facebook) != AggregationSubscriptionType.Unknown) || (className.Equals("IPM.Aggregation.LinkedIn", StringComparison.OrdinalIgnoreCase) && (subscriptionType & AggregationSubscriptionType.LinkedIn) != AggregationSubscriptionType.Unknown))
				{
					CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)36UL, "SubscriptionManager.LoadSubscription: Loading subscription from messageId: {0}.", new object[]
					{
						messageItem.Id.ObjectId
					});
					aggregationSubscription = SubscriptionManager.CreateAggregationSubscription(messageItem);
					aggregationSubscription.SubscriptionMessageId = messageItem.Id.ObjectId;
					aggregationSubscription.LoadSubscription(messageItem, (!mailboxSession.MailboxOwner.ObjectId.IsNullOrEmpty()) ? mailboxSession.MailboxOwner.ObjectId : null, mailboxSession.MailboxOwnerLegacyDN);
					aggregationSubscription.UserExchangeMailboxDisplayName = mailboxSession.MailboxOwner.MailboxInfo.DisplayName;
					aggregationSubscription.UserExchangeMailboxSmtpAddress = mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
					if (upgradeSubscriptionIfRequired)
					{
						bool flag = SubscriptionManager.instance.upgrader.UpgradeSubscription(aggregationSubscription, messageItem, SubscriptionManager.instance.messageHelper);
						if (flag)
						{
							messageItem.Load(propertyDefinitions);
							aggregationSubscription.SubscriptionMessageId = messageItem.Id.ObjectId;
						}
					}
					CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)73UL, "LoadSubscription: Loaded subscription: {0}.", new object[]
					{
						aggregationSubscription
					});
				}
				else
				{
					CommonLoggingHelper.SyncLogSession.LogError((TSLID)74UL, ExTraceGlobals.SubscriptionManagerTracer, "LoadSubscription: Not loading subscription for message id: '{0}' since messageClass: '{1}' is not recognized", new object[]
					{
						messageId,
						className
					});
				}
			}
			return aggregationSubscription;
		}

		public static PropertyDefinition[] GetPropertyDefinitions(AggregationSubscriptionType subscriptionType)
		{
			if (subscriptionType <= AggregationSubscriptionType.IMAP)
			{
				switch (subscriptionType)
				{
				case AggregationSubscriptionType.Pop:
					return SubscriptionManager.PopSearchColumnsIndex;
				case (AggregationSubscriptionType)3:
					break;
				case AggregationSubscriptionType.DeltaSyncMail:
					return SubscriptionManager.DeltaSyncSearchColumnsIndex;
				default:
					if (subscriptionType == AggregationSubscriptionType.IMAP)
					{
						return SubscriptionManager.IMAPSearchColumnsIndex;
					}
					break;
				}
			}
			else if (subscriptionType == AggregationSubscriptionType.Facebook || subscriptionType == AggregationSubscriptionType.LinkedIn)
			{
				return SubscriptionManager.ConnectSearchColumnsIndex;
			}
			throw new InvalidDataException("invalid subscription type " + subscriptionType);
		}

		internal static bool FetchSubscriptionsAndCheckForAnyActiveAggregationSubscriptions(Func<List<AggregationSubscription>> fetchSubscritpionsMethod)
		{
			SyncUtilities.ThrowIfArgumentNull("fetchSubscritpionsMethod", fetchSubscritpionsMethod);
			List<AggregationSubscription> list = fetchSubscritpionsMethod();
			foreach (AggregationSubscription aggregationSubscription in list)
			{
				if (aggregationSubscription.IsAggregation && !aggregationSubscription.Inactive)
				{
					return true;
				}
			}
			return false;
		}

		private static StoreId FindSubscription(MailboxSession mailboxSession, Guid subscriptionGuid, out AggregationSubscriptionType subscriptionType)
		{
			StoreId messageId = null;
			AggregationSubscriptionType subType = AggregationSubscriptionType.Unknown;
			SubscriptionManager.ForEachSubscriptionInMailbox(mailboxSession, delegate(object[] item)
			{
				if (SubscriptionManager.IsValidSubscriptionMessage(item) && subscriptionGuid == (Guid)item[2])
				{
					messageId = (StoreId)item[1];
					subType = AggregationSubscription.GetSubscriptionKind((string)item[0]);
				}
				return messageId == null;
			});
			subscriptionType = subType;
			return messageId;
		}

		private static bool IsValidSubscriptionMessage(object[] item)
		{
			return !(item[2] is PropertyError) && !(item[0] is PropertyError) && !(item[1] is PropertyError);
		}

		private static AggregationSubscription CreateAggregationSubscription(MessageItem message)
		{
			AggregationSubscriptionType subscriptionKind = AggregationSubscription.GetSubscriptionKind(message);
			AggregationSubscriptionType aggregationSubscriptionType = subscriptionKind;
			if (aggregationSubscriptionType <= AggregationSubscriptionType.IMAP)
			{
				switch (aggregationSubscriptionType)
				{
				case AggregationSubscriptionType.Pop:
					return new PopAggregationSubscription();
				case (AggregationSubscriptionType)3:
					break;
				case AggregationSubscriptionType.DeltaSyncMail:
					return new DeltaSyncAggregationSubscription();
				default:
					if (aggregationSubscriptionType == AggregationSubscriptionType.IMAP)
					{
						return new IMAPAggregationSubscription();
					}
					break;
				}
			}
			else if (aggregationSubscriptionType == AggregationSubscriptionType.Facebook || aggregationSubscriptionType == AggregationSubscriptionType.LinkedIn)
			{
				return new ConnectSubscription();
			}
			throw new InvalidDataException("invalid subscription type " + subscriptionKind);
		}

		private static void ForEachSubscriptionInMailbox(MailboxSession mailboxSession, SubscriptionManager.SubscriptionProcessor processor)
		{
			SubscriptionManager.ForEachSubscriptionInMailbox(mailboxSession, processor, mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox));
		}

		private static void ForEachSubscriptionInMailbox(MailboxSession mailboxSession, SubscriptionManager.SubscriptionProcessor processor, StoreObjectId folderId)
		{
			using (Folder folder = Folder.Bind(mailboxSession, folderId))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.Associated, null, SubscriptionManager.SortByItemClassInAscendingOrder, SubscriptionManager.SharingColumnsIndex))
				{
					if (queryResult.SeekToCondition(SeekReference.OriginBeginning, SubscriptionManager.AggregationMessageClassPrefixFilter))
					{
						bool flag = true;
						object[][] rows;
						do
						{
							rows = queryResult.GetRows(100);
							for (int i = 0; i < rows.Length; i++)
							{
								if (!SubscriptionManager.IsAggregationMessageClass(rows[i], 0))
								{
									goto Block_6;
								}
								flag = processor(rows[i]);
								if (!flag)
								{
									break;
								}
							}
						}
						while (flag && rows.Length > 0);
						Block_6:;
					}
				}
			}
		}

		private static bool IsAggregationMessageClass(object[] item, int messageClassColumnIndex)
		{
			string text = (string)item[messageClassColumnIndex];
			return !string.IsNullOrEmpty(text) && text.StartsWith("IPM.Aggregation.", StringComparison.InvariantCultureIgnoreCase);
		}

		private static void DeleteSubscriptionSyncState(MailboxSession mailboxSession, Guid subscriptionGuid, AggregationSubscriptionType subscriptionType)
		{
			if (subscriptionGuid != Guid.Empty && subscriptionType != AggregationSubscriptionType.Unknown)
			{
				SyncStateStorage.DeleteSyncStateStorage(mailboxSession, new DeviceIdentity(SubscriptionManager.GenerateDeviceId(subscriptionGuid), subscriptionType.ToString(), SubscriptionManager.Protocol), null);
			}
		}

		protected SubscriptionManager(SyncLogSession syncLogSession)
		{
			this.syncLogSession = syncLogSession;
			this.notificationClient = this.CreateNotificationClient();
			this.mailboxTableSubscriptionPropertyHelper = this.CreateMailboxTableSubscriptionPropertyHelper();
			this.messageHelper = this.CreateSubscriptionMessageHelper();
			this.upgrader = this.CreateSubscriptionUpgrader();
		}

		private SubscriptionManager() : this(CommonLoggingHelper.SyncLogSession)
		{
		}

		public static SubscriptionManager Instance
		{
			get
			{
				return SubscriptionManager.instance;
			}
		}

		public void CreateAndNotifyNewSubscription(SubscriptionMailboxSession subMailboxSession, ISyncWorkerData subscription)
		{
			SyncUtilities.ThrowIfArgumentNull("subMailboxSession", subMailboxSession);
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			subMailboxSession.SetPropertiesOfSubscription(subscription);
			this.UpdateMailboxTableAndSaveSubscription(subMailboxSession.MailboxSession, subscription);
			this.mailboxTableSubscriptionPropertyHelper.UpdateContentAggregationFlags(subMailboxSession.MailboxSession, ContentAggregationFlags.HasSubscriptions);
			string mailboxServerName = subMailboxSession.GetMailboxServerName();
			this.notificationClient.NotifySubscriptionAdded(subscription, mailboxServerName);
		}

		public void SaveSubscription(SubscriptionMailboxSession subMailboxSession, ISyncWorkerData subscription)
		{
			SyncUtilities.ThrowIfArgumentNull("subMailboxSession", subMailboxSession);
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			this.messageHelper.SaveSubscription(subMailboxSession.MailboxSession, subscription);
		}

		public void SaveAndNotifySubscription(SubscriptionMailboxSession subMailboxSession, ISyncWorkerData subscription)
		{
			SyncUtilities.ThrowIfArgumentNull("subMailboxSession", subMailboxSession);
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			this.UpdateMailboxTableAndSaveSubscription(subMailboxSession.MailboxSession, subscription);
			string mailboxServerName = subMailboxSession.GetMailboxServerName();
			this.notificationClient.NotifySubscriptionUpdated(subscription, mailboxServerName);
		}

		public void SaveAndNotifySubscriptionWithSyncNowRequest(SubscriptionMailboxSession subMailboxSession, ISyncWorkerData subscription, out bool syncNowSentSuccessfully)
		{
			SyncUtilities.ThrowIfArgumentNull("subMailboxSession", subMailboxSession);
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			subscription.LastSyncNowRequestTime = new DateTime?(DateTime.UtcNow);
			this.UpdateMailboxTableAndSaveSubscription(subMailboxSession.MailboxSession, subscription);
			string mailboxServerName = subMailboxSession.GetMailboxServerName();
			syncNowSentSuccessfully = this.notificationClient.NotifySubscriptionUpdatedAndSyncNowNeeded(subscription, mailboxServerName);
		}

		public bool TryRequestSyncNowForSubscription(SubscriptionMailboxSession subMailboxSession, AggregationSubscription subscription)
		{
			SyncUtilities.ThrowIfArgumentNull("subMailboxSession", subMailboxSession);
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			string mailboxServerName = subMailboxSession.GetMailboxServerName();
			return this.notificationClient.NotifySubscriptionSyncNowNeeded(subscription, mailboxServerName);
		}

		public bool TryCreateAndNotifyNewSubscription(SubscriptionMailboxSession subMailboxSession, ISyncWorkerData subscription, out Exception exception)
		{
			return this.TrySubscriptionOperation(subMailboxSession, subscription, new Action<SubscriptionMailboxSession, ISyncWorkerData>(this.CreateAndNotifyNewSubscription), out exception);
		}

		public bool TrySaveSubscription(SubscriptionMailboxSession subMailboxSession, ISyncWorkerData subscription, out Exception exception)
		{
			return this.TrySubscriptionOperation(subMailboxSession, subscription, new Action<SubscriptionMailboxSession, ISyncWorkerData>(this.SaveSubscription), out exception);
		}

		public bool TrySaveAndNotifySubscription(SubscriptionMailboxSession subMailboxSession, ISyncWorkerData subscription, out Exception exception)
		{
			return this.TrySubscriptionOperation(subMailboxSession, subscription, new Action<SubscriptionMailboxSession, ISyncWorkerData>(this.SaveAndNotifySubscription), out exception);
		}

		public bool TrySaveAndNotifySubscriptionWithSyncNowRequest(SubscriptionMailboxSession subMailboxSession, ISyncWorkerData subscription, out bool syncNowSentSuccessfully, out Exception exception)
		{
			bool syncNowSent = false;
			bool result = this.TrySubscriptionOperation(subMailboxSession, subscription, delegate(SubscriptionMailboxSession subMailboxSessionArg, ISyncWorkerData subscriptionArg)
			{
				this.SaveAndNotifySubscriptionWithSyncNowRequest(subMailboxSessionArg, subscriptionArg, out syncNowSent);
			}, out exception);
			syncNowSentSuccessfully = syncNowSent;
			return result;
		}

		public void UpdateSubscriptionToMailbox(MailboxSession mailboxSession, ISyncWorkerData subscription)
		{
			SubscriptionMailboxSession subMailboxSession = this.CreateSubscriptionMailboxSession(mailboxSession);
			Exception ex;
			this.TrySaveAndNotifySubscription(subMailboxSession, subscription, out ex);
			if (ex != null)
			{
				throw ex;
			}
		}

		public void DeleteSubscription(MailboxSession mailboxSession, ISyncWorkerData subscription, bool sendRpcNotification = true)
		{
			SyncUtilities.ThrowIfArgumentNull("mailboxSession", mailboxSession);
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			SubscriptionManager.DeleteSubscription(mailboxSession, subscription.SubscriptionGuid);
			if (sendRpcNotification)
			{
				SubscriptionMailboxSession subscriptionMailboxSession = SubscriptionManager.instance.CreateSubscriptionMailboxSession(mailboxSession);
				string mailboxServerName = subscriptionMailboxSession.GetMailboxServerName();
				SubscriptionManager.instance.notificationClient.NotifySubscriptionDeleted(subscription, mailboxServerName);
			}
		}

		protected virtual SubscriptionNotificationClient CreateNotificationClient()
		{
			return new SubscriptionNotificationClient();
		}

		protected virtual MailboxTableSubscriptionPropertyHelper CreateMailboxTableSubscriptionPropertyHelper()
		{
			return new MailboxTableSubscriptionPropertyHelper();
		}

		protected virtual SubscriptionMessageHelper CreateSubscriptionMessageHelper()
		{
			return new SubscriptionMessageHelper();
		}

		protected virtual SubscriptionMailboxSession CreateSubscriptionMailboxSession(MailboxSession mailboxSession)
		{
			return new SubscriptionMailboxSession(mailboxSession);
		}

		protected virtual SubscriptionUpgrader CreateSubscriptionUpgrader()
		{
			return new SubscriptionUpgrader(this.syncLogSession);
		}

		protected bool TrySubscriptionOperation(Action subscriptionOperation, out Exception exception)
		{
			SyncUtilities.ThrowIfArgumentNull("subscriptionOperation", subscriptionOperation);
			exception = null;
			try
			{
				subscriptionOperation();
				return true;
			}
			catch (TransientException innerException)
			{
				exception = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.ConnectionError, new SubscriptionUpdateTransientException(innerException));
			}
			catch (LocalizedException innerException2)
			{
				exception = SyncPermanentException.CreateOperationLevelException(DetailedAggregationStatus.ConnectionError, new SubscriptionUpdatePermanentException(innerException2));
			}
			this.syncLogSession.LogError((TSLID)1333UL, "Encountered error: {0}", new object[]
			{
				exception
			});
			return false;
		}

		private void UpdateMailboxTableAndSaveSubscription(MailboxSession mailboxSession, ISyncWorkerData subscription)
		{
			this.UpdateMailboxTableAndPerformSubscriptionOperation<ISyncWorkerData>(mailboxSession, subscription, new Action<MailboxSession, ISyncWorkerData>(this.messageHelper.SaveSubscription));
		}

		private void UpdateMailboxTableAndPerformSubscriptionOperation<T>(MailboxSession mailboxSession, T operationArgument, Action<MailboxSession, T> subscriptionOperation)
		{
			this.mailboxTableSubscriptionPropertyHelper.UpdateSubscriptionListTimestamp(mailboxSession);
			subscriptionOperation(mailboxSession, operationArgument);
			this.mailboxTableSubscriptionPropertyHelper.TryUpdateSubscriptionListTimestamp(mailboxSession);
		}

		private bool TrySubscriptionOperation(SubscriptionMailboxSession subMailboxSession, ISyncWorkerData subscription, Action<SubscriptionMailboxSession, ISyncWorkerData> subscriptionOperation, out Exception exception)
		{
			SyncUtilities.ThrowIfArgumentNull("subMailboxSession", subMailboxSession);
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			SyncUtilities.ThrowIfArgumentNull("subscriptionOperation", subscriptionOperation);
			return this.TrySubscriptionOperation(delegate()
			{
				subscriptionOperation(subMailboxSession, subscription);
			}, out exception);
		}

		private const string AggregationMessageClassPrefix = "IPM.Aggregation.";

		public static readonly string Protocol = "ContentAggregation";

		private static readonly PropertyDefinition[] SharingColumnsIndex = new PropertyDefinition[]
		{
			StoreObjectSchema.ItemClass,
			ItemSchema.Id,
			MessageItemSchema.SharingInstanceGuid,
			AggregationSubscriptionMessageSchema.SharingSubscriptionName,
			ItemSchema.InstanceKey
		};

		private static readonly PropertyDefinition[] PopSearchColumnsIndex = new PropertyDefinition[]
		{
			StoreObjectSchema.ItemClass,
			MessageItemSchema.SharingDetail,
			MessageItemSchema.SharingInstanceGuid,
			MessageItemSchema.SharingProviderGuid,
			MessageItemSchema.SharingLocalUid,
			MessageItemSchema.SharingLastSync,
			MessageItemSchema.SharingDetailedStatus,
			MessageItemSchema.SharingDiagnostics,
			AggregationSubscriptionMessageSchema.SharingPoisonCallstack,
			MessageItemSchema.SharingRemotePath,
			AggregationSubscriptionMessageSchema.SharingInitiatorName,
			AggregationSubscriptionMessageSchema.SharingInitiatorSmtp,
			AggregationSubscriptionMessageSchema.SharingRemoteUser,
			AggregationSubscriptionMessageSchema.SharingRemotePass,
			AggregationSubscriptionMessageSchema.SharingLastSuccessSyncTime,
			AggregationSubscriptionMessageSchema.SharingSyncRange,
			AggregationSubscriptionMessageSchema.SharingAggregationStatus,
			AggregationSubscriptionMessageSchema.SharingMigrationState,
			AggregationSubscriptionMessageSchema.SharingAggregationType,
			AggregationSubscriptionMessageSchema.SharingSubscriptionConfiguration,
			AggregationSubscriptionMessageSchema.SharingAggregationProtocolVersion,
			AggregationSubscriptionMessageSchema.SharingAggregationProtocolName,
			AggregationSubscriptionMessageSchema.SharingSubscriptionName,
			MessageItemSchema.SharingSubscriptionVersion,
			MessageItemSchema.SharingSendAsState,
			MessageItemSchema.SharingSendAsValidatedEmail,
			AggregationSubscriptionMessageSchema.SharingSubscriptionCreationType,
			AggregationSubscriptionMessageSchema.SharingSendAsVerificationEmailState,
			AggregationSubscriptionMessageSchema.SharingSendAsVerificationMessageId,
			AggregationSubscriptionMessageSchema.SharingSendAsVerificationTimestamp,
			AggregationSubscriptionMessageSchema.SharingSubscriptionEvents,
			AggregationSubscriptionMessageSchema.SharingSubscriptionExclusionFolders,
			AggregationSubscriptionMessageSchema.SharingSubscriptionItemsSynced,
			AggregationSubscriptionMessageSchema.SharingSubscriptionItemsSkipped,
			AggregationSubscriptionMessageSchema.SharingSubscriptionTotalItemsInSourceMailbox,
			AggregationSubscriptionMessageSchema.SharingSubscriptionTotalSizeOfSourceMailbox,
			AggregationSubscriptionMessageSchema.SharingAdjustedLastSuccessfulSyncTime,
			AggregationSubscriptionMessageSchema.SharingOutageDetectionDiagnostics,
			AggregationSubscriptionMessageSchema.SharingSubscriptionSyncPhase,
			AggregationSubscriptionMessageSchema.SharingLastSyncNowRequest,
			AggregationSubscriptionMessageSchema.SharingInitialSyncInRecoveryMode
		};

		private static readonly PropertyDefinition[] DeltaSyncSearchColumnsIndex = new PropertyDefinition[]
		{
			StoreObjectSchema.ItemClass,
			MessageItemSchema.SharingDetail,
			MessageItemSchema.SharingInstanceGuid,
			MessageItemSchema.SharingProviderGuid,
			MessageItemSchema.SharingLocalUid,
			MessageItemSchema.SharingLastSync,
			MessageItemSchema.SharingDetailedStatus,
			MessageItemSchema.SharingDiagnostics,
			AggregationSubscriptionMessageSchema.SharingPoisonCallstack,
			MessageItemSchema.SharingRemotePath,
			AggregationSubscriptionMessageSchema.SharingInitiatorName,
			AggregationSubscriptionMessageSchema.SharingInitiatorSmtp,
			AggregationSubscriptionMessageSchema.SharingRemoteUser,
			AggregationSubscriptionMessageSchema.SharingRemotePass,
			AggregationSubscriptionMessageSchema.SharingLastSuccessSyncTime,
			AggregationSubscriptionMessageSchema.SharingAggregationStatus,
			AggregationSubscriptionMessageSchema.SharingWlidAuthPolicy,
			AggregationSubscriptionMessageSchema.SharingWlidUserPuid,
			AggregationSubscriptionMessageSchema.SharingWlidAuthToken,
			AggregationSubscriptionMessageSchema.SharingWlidAuthTokenExpireTime,
			AggregationSubscriptionMessageSchema.SharingMinSyncPollInterval,
			AggregationSubscriptionMessageSchema.SharingMinSettingPollInterval,
			AggregationSubscriptionMessageSchema.SharingSyncMultiplier,
			AggregationSubscriptionMessageSchema.SharingMaxObjectsInSync,
			AggregationSubscriptionMessageSchema.SharingMaxNumberOfEmails,
			AggregationSubscriptionMessageSchema.SharingMaxNumberOfFolders,
			AggregationSubscriptionMessageSchema.SharingMaxAttachments,
			AggregationSubscriptionMessageSchema.SharingMaxMessageSize,
			AggregationSubscriptionMessageSchema.SharingMaxRecipients,
			AggregationSubscriptionMessageSchema.SharingMigrationState,
			AggregationSubscriptionMessageSchema.SharingAggregationType,
			AggregationSubscriptionMessageSchema.SharingSubscriptionConfiguration,
			AggregationSubscriptionMessageSchema.SharingAggregationProtocolVersion,
			AggregationSubscriptionMessageSchema.SharingAggregationProtocolName,
			AggregationSubscriptionMessageSchema.SharingSubscriptionName,
			MessageItemSchema.SharingSubscriptionVersion,
			MessageItemSchema.SharingSendAsState,
			MessageItemSchema.SharingSendAsValidatedEmail,
			AggregationSubscriptionMessageSchema.SharingSubscriptionCreationType,
			AggregationSubscriptionMessageSchema.SharingSendAsVerificationEmailState,
			AggregationSubscriptionMessageSchema.SharingSendAsVerificationMessageId,
			AggregationSubscriptionMessageSchema.SharingSendAsVerificationTimestamp,
			AggregationSubscriptionMessageSchema.SharingSubscriptionEvents,
			AggregationSubscriptionMessageSchema.SharingSubscriptionExclusionFolders,
			AggregationSubscriptionMessageSchema.SharingSubscriptionItemsSynced,
			AggregationSubscriptionMessageSchema.SharingSubscriptionItemsSkipped,
			AggregationSubscriptionMessageSchema.SharingSubscriptionTotalItemsInSourceMailbox,
			AggregationSubscriptionMessageSchema.SharingSubscriptionTotalSizeOfSourceMailbox,
			AggregationSubscriptionMessageSchema.SharingAdjustedLastSuccessfulSyncTime,
			AggregationSubscriptionMessageSchema.SharingOutageDetectionDiagnostics,
			AggregationSubscriptionMessageSchema.SharingSubscriptionSyncPhase,
			AggregationSubscriptionMessageSchema.SharingLastSyncNowRequest,
			AggregationSubscriptionMessageSchema.SharingInitialSyncInRecoveryMode
		};

		private static readonly PropertyDefinition[] IMAPSearchColumnsIndex = new PropertyDefinition[]
		{
			StoreObjectSchema.ItemClass,
			MessageItemSchema.SharingDetail,
			MessageItemSchema.SharingInstanceGuid,
			MessageItemSchema.SharingProviderGuid,
			MessageItemSchema.SharingLocalUid,
			MessageItemSchema.SharingLastSync,
			MessageItemSchema.SharingDetailedStatus,
			MessageItemSchema.SharingDiagnostics,
			AggregationSubscriptionMessageSchema.SharingPoisonCallstack,
			MessageItemSchema.SharingRemotePath,
			AggregationSubscriptionMessageSchema.SharingInitiatorName,
			AggregationSubscriptionMessageSchema.SharingInitiatorSmtp,
			AggregationSubscriptionMessageSchema.SharingRemoteUser,
			AggregationSubscriptionMessageSchema.SharingRemotePass,
			AggregationSubscriptionMessageSchema.SharingLastSuccessSyncTime,
			AggregationSubscriptionMessageSchema.SharingSyncRange,
			AggregationSubscriptionMessageSchema.SharingAggregationStatus,
			AggregationSubscriptionMessageSchema.SharingMigrationState,
			AggregationSubscriptionMessageSchema.SharingAggregationType,
			AggregationSubscriptionMessageSchema.SharingSubscriptionConfiguration,
			AggregationSubscriptionMessageSchema.SharingAggregationProtocolVersion,
			AggregationSubscriptionMessageSchema.SharingAggregationProtocolName,
			AggregationSubscriptionMessageSchema.SharingSubscriptionName,
			MessageItemSchema.SharingSubscriptionVersion,
			MessageItemSchema.SharingSendAsState,
			MessageItemSchema.SharingSendAsValidatedEmail,
			AggregationSubscriptionMessageSchema.SharingSubscriptionCreationType,
			AggregationSubscriptionMessageSchema.SharingSendAsVerificationEmailState,
			AggregationSubscriptionMessageSchema.SharingSendAsVerificationMessageId,
			AggregationSubscriptionMessageSchema.SharingSendAsVerificationTimestamp,
			AggregationSubscriptionMessageSchema.SharingSubscriptionEvents,
			AggregationSubscriptionMessageSchema.SharingSubscriptionExclusionFolders,
			AggregationSubscriptionMessageSchema.SharingImapPathPrefix,
			AggregationSubscriptionMessageSchema.SharingSubscriptionItemsSynced,
			AggregationSubscriptionMessageSchema.SharingSubscriptionItemsSkipped,
			AggregationSubscriptionMessageSchema.SharingSubscriptionTotalItemsInSourceMailbox,
			AggregationSubscriptionMessageSchema.SharingSubscriptionTotalSizeOfSourceMailbox,
			AggregationSubscriptionMessageSchema.SharingAdjustedLastSuccessfulSyncTime,
			AggregationSubscriptionMessageSchema.SharingOutageDetectionDiagnostics,
			AggregationSubscriptionMessageSchema.SharingSubscriptionSyncPhase,
			AggregationSubscriptionMessageSchema.SharingLastSyncNowRequest,
			AggregationSubscriptionMessageSchema.SharingInitialSyncInRecoveryMode
		};

		private static readonly PropertyDefinition[] ConnectSearchColumnsIndex = new PropertyDefinition[]
		{
			StoreObjectSchema.ItemClass,
			MessageItemSchema.SharingDetail,
			MessageItemSchema.SharingInstanceGuid,
			MessageItemSchema.SharingProviderGuid,
			MessageItemSchema.SharingLocalUid,
			MessageItemSchema.SharingLastSync,
			MessageItemSchema.SharingDetailedStatus,
			MessageItemSchema.SharingDiagnostics,
			AggregationSubscriptionMessageSchema.SharingPoisonCallstack,
			MessageItemSchema.SharingRemotePath,
			AggregationSubscriptionMessageSchema.SharingInitiatorName,
			AggregationSubscriptionMessageSchema.SharingInitiatorSmtp,
			AggregationSubscriptionMessageSchema.SharingRemoteUser,
			AggregationSubscriptionMessageSchema.SharingRemotePass,
			AggregationSubscriptionMessageSchema.SharingLastSuccessSyncTime,
			AggregationSubscriptionMessageSchema.SharingAggregationStatus,
			AggregationSubscriptionMessageSchema.SharingMigrationState,
			AggregationSubscriptionMessageSchema.SharingAggregationType,
			AggregationSubscriptionMessageSchema.SharingSubscriptionConfiguration,
			AggregationSubscriptionMessageSchema.SharingAggregationProtocolVersion,
			AggregationSubscriptionMessageSchema.SharingAggregationProtocolName,
			AggregationSubscriptionMessageSchema.SharingSubscriptionName,
			MessageItemSchema.SharingSubscriptionVersion,
			MessageItemSchema.SharingSendAsState,
			MessageItemSchema.SharingSendAsValidatedEmail,
			AggregationSubscriptionMessageSchema.SharingSubscriptionCreationType,
			AggregationSubscriptionMessageSchema.SharingSendAsVerificationEmailState,
			AggregationSubscriptionMessageSchema.SharingSendAsVerificationMessageId,
			AggregationSubscriptionMessageSchema.SharingSendAsVerificationTimestamp,
			AggregationSubscriptionMessageSchema.SharingSubscriptionEvents,
			AggregationSubscriptionMessageSchema.SharingSubscriptionExclusionFolders,
			AggregationSubscriptionMessageSchema.SharingSubscriptionItemsSynced,
			AggregationSubscriptionMessageSchema.SharingSubscriptionItemsSkipped,
			AggregationSubscriptionMessageSchema.SharingSubscriptionTotalItemsInSourceMailbox,
			AggregationSubscriptionMessageSchema.SharingSubscriptionTotalSizeOfSourceMailbox,
			AggregationSubscriptionMessageSchema.SharingAdjustedLastSuccessfulSyncTime,
			AggregationSubscriptionMessageSchema.SharingOutageDetectionDiagnostics,
			AggregationSubscriptionMessageSchema.SharingSubscriptionSyncPhase,
			AggregationSubscriptionMessageSchema.SharingLastSyncNowRequest,
			AggregationSubscriptionMessageSchema.SharingEncryptedAccessToken,
			AggregationSubscriptionMessageSchema.SharingAppId,
			AggregationSubscriptionMessageSchema.SharingUserId,
			AggregationSubscriptionMessageSchema.SharingEncryptedAccessTokenSecret,
			AggregationSubscriptionMessageSchema.SharingInitialSyncInRecoveryMode
		};

		private static readonly SortBy[] SortByItemClassInAscendingOrder = new SortBy[]
		{
			new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending)
		};

		private static readonly ComparisonFilter AggregationMessageClassPrefixFilter = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, StoreObjectSchema.ItemClass, "IPM.Aggregation.");

		private static readonly SubscriptionManager instance = new SubscriptionManager();

		private readonly SyncLogSession syncLogSession;

		private readonly SubscriptionNotificationClient notificationClient;

		private readonly MailboxTableSubscriptionPropertyHelper mailboxTableSubscriptionPropertyHelper;

		private readonly SubscriptionMessageHelper messageHelper;

		private readonly SubscriptionUpgrader upgrader;

		private delegate bool SubscriptionProcessor(object[] item);

		private enum SharingColumn
		{
			ItemClass,
			ItemId,
			SharingInstanceGuid,
			SharingSubscriptionName,
			InstanceKey
		}
	}
}
