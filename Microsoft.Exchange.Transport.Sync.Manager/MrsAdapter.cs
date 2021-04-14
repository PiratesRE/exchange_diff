using System;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.DeltaSync;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Imap;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MrsAdapter
	{
		public static bool UpdateAndCheckMrsJob(SyncLogSession logSession, AggregationSubscription subscription, Guid mdbGuid, Guid orgId)
		{
			TransactionalRequestJob transactionalRequestJob = null;
			bool result = false;
			try
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromExternalDirectoryOrganizationId(orgId);
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 58, "UpdateAndCheckMrsJob", "f:\\15.00.1497\\sources\\dev\\transportSync\\src\\Manager\\MrsAdapter.cs");
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 62, "UpdateAndCheckMrsJob", "f:\\15.00.1497\\sources\\dev\\transportSync\\src\\Manager\\MrsAdapter.cs");
				ADUser aduser;
				if (subscription.AdUserId != null)
				{
					aduser = tenantOrRootOrgRecipientSession.FindADUserByObjectId(subscription.AdUserId);
				}
				else
				{
					aduser = (tenantOrRootOrgRecipientSession.FindByLegacyExchangeDN(subscription.PrimaryMailboxUserLegacyDN) as ADUser);
				}
				if (aduser == null)
				{
					return false;
				}
				VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(aduser.GetContext(null), null, null);
				if (!snapshot.Mrs.TxSyncMrsImapCopy.Enabled)
				{
					return false;
				}
				bool enabled = snapshot.Mrs.TxSyncMrsImapExecute.Enabled;
				using (RequestJobProvider requestJobProvider = new RequestJobProvider(mdbGuid))
				{
					RequestJobObjectId identity = new RequestJobObjectId(subscription.SubscriptionGuid, mdbGuid, null);
					requestJobProvider.AttachToMDB(mdbGuid);
					transactionalRequestJob = (requestJobProvider.Read<TransactionalRequestJob>(identity) as TransactionalRequestJob);
					if (transactionalRequestJob != null)
					{
						if (!transactionalRequestJob.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.ExecutedByTransportSync))
						{
							return true;
						}
						MrsAdapter.RefreshRequestJob(subscription, enabled, ref transactionalRequestJob);
					}
					else
					{
						if (subscription.AggregationType != AggregationType.Aggregation)
						{
							return false;
						}
						if (subscription.SubscriptionType != AggregationSubscriptionType.IMAP)
						{
							return false;
						}
						MrsAdapter.CreateRequestJob(subscription, tenantOrTopologyConfigurationSession, aduser, mdbGuid, enabled, out transactionalRequestJob);
					}
					requestJobProvider.Save(transactionalRequestJob);
				}
			}
			catch (Exception ex)
			{
				logSession.LogError((TSLID)1500UL, "Failed to update the mrs request {0}. Error: {1}", new object[]
				{
					subscription.SubscriptionGuid,
					ex.Message
				});
			}
			finally
			{
				if (transactionalRequestJob != null)
				{
					result = !transactionalRequestJob.RequestJobInternalFlags.HasFlag(RequestJobInternalFlags.ExecutedByTransportSync);
					transactionalRequestJob.Dispose();
					transactionalRequestJob = null;
				}
			}
			return result;
		}

		private static void CreateRequestJob(AggregationSubscription subscription, IConfigurationSession configSession, ADUser user, Guid mdbGuid, bool executeInMrs, out TransactionalRequestJob requestJob)
		{
			requestJob = new TransactionalRequestJob();
			if (subscription.AggregationType == AggregationType.Migration)
			{
				requestJob.WorkloadType = RequestWorkloadType.Onboarding;
			}
			else
			{
				requestJob.WorkloadType = RequestWorkloadType.SyncAggregation;
			}
			requestJob.TimeTracker.SetTimestamp(RequestJobTimestamp.Creation, new DateTime?(DateTime.UtcNow));
			requestJob.TimeTracker.CurrentState = RequestState.Queued;
			requestJob.JobType = MRSJobType.RequestJobE15_TenantHint;
			requestJob.RequestType = MRSRequestType.Sync;
			requestJob.RequestGuid = subscription.SubscriptionGuid;
			requestJob.AllowedToFinishMove = true;
			requestJob.BadItemLimit = 5;
			requestJob.LargeItemLimit = 5;
			requestJob.RehomeRequest = false;
			requestJob.Status = RequestStatus.Queued;
			requestJob.Flags = (RequestFlags.CrossOrg | RequestFlags.Pull);
			requestJob.RequestJobState = JobProcessingState.Ready;
			requestJob.RequestJobInternalFlags = (executeInMrs ? RequestJobInternalFlags.None : RequestJobInternalFlags.ExecutedByTransportSync);
			requestJob.Identity = new RequestJobObjectId(requestJob.RequestGuid, mdbGuid, null);
			requestJob.RequestCreator = subscription.UserExchangeMailboxDisplayName;
			requestJob.Name = subscription.Name;
			requestJob.Suspend = false;
			requestJob.User = user;
			requestJob.UserId = user.Id;
			requestJob.TargetUser = user;
			requestJob.TargetUserId = user.Id;
			requestJob.RequestQueue = ADObjectIdResolutionHelper.ResolveDN(user.Database);
			requestJob.OrganizationId = user.OrganizationId;
			RequestIndexEntryProvider.CreateAndPopulateRequestIndexEntries(requestJob, configSession);
			MrsAdapter.RefreshRequestJob(subscription, executeInMrs, ref requestJob);
		}

		private static void RefreshRequestJob(AggregationSubscription subscription, bool executeInMrs, ref TransactionalRequestJob requestJob)
		{
			if (executeInMrs)
			{
				requestJob.RequestJobInternalFlags &= ~RequestJobInternalFlags.ExecutedByTransportSync;
				requestJob.TimeTracker.SetTimestamp(RequestJobTimestamp.DoNotPickUntil, new DateTime?(DateTime.UtcNow.AddMinutes(1.0)));
			}
			AggregationSubscriptionType subscriptionType = subscription.SubscriptionType;
			if (subscriptionType <= AggregationSubscriptionType.IMAP)
			{
				switch (subscriptionType)
				{
				case AggregationSubscriptionType.Pop:
					MrsAdapter.RefreshPopRequest(subscription as PopAggregationSubscription, ref requestJob);
					return;
				case (AggregationSubscriptionType)3:
					break;
				case AggregationSubscriptionType.DeltaSyncMail:
					MrsAdapter.RefreshDeltaSyncRequest(subscription as DeltaSyncAggregationSubscription, ref requestJob);
					return;
				default:
					if (subscriptionType != AggregationSubscriptionType.IMAP)
					{
						return;
					}
					if (subscription.AggregationType == AggregationType.Migration)
					{
						MrsAdapter.RefreshImapMigrationRequest(subscription as IMAPAggregationSubscription, ref requestJob);
						return;
					}
					MrsAdapter.RefreshImapAggregationRequest(subscription as IMAPAggregationSubscription, ref requestJob);
					return;
				}
			}
			else
			{
				if (subscriptionType != AggregationSubscriptionType.Facebook && subscriptionType != AggregationSubscriptionType.LinkedIn)
				{
					return;
				}
				MrsAdapter.RefreshConnectRequest(subscription as ConnectSubscription, ref requestJob);
			}
		}

		private static void RefreshImapAggregationRequest(IMAPAggregationSubscription imapSubscription, ref TransactionalRequestJob requestJob)
		{
			MrsAdapter.RefreshImapRequest(imapSubscription, ref requestJob);
			requestJob.IncrementalSyncInterval = TimeSpan.FromHours(1.0);
		}

		private static void RefreshImapMigrationRequest(IMAPAggregationSubscription imapSubscription, ref TransactionalRequestJob requestJob)
		{
			MrsAdapter.RefreshImapRequest(imapSubscription, ref requestJob);
			requestJob.IncrementalSyncInterval = TimeSpan.FromDays(1.0);
		}

		private static void RefreshImapRequest(IMAPAggregationSubscription imapSubscription, ref TransactionalRequestJob requestJob)
		{
			requestJob.EmailAddress = imapSubscription.UserEmailAddress;
			requestJob.RemoteCredential = new NetworkCredential(imapSubscription.VerifiedUserName, imapSubscription.LogonPasswordSecured);
			requestJob.RemoteHostName = imapSubscription.IncomingServerName;
			requestJob.RemoteHostPort = imapSubscription.IncomingServerPort;
			requestJob.SecurityMechanism = imapSubscription.IMAPSecurity;
			requestJob.AuthenticationMethod = new AuthenticationMethod?((imapSubscription.IMAPAuthentication == IMAPAuthenticationMechanism.Ntlm) ? AuthenticationMethod.Ntlm : AuthenticationMethod.Basic);
			requestJob.SyncProtocol = SyncProtocol.Imap;
		}

		private static void RefreshPopRequest(PopAggregationSubscription popSubscription, ref TransactionalRequestJob requestJob)
		{
			requestJob.EmailAddress = popSubscription.UserEmailAddress;
			requestJob.RemoteCredential = new NetworkCredential(popSubscription.VerifiedUserName, popSubscription.LogonPasswordSecured);
			requestJob.RemoteHostName = popSubscription.IncomingServerName;
			requestJob.RemoteHostPort = popSubscription.IncomingServerPort;
			requestJob.SyncProtocol = SyncProtocol.Pop;
		}

		private static void RefreshDeltaSyncRequest(DeltaSyncAggregationSubscription deltaSyncSubscription, ref TransactionalRequestJob requestJob)
		{
		}

		private static void RefreshConnectRequest(ConnectSubscription connectSubscription, ref TransactionalRequestJob requestJob)
		{
		}
	}
}
