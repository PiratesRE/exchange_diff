using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal class PolicyBasedThrottler : ISchedulerThrottler
	{
		public PolicyBasedThrottler(IEnumerable<IThrottlingPolicy> policies, IEnumerable<MessageScopeType> relevantTypes, ISchedulerMetering metering)
		{
			ArgumentValidator.ThrowIfNull("policies", policies);
			ArgumentValidator.ThrowIfNull("relevantTypes", relevantTypes);
			ArgumentValidator.ThrowIfNull("metering", metering);
			MessageScopeType[] array = (MessageScopeType[])Enum.GetValues(typeof(MessageScopeType));
			this.orderdScopeTypes = new int[array.Length];
			foreach (MessageScopeType messageScopeType in array)
			{
				this.orderdScopeTypes[(int)messageScopeType] = -1;
			}
			int num = 0;
			foreach (MessageScopeType messageScopeType2 in relevantTypes)
			{
				this.orderdScopeTypes[(int)messageScopeType2] = num;
				num++;
			}
			this.policies = policies;
			this.metering = metering;
		}

		public bool ShouldThrottle(IEnumerable<IMessageScope> scopes, out IMessageScope throttledScope)
		{
			ArgumentValidator.ThrowIfNull("scopes", scopes);
			UsageData usageData = null;
			foreach (IMessageScope messageScope in scopes)
			{
				if (usageData == null)
				{
					usageData = this.metering.GetTotalUsage();
				}
				switch (this.Evaluate(messageScope, usageData))
				{
				case PolicyDecision.Allow:
					throttledScope = null;
					return false;
				case PolicyDecision.Deny:
					throttledScope = messageScope;
					return true;
				}
			}
			throttledScope = null;
			return false;
		}

		public IEnumerable<IMessageScope> GetThrottlingScopes(IEnumerable<IMessageScope> candidateScopes)
		{
			ArgumentValidator.ThrowIfNull("candidateScopes", candidateScopes);
			IDictionary<MessageScopeType, IMessageScope> dictionary = new Dictionary<MessageScopeType, IMessageScope>();
			foreach (IMessageScope messageScope in candidateScopes)
			{
				MessageScopeType type = messageScope.Type;
				if (this.orderdScopeTypes[(int)type] >= 0 && !dictionary.ContainsKey(type))
				{
					dictionary.Add(type, messageScope);
				}
			}
			List<IMessageScope> list = new List<IMessageScope>(dictionary.Values);
			list.Sort((IMessageScope scopeA, IMessageScope scopeB) => this.orderdScopeTypes[(int)scopeA.Type].CompareTo(this.orderdScopeTypes[(int)scopeB.Type]));
			return list;
		}

		public bool ShouldThrottle(IMessageScope scope)
		{
			ArgumentValidator.ThrowIfNull("scope", scope);
			PolicyDecision policyDecision = this.Evaluate(scope, this.metering.GetTotalUsage());
			return PolicyDecision.Deny == policyDecision;
		}

		private PolicyDecision Evaluate(IMessageScope scope, UsageData totalUsage)
		{
			UsageData usage;
			if (this.metering.TryGetUsage(scope, out usage))
			{
				foreach (IThrottlingPolicy throttlingPolicy in this.policies)
				{
					PolicyDecision policyDecision = throttlingPolicy.Evaluate(scope, usage, totalUsage);
					if (policyDecision != PolicyDecision.None)
					{
						return policyDecision;
					}
				}
				return PolicyDecision.None;
			}
			return PolicyDecision.None;
		}

		private readonly IEnumerable<IThrottlingPolicy> policies;

		private readonly int[] orderdScopeTypes;

		private readonly ISchedulerMetering metering;
	}
}
