using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public abstract class SizeOverPredicate : TransportRulePredicate
	{
		public virtual ByteQuantifiedSize Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		protected SizeOverPredicate(string propertyName)
		{
			this.propertyName = propertyName;
		}

		internal static TransportRulePredicate CreateFromInternalCondition<T>(Condition condition, string propertyName) where T : SizeOverPredicate, new()
		{
			if (condition.ConditionType != ConditionType.Predicate)
			{
				return null;
			}
			PredicateCondition predicateCondition = (PredicateCondition)condition;
			if (!predicateCondition.Name.Equals("greaterThanOrEqual") || !predicateCondition.Property.Name.Equals(propertyName) || predicateCondition.Value.RawValues.Count != 1)
			{
				return null;
			}
			ulong num;
			if (!ulong.TryParse(predicateCondition.Value.RawValues[0], out num))
			{
				return null;
			}
			T t = Activator.CreateInstance<T>();
			if (num == 0UL)
			{
				t.Size = ByteQuantifiedSize.FromKB(0UL);
			}
			else
			{
				t.Size = ByteQuantifiedSize.FromBytes(num);
			}
			return t;
		}

		internal override void Reset()
		{
			this.size = ByteQuantifiedSize.FromKB(0UL);
			base.Reset();
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			base.ValidateRead(errors);
		}

		internal override Condition ToInternalCondition()
		{
			ShortList<string> shortList = new ShortList<string>();
			shortList.Add(this.Size.ToBytes().ToString());
			return TransportRuleParser.Instance.CreatePredicate("greaterThanOrEqual", TransportRuleParser.Instance.CreateProperty(this.propertyName), shortList);
		}

		internal override string GetPredicateParameters()
		{
			return Utils.QuoteCmdletParameter(this.Size.ToString());
		}

		private readonly string propertyName;

		private ByteQuantifiedSize size;
	}
}
