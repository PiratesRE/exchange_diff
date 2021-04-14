using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CalendarItemOccurrenceSchema : CalendarItemInstanceSchema
	{
		private CalendarItemOccurrenceSchema()
		{
			base.AddDependencies(new Schema[]
			{
				AttachmentSchema.Instance
			});
		}

		public new static CalendarItemOccurrenceSchema Instance
		{
			get
			{
				if (CalendarItemOccurrenceSchema.instance == null)
				{
					CalendarItemOccurrenceSchema.instance = new CalendarItemOccurrenceSchema();
				}
				return CalendarItemOccurrenceSchema.instance;
			}
		}

		protected override ICollection<PropertyRule> PropertyRules
		{
			get
			{
				if (this.propertyRulesCache == null)
				{
					List<PropertyRule> list = base.PropertyRules.ToList<PropertyRule>();
					list.AddRange(CalendarItemOccurrenceSchema.CalendarItemOccurrencePropertyRules);
					list.Add(PropertyRuleLibrary.PropertyChangeMetadataTracking);
					this.propertyRulesCache = list.ToArray();
				}
				return this.propertyRulesCache;
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static CalendarItemOccurrenceSchema()
		{
			PropertyRule[] array = new PropertyRule[3];
			array[0] = PropertyRuleLibrary.DefaultInvitedForCalendarItem;
			array[1] = PropertyRuleLibrary.ExceptionalBodyFromBody;
			array[2] = new SequenceCompositePropertyRule(string.Empty, delegate(ILocationIdentifierSetter lidSetter)
			{
				lidSetter.SetLocationIdentifier(64671U, LastChangeAction.SequenceCompositePropertyRuleApplied);
			}, new PropertyRule[]
			{
				PropertyRuleLibrary.DefaultIsExceptionFromItemClass,
				PropertyRuleLibrary.RecurrenceBlobToFlags,
				PropertyRuleLibrary.CalendarOriginatorId
			});
			CalendarItemOccurrenceSchema.CalendarItemOccurrencePropertyRules = array;
		}

		[Autoload]
		public static readonly StorePropertyDefinition IsSeriesCancelled = InternalSchema.IsSeriesCancelled;

		internal static readonly PropertyDefinition ExceptionReplaceTime = InternalSchema.ExceptionReplaceTime;

		private static readonly PropertyRule[] CalendarItemOccurrencePropertyRules;

		private static CalendarItemOccurrenceSchema instance;

		private ICollection<PropertyRule> propertyRulesCache;
	}
}
