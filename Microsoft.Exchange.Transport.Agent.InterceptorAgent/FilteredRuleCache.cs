using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal class FilteredRuleCache
	{
		public FilteredRuleCache(InterceptorAgentEvent eventTypes)
		{
			this.eventTypes = eventTypes;
			InterceptorAgentRulesCache.Instance.RegisterCache(this);
		}

		public IEnumerable<InterceptorAgentRule> Rules
		{
			get
			{
				return this.rulesCache;
			}
		}

		public InterceptorAgentEvent EventTypes
		{
			get
			{
				return this.eventTypes;
			}
		}

		public void UpdateCache(IList<InterceptorAgentRule> allRules)
		{
			ArgumentValidator.ThrowIfNull("allRules", allRules);
			List<InterceptorAgentRule> newCache = new List<InterceptorAgentRule>(this.rulesCache);
			List<InterceptorAgentRule> list = new List<InterceptorAgentRule>(from r in allRules
			where (ushort)(r.Events & this.eventTypes) != 0
			select r);
			List<InterceptorAgentRule> list2 = newCache.Except(list).ToList<InterceptorAgentRule>();
			List<InterceptorAgentRule> list3 = list.Except(newCache).ToList<InterceptorAgentRule>();
			list2.ForEach(delegate(InterceptorAgentRule rule)
			{
				newCache.Remove(rule);
				rule.DeactivateRule();
			});
			list3.ForEach(delegate(InterceptorAgentRule rule)
			{
				newCache.Add(rule);
				rule.ActivateRule();
			});
			this.rulesCache = newCache;
		}

		private readonly InterceptorAgentEvent eventTypes;

		private List<InterceptorAgentRule> rulesCache = new List<InterceptorAgentRule>();
	}
}
