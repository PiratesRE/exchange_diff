using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal abstract class JournalBase : TransportAction
	{
		protected abstract string MailItemProperty { get; }

		protected abstract string GetItemToAdd(TransportRulesEvaluationContext context);

		public JournalBase(ShortList<Argument> arguments) : base(arguments)
		{
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			MailItem mailItem = transportRulesEvaluationContext.MailItem;
			string itemToAdd = this.GetItemToAdd(transportRulesEvaluationContext);
			List<string> list = null;
			object obj;
			if (mailItem.Properties.TryGetValue(this.MailItemProperty, out obj))
			{
				list = (obj as List<string>);
			}
			if (transportRulesEvaluationContext.RulesEvaluationHistory.History != null)
			{
				List<string> list2 = new List<string>();
				foreach (KeyValuePair<Guid, RuleEvaluationResult> keyValuePair in transportRulesEvaluationContext.RulesEvaluationHistory.History)
				{
					if (keyValuePair.Value.IsMatch)
					{
						list2.Add(keyValuePair.Key.ToString("D"));
					}
				}
				if (mailItem.Properties.ContainsKey("Microsoft.Exchange.JournalRuleIds"))
				{
					((List<string>)mailItem.Properties["Microsoft.Exchange.JournalRuleIds"]).AddRange(list2);
				}
				else
				{
					mailItem.Properties["Microsoft.Exchange.JournalRuleIds"] = list2;
				}
			}
			if (list == null)
			{
				list = new List<string>();
				mailItem.Properties[this.MailItemProperty] = list;
			}
			int num = list.BinarySearch(itemToAdd, StringComparer.OrdinalIgnoreCase);
			if (num >= 0)
			{
				return ExecutionControl.Execute;
			}
			list.Insert(~num, itemToAdd);
			return ExecutionControl.Execute;
		}
	}
}
