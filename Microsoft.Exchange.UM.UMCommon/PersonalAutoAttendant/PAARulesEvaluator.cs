using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class PAARulesEvaluator : IRuleEvaluator
	{
		private PAARulesEvaluator(IList<IRuleEvaluator> rules)
		{
			this.rules = rules;
		}

		public static PAARulesEvaluator Create(PersonalAutoAttendant paa)
		{
			IList<IRuleEvaluator> list = new List<IRuleEvaluator>();
			if (paa.ExtensionList.Count > 0)
			{
				list.Add(new ExtensionListEvaluator(paa.ExtensionList));
			}
			if (paa.TimeOfDay != TimeOfDayEnum.None)
			{
				list.Add(new TimeOfDayRuleEvaluator(paa.TimeOfDay, paa.WorkingPeriod));
			}
			if (paa.OutOfOffice != OutOfOfficeStatusEnum.None)
			{
				list.Add(new OutOfOfficeRuleEvaluator(paa.OutOfOffice));
			}
			if (paa.CallerIdList.Count > 0)
			{
				list.Add(new CallerIdRuleEvaluator(paa.CallerIdList));
			}
			if (paa.FreeBusy != FreeBusyStatusEnum.None)
			{
				list.Add(new FreeBusyRuleEvaluator(paa.FreeBusy));
			}
			return new PAARulesEvaluator(list);
		}

		public bool Evaluate(IDataLoader dataLoader)
		{
			for (int i = 0; i < this.rules.Count; i++)
			{
				IRuleEvaluator ruleEvaluator = this.rules[i];
				if (!ruleEvaluator.Evaluate(dataLoader))
				{
					return false;
				}
			}
			return true;
		}

		private IList<IRuleEvaluator> rules;
	}
}
