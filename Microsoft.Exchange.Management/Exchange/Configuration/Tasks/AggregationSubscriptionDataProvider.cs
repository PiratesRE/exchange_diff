using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Aggregation;
using Microsoft.Exchange.Management.TransportSync;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.SendAsVerification;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;
using Microsoft.Exchange.Transport.Sync.SendAs;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AggregationSubscriptionDataProvider : IConfigDataProvider
	{
		public AggregationSubscriptionDataProvider(AggregationTaskType taskType, IRecipientSession session, ADUser adUser)
		{
			session.LinkResolutionServer = ADSession.GetCurrentConfigDC(session.SessionSettings.GetAccountOrResourceForestFqdn());
			session.UseGlobalCatalog = false;
			this.taskType = taskType;
			this.adUser = adUser;
			this.recipientSession = session;
			this.aggregationSubscriptionConstraintChecker = new AggregationSubscriptionConstraintChecker();
			if (this.adUser != null)
			{
				try
				{
					this.primaryExchangePrincipal = ExchangePrincipal.FromADUser(this.recipientSession.SessionSettings, this.adUser, RemotingOptions.AllowCrossSite);
				}
				catch (ObjectNotFoundException innerException)
				{
					throw new MailboxFailureException(innerException);
				}
			}
		}

		public IRecipientSession RecipientSession
		{
			get
			{
				return this.recipientSession;
			}
		}

		public string Source
		{
			get
			{
				return "AggregationSubscriptionDataProvider";
			}
		}

		public ADUser ADUser
		{
			get
			{
				return this.adUser;
			}
		}

		public ExchangePrincipal SubscriptionExchangePrincipal
		{
			get
			{
				return this.primaryExchangePrincipal;
			}
		}

		public string UserLegacyDN
		{
			get
			{
				return this.primaryExchangePrincipal.LegacyDn;
			}
		}

		public bool LoadReport
		{
			get
			{
				return this.loadReport;
			}
			set
			{
				this.loadReport = value;
			}
		}

		public IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			AggregationSubscriptionIdentity aggregationSubscriptionIdentity = (AggregationSubscriptionIdentity)identity;
			try
			{
				using (MailboxSession mailboxSession = this.OpenMailboxSession(aggregationSubscriptionIdentity))
				{
					bool upgradeIfRequired = this.ShouldUpgradeIfRequired();
					PimAggregationSubscription pimAggregationSubscription = (PimAggregationSubscription)SubscriptionManager.GetSubscription(mailboxSession, aggregationSubscriptionIdentity.SubscriptionId, upgradeIfRequired);
					PimSubscriptionProxy pimSubscriptionProxy = pimAggregationSubscription.CreateSubscriptionProxy();
					if (this.loadReport)
					{
						ReportData reportData = SkippedItemUtilities.GetReportData(aggregationSubscriptionIdentity.SubscriptionId);
						reportData.Load(mailboxSession.Mailbox.MapiStore);
						pimSubscriptionProxy.Report = reportData.ToReport();
					}
					return pimSubscriptionProxy;
				}
			}
			catch (LocalizedException ex)
			{
				CommonLoggingHelper.SyncLogSession.LogError((TSLID)1365UL, AggregationTaskUtils.Tracer, "Read: {0} hit exception: {1}.", new object[]
				{
					aggregationSubscriptionIdentity,
					ex
				});
			}
			return null;
		}

		public IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			return this.GetSubscriptions((AggregationSubscriptionQueryFilter)filter);
		}

		public IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			PimSubscriptionProxy[] subscriptions = this.GetSubscriptions((AggregationSubscriptionQueryFilter)filter);
			List<T> list = new List<T>(subscriptions.Length);
			foreach (PimSubscriptionProxy configurable in subscriptions)
			{
				list.Add((T)((object)configurable));
			}
			return list;
		}

		public void Save(IConfigurable instance)
		{
			PimSubscriptionProxy pimSubscriptionProxy = instance as PimSubscriptionProxy;
			switch (pimSubscriptionProxy.ObjectState)
			{
			case ObjectState.New:
				this.NewAggregationSubscription(pimSubscriptionProxy);
				return;
			case ObjectState.Unchanged:
				return;
			case ObjectState.Changed:
				this.UpdateAggregationSubscription(pimSubscriptionProxy);
				return;
			case ObjectState.Deleted:
				throw new InvalidOperationException("Calling Save() on a deleted object is not permitted. Delete() should be used instead.");
			default:
				return;
			}
		}

		public virtual void Delete(IConfigurable instance)
		{
			PimSubscriptionProxy pimSubscriptionProxy = instance as PimSubscriptionProxy;
			this.RemoveAggregationSubscription(pimSubscriptionProxy.Subscription);
		}

		protected MailboxSession OpenMailboxSession(AggregationSubscriptionIdentity subscriptionIdentity)
		{
			ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromLegacyDN(this.recipientSession.SessionSettings, subscriptionIdentity.LegacyDN, RemotingOptions.AllowCrossSite);
			return this.OpenMailboxSession(exchangePrincipal);
		}

		private List<AggregationSubscription> GetAllSubscriptions()
		{
			List<AggregationSubscription> list = null;
			try
			{
				using (MailboxSession mailboxSession = this.OpenMailboxSession())
				{
					bool upgradeIfRequired = this.ShouldUpgradeIfRequired();
					list = SubscriptionManager.GetAllSubscriptions(mailboxSession, AggregationSubscriptionType.All, upgradeIfRequired);
					if (list != null && this.loadReport)
					{
						this.reports.Clear();
						foreach (AggregationSubscription aggregationSubscription in list)
						{
							Guid subscriptionGuid = aggregationSubscription.SubscriptionGuid;
							ReportData reportData = SkippedItemUtilities.GetReportData(subscriptionGuid);
							reportData.Load(mailboxSession.Mailbox.MapiStore);
							if (!this.reports.ContainsKey(subscriptionGuid))
							{
								this.reports.Add(aggregationSubscription.SubscriptionGuid, reportData.ToReport());
							}
						}
					}
				}
			}
			catch (LocalizedException ex)
			{
				CommonLoggingHelper.SyncLogSession.LogError((TSLID)1492UL, AggregationTaskUtils.Tracer, "GetSubscriptions: Hit exception: {0}.", new object[]
				{
					ex
				});
			}
			return list;
		}

		private PimSubscriptionProxy[] GetSubscriptions(AggregationSubscriptionQueryFilter queryFilter)
		{
			List<PimSubscriptionProxy> list = new List<PimSubscriptionProxy>(3);
			IList<AggregationSubscription> allSubscriptions = this.GetAllSubscriptions();
			if (allSubscriptions != null)
			{
				foreach (AggregationSubscription aggregationSubscription in allSubscriptions)
				{
					PimAggregationSubscription pimAggregationSubscription = (PimAggregationSubscription)aggregationSubscription;
					if (queryFilter == null || queryFilter.Match(pimAggregationSubscription))
					{
						PimSubscriptionProxy pimSubscriptionProxy = pimAggregationSubscription.CreateSubscriptionProxy();
						if (this.loadReport && this.reports.ContainsKey(aggregationSubscription.SubscriptionGuid))
						{
							pimSubscriptionProxy.Report = this.reports[aggregationSubscription.SubscriptionGuid];
						}
						list.Add(pimSubscriptionProxy);
					}
				}
			}
			return list.ToArray();
		}

		private void NewAggregationSubscription(PimSubscriptionProxy pimSubscriptionProxy)
		{
			PimAggregationSubscription subscription = pimSubscriptionProxy.Subscription;
			ExchangePrincipal exchangePrincipal = this.primaryExchangePrincipal;
			IList<AggregationSubscription> allSubscriptions = this.GetAllSubscriptions();
			int userMaximumSubscriptionAllowed = this.GetUserMaximumSubscriptionAllowed();
			this.aggregationSubscriptionConstraintChecker.CheckNewSubscriptionConstraints(subscription, allSubscriptions, userMaximumSubscriptionAllowed);
			bool flag = false;
			try
			{
				DelayedEmailSender delayedEmailSender = null;
				if (pimSubscriptionProxy.SendAsCheckNeeded)
				{
					delayedEmailSender = this.SetAppropriateSendAsState(subscription);
				}
				flag = true;
				using (MailboxSession mailboxSession = this.OpenMailboxSession(exchangePrincipal))
				{
					SubscriptionManager.CreateSubscription(mailboxSession, subscription);
					if (pimSubscriptionProxy.SendAsCheckNeeded)
					{
						this.PostSaveSendAsStateProcessing(subscription, delayedEmailSender, mailboxSession);
					}
				}
			}
			catch (LocalizedException ex)
			{
				if (!flag)
				{
					CommonLoggingHelper.SyncLogSession.LogError((TSLID)1504UL, AggregationTaskUtils.Tracer, "NewAggregationSubscription: {0}. Failed to set send as state with exception: {1}.", new object[]
					{
						subscription.Name,
						ex
					});
				}
				else
				{
					CommonLoggingHelper.SyncLogSession.LogError((TSLID)1505UL, AggregationTaskUtils.Tracer, "NewAggregationSubscription: {0}. Failed to open mailbox session with exception: {1}.", new object[]
					{
						subscription.Name,
						ex
					});
				}
				throw new FailedCreateAggregationSubscriptionException(subscription.Name, ex);
			}
		}

		private void RemoveAggregationSubscription(PimAggregationSubscription subscription)
		{
			try
			{
				ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromLegacyDN(this.recipientSession.SessionSettings, subscription.UserLegacyDN, RemotingOptions.AllowCrossSite);
				using (MailboxSession mailboxSession = this.OpenMailboxSession(exchangePrincipal))
				{
					SubscriptionManager.Instance.DeleteSubscription(mailboxSession, subscription, true);
				}
			}
			catch (LocalizedException ex)
			{
				CommonLoggingHelper.SyncLogSession.LogError((TSLID)1506UL, AggregationTaskUtils.Tracer, "RemoveAggregationSubscription: {0} hit exception: {1}.", new object[]
				{
					subscription.Name,
					ex
				});
				throw new FailedDeleteAggregationSubscriptionException(subscription.Name, ex);
			}
		}

		private void UpdateAggregationSubscription(PimSubscriptionProxy pimSubscriptionProxy)
		{
			PimAggregationSubscription subscription = pimSubscriptionProxy.Subscription;
			try
			{
				using (MailboxSession mailboxSession = this.OpenMailboxSession(subscription.SubscriptionIdentity))
				{
					IList<AggregationSubscription> allSubscriptions = this.GetAllSubscriptions();
					this.aggregationSubscriptionConstraintChecker.CheckUpdateSubscriptionConstraints(subscription, allSubscriptions);
					DelayedEmailSender delayedEmailSender = null;
					if (pimSubscriptionProxy.SendAsCheckNeeded)
					{
						delayedEmailSender = this.SetAppropriateSendAsState(subscription);
					}
					SubscriptionManager.SetSubscriptionAndSyncNow(mailboxSession, subscription);
					if (pimSubscriptionProxy.SendAsCheckNeeded)
					{
						this.PostSaveSendAsStateProcessing(subscription, delayedEmailSender, mailboxSession);
					}
				}
			}
			catch (LocalizedException ex)
			{
				CommonLoggingHelper.SyncLogSession.LogError((TSLID)1246UL, AggregationTaskUtils.Tracer, "UpdateAggregationSubscription: {0} hit exception: {1}.", new object[]
				{
					subscription.Name,
					ex
				});
				throw new FailedSetAggregationSubscriptionException(subscription.Name, ex);
			}
		}

		private MailboxSession OpenMailboxSession()
		{
			return this.OpenMailboxSession(this.primaryExchangePrincipal);
		}

		private MailboxSession OpenMailboxSession(ExchangePrincipal exchangePrincipal)
		{
			return SubscriptionManager.OpenMailbox(exchangePrincipal, ExchangeMailboxOpenType.AsAdministrator, AggregationSubscriptionDataProvider.ClientInfoString);
		}

		private int GetUserMaximumSubscriptionAllowed()
		{
			if (this.adUser.RemoteAccountPolicy == null)
			{
				return int.Parse(((ADPropertyDefinition)RemoteAccountPolicySchema.MaxSyncAccounts).DefaultValue.ToString());
			}
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, this.recipientSession.SessionSettings, 614, "GetUserMaximumSubscriptionAllowed", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Mobility\\Aggregation\\Tasks\\AggregationSubscriptionDataProvider.cs");
			RemoteAccountPolicy remoteAccountPolicy = tenantOrTopologyConfigurationSession.Read<RemoteAccountPolicy>(this.adUser.RemoteAccountPolicy);
			if (remoteAccountPolicy != null)
			{
				return remoteAccountPolicy.MaxSyncAccounts;
			}
			return int.Parse(((ADPropertyDefinition)RemoteAccountPolicySchema.MaxSyncAccounts).DefaultValue.ToString());
		}

		private DelayedEmailSender SetAppropriateSendAsState(PimAggregationSubscription subscription)
		{
			IEmailSender toWrap = subscription.CreateEmailSenderFor(this.adUser, this.primaryExchangePrincipal);
			DelayedEmailSender delayedEmailSender = new DelayedEmailSender(toWrap);
			SendAsAutoProvision sendAsAutoProvision = new SendAsAutoProvision();
			sendAsAutoProvision.SetAppropriateSendAsState(subscription, delayedEmailSender);
			return delayedEmailSender;
		}

		private void PostSaveSendAsStateProcessing(PimAggregationSubscription subscription, DelayedEmailSender delayedEmailSender, MailboxSession mailboxSession)
		{
			SyncUtilities.ThrowIfArgumentNull("delayedEmailSender", delayedEmailSender);
			if (subscription.SendAsState == SendAsState.Enabled)
			{
				AggregationTaskUtils.EnableAlwaysShowFrom(this.primaryExchangePrincipal);
			}
			if (!delayedEmailSender.SendAttempted)
			{
				return;
			}
			IEmailSender emailSender = delayedEmailSender.TriggerDelayedSend();
			SendAsManager sendAsManager = new SendAsManager();
			sendAsManager.UpdateSubscriptionWithDiagnostics(subscription, emailSender);
			SubscriptionManager.SetSubscription(mailboxSession, subscription);
		}

		private bool ShouldUpgradeIfRequired()
		{
			return this.taskType != AggregationTaskType.Get;
		}

		private static readonly string ClientInfoString = "Client=TransportSync;Action=Tasks";

		private readonly AggregationTaskType taskType;

		private readonly IRecipientSession recipientSession;

		private readonly ADUser adUser;

		private ExchangePrincipal primaryExchangePrincipal;

		private AggregationSubscriptionConstraintChecker aggregationSubscriptionConstraintChecker;

		private bool loadReport;

		private Dictionary<Guid, Report> reports = new Dictionary<Guid, Report>();
	}
}
