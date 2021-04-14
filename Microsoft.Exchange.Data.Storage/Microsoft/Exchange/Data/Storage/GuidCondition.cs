using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class GuidCondition : Condition
	{
		protected GuidCondition(ConditionType conditionType, Rule rule, Guid[] guids) : base(conditionType, rule)
		{
			bool flag = false;
			if (guids != null && guids.Length > 0)
			{
				for (int i = 0; i < guids.Length; i++)
				{
					if (guids[i].Equals(Guid.Empty))
					{
						flag = true;
						break;
					}
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				rule.ThrowValidateException(delegate
				{
					throw new ArgumentException("guids");
				}, "guids");
			}
			this.guids = guids;
		}

		public Guid[] Guids
		{
			get
			{
				return this.guids;
			}
		}

		private readonly Guid[] guids;
	}
}
