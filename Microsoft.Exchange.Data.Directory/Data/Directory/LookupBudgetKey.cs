using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal abstract class LookupBudgetKey : BudgetKey
	{
		public LookupBudgetKey(BudgetType budgetType, bool isServiceAccount) : base(budgetType, isServiceAccount)
		{
		}

		internal IThrottlingPolicy Lookup()
		{
			if (BudgetKey.LookupPolicyForTest != null)
			{
				return BudgetKey.LookupPolicyForTest(this);
			}
			return this.InternalLookup();
		}

		internal abstract IThrottlingPolicy InternalLookup();

		protected internal IThrottlingPolicy ADRetryLookup(Func<IThrottlingPolicy> policyLookup)
		{
			IThrottlingPolicy policy = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				policy = policyLookup();
			});
			if (!adoperationResult.Succeeded)
			{
				ExTraceGlobals.ClientThrottlingTracer.TraceError<Exception>((long)this.GetHashCode(), "[LookupBudgetKey.ADRetryLookup] Failed to lookup throttling policy.  Failed with exception '{0}'", adoperationResult.Exception);
				return ThrottlingPolicyCache.Singleton.GetGlobalThrottlingPolicy();
			}
			return policy;
		}

		protected IThrottlingPolicy GetPolicyForRecipient(MiniRecipient recipient)
		{
			if (recipient == null)
			{
				ExTraceGlobals.ClientThrottlingTracer.TraceDebug<LookupBudgetKey>((long)this.GetHashCode(), "[LookupBudgetKey.GetPolicyForRecipient] Passed identifier did not resolve to an AD account: '{0}'.  Using global policy.", this);
				return ThrottlingPolicyCache.Singleton.GetGlobalThrottlingPolicy();
			}
			return recipient.ReadThrottlingPolicy();
		}
	}
}
