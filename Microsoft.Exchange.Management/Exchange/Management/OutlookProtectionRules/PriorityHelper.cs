using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Management.OutlookProtectionRules
{
	internal sealed class PriorityHelper
	{
		public PriorityHelper(IConfigDataProvider session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			this.session = session;
			this.rules = new List<TransportRule>();
			IEnumerable<TransportRule> enumerable = session.FindPaged<TransportRule>(null, Utils.GetRuleCollectionId(this.session), false, null, 0);
			foreach (TransportRule item in enumerable)
			{
				this.rules.Add(item);
			}
			this.rules.Sort(new Comparison<TransportRule>(ADRuleStorageManager.CompareTransportRule));
			this.sequenceNumbers = new int[this.rules.Count];
			int num = 0;
			foreach (TransportRule transportRule in this.rules)
			{
				this.sequenceNumbers[num++] = transportRule.Priority;
			}
		}

		public bool IsPriorityValidForInsertion(int priority)
		{
			return priority >= 0 && priority <= this.rules.Count;
		}

		public bool IsPriorityValidForUpdate(int priority)
		{
			return priority >= 0 && priority < this.rules.Count;
		}

		public int GetSequenceNumberToInsertPriority(int priority)
		{
			if (priority < 0)
			{
				throw new ArgumentOutOfRangeException("priority");
			}
			return this.GetSequenceNumberForPriority(priority, int.MinValue);
		}

		public int GetSequenceNumberToUpdatePriority(TransportRule rule, int newPriority)
		{
			if (rule == null)
			{
				throw new ArgumentNullException("rule");
			}
			if (newPriority < 0)
			{
				throw new ArgumentOutOfRangeException("newPriority");
			}
			return this.GetSequenceNumberForPriority(newPriority, rule.Priority);
		}

		public int GetPriorityFromSequenceNumber(int sequenceNumber)
		{
			if (sequenceNumber < 0)
			{
				throw new ArgumentOutOfRangeException("sequenceNumber");
			}
			int num = Array.BinarySearch<int>(this.sequenceNumbers, sequenceNumber);
			if (num >= 0)
			{
				return num;
			}
			return this.rules.Count;
		}

		private int GetSequenceNumberForPriority(int priority, int currentSequenceNumber)
		{
			if (priority < 0)
			{
				throw new ArgumentOutOfRangeException("priority");
			}
			List<TransportRule> list = (from r in this.rules
			where r.Priority != currentSequenceNumber
			select r).ToList<TransportRule>();
			if (priority > list.Count)
			{
				priority = list.Count;
			}
			ADRuleStorageManager.NormalizeInternalSequenceNumbersIfNecessary(list, this.session);
			return ADRuleStorageManager.AssignInternalSequenceNumber(list, priority);
		}

		private readonly IConfigDataProvider session;

		private readonly List<TransportRule> rules;

		private readonly int[] sequenceNumbers;
	}
}
