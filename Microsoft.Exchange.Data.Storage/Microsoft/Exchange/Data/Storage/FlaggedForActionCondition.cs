using System;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FlaggedForActionCondition : Condition
	{
		public static int GetRequestedActionLocalizedStringEnumIndex(string actionString)
		{
			return FlaggedForActionCondition.GetRequestedActionLocalizedStringEnumIndex(actionString, null);
		}

		public static int GetRequestedActionLocalizedStringEnumIndex(string actionString, CultureInfo culture)
		{
			Array values = Enum.GetValues(FlaggedForActionCondition.RequestedActionType);
			for (int i = 0; i < values.Length; i++)
			{
				string a = (culture != null) ? LocalizedDescriptionAttribute.FromEnum(FlaggedForActionCondition.RequestedActionType, values.GetValue(i), culture) : LocalizedDescriptionAttribute.FromEnum(FlaggedForActionCondition.RequestedActionType, values.GetValue(i));
				if (string.Equals(a, actionString, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
			return -1;
		}

		public string Action
		{
			get
			{
				return this.action;
			}
		}

		private FlaggedForActionCondition(Rule rule, string action) : base(ConditionType.FlaggedForActionCondition, rule)
		{
			this.action = action;
		}

		public static FlaggedForActionCondition Create(Rule rule, string action)
		{
			Condition.CheckParams(new object[]
			{
				rule
			});
			RequestedAction requestedAction;
			if (!RequestedAction.Any.ToString().Equals(action, StringComparison.OrdinalIgnoreCase) && EnumValidator.TryParse<RequestedAction>(action, EnumParseOptions.IgnoreCase, out requestedAction))
			{
				return new FlaggedForActionCondition(rule, LocalizedDescriptionAttribute.FromEnum(FlaggedForActionCondition.RequestedActionType, requestedAction));
			}
			return new FlaggedForActionCondition(rule, action);
		}

		internal override Restriction BuildRestriction()
		{
			if (string.Equals(this.action, RequestedAction.Any.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				return Condition.CreatePropertyRestriction<int>((PropTag)277872643U, 2);
			}
			PropTag propertyTag = base.Rule.PropertyDefinitionToPropTagFromCache(Rule.NamedDefinitions[5]);
			return Condition.CreateAndRestriction(new Restriction[]
			{
				Condition.CreatePropertyRestriction<int>((PropTag)277872643U, 2),
				Condition.CreatePropertyRestriction<string>(propertyTag, this.action)
			});
		}

		internal const int MapiFlagStatusValue = 2;

		private readonly string action;

		public static readonly Type RequestedActionType = typeof(RequestedAction);
	}
}
