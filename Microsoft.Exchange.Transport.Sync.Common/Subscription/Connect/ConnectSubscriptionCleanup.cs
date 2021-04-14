using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.Facebook;
using Microsoft.Exchange.Net.LinkedIn;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConnectSubscriptionCleanup : IConnectSubscriptionCleanup
	{
		public ConnectSubscriptionCleanup(ISubscriptionManager manager)
		{
			SyncUtilities.ThrowIfArgumentNull("manager", manager);
			this.SubscriptionManager = manager;
		}

		private ISubscriptionManager SubscriptionManager { get; set; }

		public void Cleanup(MailboxSession mailbox, IConnectSubscription subscription, bool sendRpcNotification = true)
		{
			SyncUtilities.ThrowIfArgumentNull("mailbox", mailbox);
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			this.DisableSubscription(mailbox, subscription);
			this.DeleteContacts(mailbox, subscription);
			this.DeleteOscSyncEnabledOnServerInSyncLock(mailbox, subscription);
			this.TryRemovePermissions(subscription);
			this.DeleteSubscription(mailbox, subscription, sendRpcNotification);
		}

		internal void DisableSubscription(MailboxSession mailbox, IConnectSubscription subscription)
		{
			subscription.Status = AggregationStatus.Disabled;
			subscription.DetailedAggregationStatus = DetailedAggregationStatus.RemoveSubscription;
			string str = string.IsNullOrEmpty(subscription.Diagnostics) ? string.Empty : Environment.NewLine;
			subscription.Diagnostics = subscription.Diagnostics + str + "Disabling before removing data and the subscription eventually";
			this.SubscriptionManager.UpdateSubscriptionToMailbox(mailbox, subscription);
		}

		internal void DeleteContacts(MailboxSession mailbox, IConnectSubscription subscription)
		{
			try
			{
				StoreObjectId storeObjectId = this.RetrieveFolderId(mailbox, subscription);
				StoreObjectId parentFolderId = mailbox.GetParentFolderId(storeObjectId);
				using (Folder folder = Folder.Bind(mailbox, parentFolderId))
				{
					AggregateOperationResult aggregateOperationResult = folder.DeleteObjects(DeleteItemFlags.HardDelete, new StoreId[]
					{
						storeObjectId
					});
					if (aggregateOperationResult.OperationResult != OperationResult.Succeeded)
					{
						throw new FailedDeletePeopleConnectSubscriptionException(subscription.SubscriptionType);
					}
				}
			}
			catch (ObjectNotFoundException)
			{
			}
		}

		internal void TryRemovePermissions(IConnectSubscription subscription)
		{
			AggregationSubscriptionType subscriptionType = subscription.SubscriptionType;
			IRemoveConnectSubscription removeConnectSubscription;
			if (subscriptionType != AggregationSubscriptionType.Facebook)
			{
				if (subscriptionType != AggregationSubscriptionType.LinkedIn)
				{
					throw new InvalidOperationException("Unknown subscription type: " + subscription.SubscriptionType);
				}
				removeConnectSubscription = ConnectSubscriptionCleanup.InitLinkedInProviderImpl();
			}
			else
			{
				removeConnectSubscription = ConnectSubscriptionCleanup.InitFacebookProviderImpl();
			}
			removeConnectSubscription.TryRemovePermissions(subscription);
		}

		internal void DeleteSubscription(MailboxSession mailbox, IConnectSubscription subscription, bool sendRpcNotification = true)
		{
			this.SubscriptionManager.DeleteSubscription(mailbox, subscription, sendRpcNotification);
		}

		internal void DeleteOscSyncEnabledOnServerInSyncLock(MailboxSession mailbox, IConnectSubscription subscription)
		{
			try
			{
				StoreObjectId messageId = new OscSyncLockLocator(mailbox).Find(subscription.Name, subscription.UserId);
				using (MessageItem messageItem = MessageItem.Bind(mailbox, messageId, ConnectSubscriptionCleanup.PropertiesToLoadFromSyncLock))
				{
					if (messageItem.GetValueOrDefault<bool>(MessageItemSchema.OscSyncEnabledOnServer, false))
					{
						messageItem.OpenAsReadWrite();
						messageItem.Delete(MessageItemSchema.OscSyncEnabledOnServer);
						messageItem.Save(SaveMode.ResolveConflicts);
					}
				}
			}
			catch (ObjectNotFoundException)
			{
			}
		}

		private static IRemoveConnectSubscription InitFacebookProviderImpl()
		{
			IPeopleConnectApplicationConfig peopleConnectApplicationConfig = CachedPeopleConnectApplicationConfig.Instance.ReadFacebook();
			IFacebookClient client = new FacebookClient(new Uri(peopleConnectApplicationConfig.GraphApiEndpoint));
			return new RemoveFacebookSubscription(client);
		}

		private static IRemoveConnectSubscription InitLinkedInProviderImpl()
		{
			IPeopleConnectApplicationConfig peopleConnectApplicationConfig = CachedPeopleConnectApplicationConfig.Instance.ReadLinkedIn();
			ILinkedInWebClient client = new LinkedInWebClient(new LinkedInAppConfig(peopleConnectApplicationConfig.AppId, peopleConnectApplicationConfig.AppSecretClearText, peopleConnectApplicationConfig.ProfileEndpoint, peopleConnectApplicationConfig.ConnectionsEndpoint, peopleConnectApplicationConfig.RemoveAppEndpoint, peopleConnectApplicationConfig.WebRequestTimeout, peopleConnectApplicationConfig.WebProxyUri), NullTracer.Instance);
			return new RemoveLinkedInSubscription(client);
		}

		private StoreObjectId RetrieveFolderId(MailboxSession mailbox, IConnectSubscription subscription)
		{
			return new OscFolderLocator(mailbox).Find(subscription.Name, subscription.UserId);
		}

		internal const string DiagnosticInformation = "Disabling before removing data and the subscription eventually";

		private static readonly PropertyDefinition[] PropertiesToLoadFromSyncLock = new StorePropertyDefinition[]
		{
			MessageItemSchema.OscSyncEnabledOnServer
		};
	}
}
