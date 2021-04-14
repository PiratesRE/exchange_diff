using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SequenceCompositePropertyRule : PropertyRule
	{
		public SequenceCompositePropertyRule(string name, Action<ILocationIdentifierSetter> onSetWriteEnforceLocationIdentifier, params PropertyRule[] propertyRules) : base(name, onSetWriteEnforceLocationIdentifier, new PropertyReference[0])
		{
			IEnumerable<PropertyDefinition> enumerable = new List<PropertyDefinition>();
			IEnumerable<PropertyDefinition> enumerable2 = new List<PropertyDefinition>();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (PropertyRule propertyRule in propertyRules)
			{
				foreach (PropertyDefinition propertyDefinition in propertyRule.WriteProperties)
				{
					if (enumerable.Contains(propertyDefinition) || enumerable2.Contains(propertyDefinition))
					{
						throw new ArgumentException("ordered rule set has circular write to previous listed propreties. Property: " + propertyDefinition.Name);
					}
				}
				enumerable = enumerable.Union(propertyRule.ReadProperties);
				enumerable2 = enumerable2.Union(propertyRule.WriteProperties);
				stringBuilder.AppendFormat("{0};", propertyRule.ToString());
			}
			base.ReadProperties = enumerable.ToArray<PropertyDefinition>();
			base.WriteProperties = enumerable2.ToArray<PropertyDefinition>();
			this.ruleSequence = propertyRules;
			this.subRuleString = stringBuilder.ToString();
		}

		protected override bool WriteEnforceRule(ICorePropertyBag propertyBag)
		{
			bool flag = false;
			for (int i = 0; i < this.ruleSequence.Length; i++)
			{
				flag |= this.ruleSequence[i].WriteEnforce(propertyBag);
			}
			return flag;
		}

		public override string ToString()
		{
			return this.Name + "(" + this.subRuleString + ")";
		}

		private readonly PropertyRule[] ruleSequence;

		private readonly string subRuleString = string.Empty;
	}
}
