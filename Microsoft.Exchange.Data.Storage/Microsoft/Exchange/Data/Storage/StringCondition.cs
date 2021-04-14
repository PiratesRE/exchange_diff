using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class StringCondition : Condition
	{
		protected StringCondition(ConditionType conditionType, Rule rule, string[] text) : base(conditionType, rule)
		{
			this.text = text;
		}

		public string[] Text
		{
			get
			{
				return this.text;
			}
		}

		private readonly string[] text;
	}
}
