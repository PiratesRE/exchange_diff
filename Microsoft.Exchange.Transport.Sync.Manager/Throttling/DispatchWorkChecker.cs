using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Manager.Throttling
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DispatchWorkChecker
	{
		internal DispatchWorkChecker(SyncLogSession syncLogSession, IDispatchEntryManager dispatchEntryManager, ISyncManagerConfiguration configuration, IHealthLogDispatchEntryReporter healthLogDispatchEntryReporter)
		{
			this.syncLogSession = syncLogSession;
			this.dispatchEntryManager = dispatchEntryManager;
			this.healthLogDispatchEntryReporter = healthLogDispatchEntryReporter;
		}

		protected IHealthLogDispatchEntryReporter HealthLogDispatchEntryReporter
		{
			get
			{
				return this.healthLogDispatchEntryReporter;
			}
		}

		internal bool CanAttemptDispatchForWorkType(DispatchTrigger dispatchTrigger, WorkType workTypeAttempting, WorkType? completedWorkType)
		{
			if (completedWorkType != null)
			{
				WorkTypeDefinition workTypeDefinition = WorkTypeManager.Instance.GetWorkTypeDefinition(completedWorkType.Value);
				WorkTypeDefinition workTypeDefinition2 = WorkTypeManager.Instance.GetWorkTypeDefinition(workTypeAttempting);
				if (workTypeDefinition.IsLightLoad && !workTypeDefinition2.IsLightLoad)
				{
					this.syncLogSession.LogDebugging((TSLID)349UL, "DWC.CanAttemptDispatchForWorkType: Previous sync {0} was light load, new job {1} is heavy load, not doing to trigger dispatch", new object[]
					{
						completedWorkType.Value,
						workTypeAttempting
					});
					return false;
				}
			}
			return true;
		}

		internal bool CanAttemptDispatchForSubscription(DispatchEntry dispatchEntry, out DispatchResult? dispatchResult)
		{
			SyncUtilities.ThrowIfArgumentNull("dispatchEntry", dispatchEntry);
			dispatchResult = null;
			if (this.dispatchEntryManager.ContainsSubscription(dispatchEntry.MiniSubscriptionInformation.SubscriptionGuid))
			{
				this.syncLogSession.LogDebugging((TSLID)351UL, "DWC.CanAttemptDispatchForSubscription: Subscription already being dispatched ID:{0}", new object[]
				{
					dispatchEntry.MiniSubscriptionInformation.SubscriptionGuid
				});
				dispatchResult = new DispatchResult?(DispatchResult.SubscriptionAlreadyDispatched);
				return false;
			}
			if (!ContentAggregationConfig.OwaMailboxPolicyConstraintEnabled)
			{
				this.syncLogSession.LogDebugging((TSLID)115UL, "DWC.CanAttemptDispatchForSubscription: policy checks are disabled.", new object[0]);
			}
			else if (MailboxPolicyConstraint.Instance.WantsDispositionChangedToDeletion(dispatchEntry, this.syncLogSession))
			{
				this.syncLogSession.LogDebugging((TSLID)1111UL, "DWC.CanAttemptDispatchForSubscription: Subscription is targeted for deletion ID:{0}", new object[]
				{
					dispatchEntry.MiniSubscriptionInformation.SubscriptionGuid
				});
				dispatchResult = new DispatchResult?(DispatchResult.PolicyInducedDeletion);
				return false;
			}
			return true;
		}

		protected virtual ExDateTime GetCurrentTime()
		{
			return ExDateTime.UtcNow;
		}

		private readonly IDispatchEntryManager dispatchEntryManager;

		private readonly IHealthLogDispatchEntryReporter healthLogDispatchEntryReporter;

		private readonly SyncLogSession syncLogSession;
	}
}
