using System;
using System.Net;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.SubscriptionCompletion;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Rpc.Completion;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SubscriptionCompletionClient
	{
		private SubscriptionCompletionClient()
		{
		}

		internal static bool SkipNotifications
		{
			set
			{
				SubscriptionCompletionClient.skipNotifications = value;
			}
		}

		internal static bool NotifyCompletion(AggregationWorkItem item, SubscriptionCompletionStatus itemError)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (SubscriptionCompletionClient.skipNotifications)
			{
				return true;
			}
			item.SyncLogSession.LogDebugging((TSLID)497UL, SubscriptionCompletionClient.diag, "NotifyCompletion called for item: '{0}' with database Guid '{1}' for mailbox server '{2}'.", new object[]
			{
				item,
				item.DatabaseGuid,
				item.MailboxServer
			});
			if (item.SubscriptionType == AggregationSubscriptionType.Unknown)
			{
				return false;
			}
			byte[] subscriptionCompletionInputBytes = SubscriptionCompletionClient.GetSubscriptionCompletionInputBytes(item, itemError);
			byte[] array = null;
			for (int i = 0; i < 2; i++)
			{
				using (SubscriptionCompletionRpcClient subscriptionCompletionRpcClient = new SubscriptionCompletionRpcClient(item.MailboxServer, SubscriptionCompletionClient.localSystemCredential))
				{
					try
					{
						item.SyncLogSession.LogDebugging((TSLID)498UL, SubscriptionCompletionClient.diag, "Notify completion of sync to server {0}.", new object[]
						{
							item.MailboxServer
						});
						array = subscriptionCompletionRpcClient.SubscriptionComplete(0, subscriptionCompletionInputBytes);
						break;
					}
					catch (RpcException ex)
					{
						item.SyncLogSession.LogError((TSLID)499UL, SubscriptionCompletionClient.diag, "Submission to server {0} failed. Error Code:{1} with RpcException:{2}.", new object[]
						{
							item.MailboxServer,
							ex.ErrorCode,
							ex.Message
						});
					}
				}
			}
			if (array == null)
			{
				return false;
			}
			MdbefPropertyCollection mdbefPropertyCollection = MdbefPropertyCollection.Create(array, 0, array.Length);
			object obj;
			if (mdbefPropertyCollection.TryGetValue(2835349507U, out obj) && obj is int)
			{
				uint num = (uint)((int)obj);
				bool flag = SubscriptionCompletionResult.IsSuccess(num);
				if (flag)
				{
					item.SyncLogSession.LogDebugging((TSLID)500UL, SubscriptionCompletionClient.diag, "Mailbox server '{0}' returned succesfully for completion of item '{1}', result: {2}.", new object[]
					{
						item.MailboxServer,
						item,
						num
					});
				}
				else
				{
					item.SyncLogSession.LogError((TSLID)501UL, SubscriptionCompletionClient.diag, "Mailbox server '{0}' returned failure for completion of item '{1}', result: {2}.", new object[]
					{
						item.MailboxServer,
						item,
						num
					});
				}
				return flag;
			}
			item.SyncLogSession.LogError((TSLID)502UL, SubscriptionCompletionClient.diag, "Mailbox server '{0}' is no longer active to notify completion of item '{1}'.", new object[]
			{
				item.MailboxServer,
				item
			});
			return false;
		}

		private static byte[] GetSubscriptionCompletionInputBytes(AggregationWorkItem item, SubscriptionCompletionStatus resultCode)
		{
			int? num = null;
			if (item.LastWorkItemResultData != null && item.LastWorkItemResultData.Data != null)
			{
				if (item.LastWorkItemResultData.Exception == null || (item.LastWorkItemResultData.Exception is SyncTransientException && item.LastWorkItemResultData.Exception.InnerException is RemoteServerTooSlowException))
				{
					num = new int?(item.LastWorkItemResultData.Data.CloudMoreItemsAvailable ? 1 : 0);
				}
				if (item.LastWorkItemResultData.Data.DisableSubscription)
				{
					resultCode = SubscriptionCompletionStatus.DisableSubscription;
				}
				else if (item.LastWorkItemResultData.Data.InvalidState)
				{
					resultCode = SubscriptionCompletionStatus.InvalidState;
				}
				else if (item.LastWorkItemResultData.Data.DeleteSubscription)
				{
					resultCode = SubscriptionCompletionStatus.DeleteSubscription;
				}
			}
			MdbefPropertyCollection mdbefPropertyCollection = new MdbefPropertyCollection();
			mdbefPropertyCollection[2684354632U] = item.DatabaseGuid;
			mdbefPropertyCollection[2684813384U] = item.UserMailboxGuid;
			mdbefPropertyCollection[2685075528U] = item.SubscriptionId;
			mdbefPropertyCollection[2684420354U] = item.SubscriptionMessageId.GetBytes();
			mdbefPropertyCollection[2685140995U] = (int)item.AggregationType;
			mdbefPropertyCollection[2684616707U] = (int)resultCode;
			mdbefPropertyCollection[2684878851U] = (int)item.SyncPhase;
			if (num != null)
			{
				mdbefPropertyCollection[2684551171U] = num.Value;
			}
			if (item.LastWorkItemResultData != null && item.LastWorkItemResultData.Data != null)
			{
				if (item.LastWorkItemResultData.Data.UpdatedSubscription != null && item.LastWorkItemResultData.Data.UpdatedSubscription.IsValid)
				{
					AggregationSubscription aggregationSubscription = (AggregationSubscription)item.LastWorkItemResultData.Data.UpdatedSubscription;
					mdbefPropertyCollection[2684878851U] = (int)aggregationSubscription.SyncPhase;
					mdbefPropertyCollection[2684944642U] = SerializedSubscription.FromSubscription(aggregationSubscription).GetBytes();
				}
				if (item.LastWorkItemResultData.Data.UpdatedSyncWatermark != null)
				{
					mdbefPropertyCollection[2685009951U] = item.LastWorkItemResultData.Data.UpdatedSyncWatermark;
				}
			}
			return mdbefPropertyCollection.GetBytes();
		}

		private static readonly NetworkCredential localSystemCredential = new NetworkCredential(Environment.MachineName + "$", string.Empty, string.Empty);

		private static readonly Trace diag = ExTraceGlobals.SubscriptionCompletionClientTracer;

		private static bool skipNotifications;
	}
}
