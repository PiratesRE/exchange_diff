using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CalendarItemInstanceSchema : CalendarItemBaseSchema
	{
		public new static CalendarItemInstanceSchema Instance
		{
			get
			{
				if (CalendarItemInstanceSchema.instance == null)
				{
					CalendarItemInstanceSchema.instance = new CalendarItemInstanceSchema();
				}
				return CalendarItemInstanceSchema.instance;
			}
		}

		protected override void AddConstraints(List<StoreObjectConstraint> constraints)
		{
			base.AddConstraints(constraints);
			constraints.Add(CalendarItemInstanceSchema.StartTimeMustBeLessThanOrEqualToEndTimeConstraint);
		}

		protected override ICollection<PropertyRule> PropertyRules
		{
			get
			{
				if (this.propertyRulesCache == null)
				{
					this.propertyRulesCache = CalendarItemInstanceSchema.CalendarItemInstancePropertyRules.Concat(base.PropertyRules);
				}
				return this.propertyRulesCache;
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static CalendarItemInstanceSchema()
		{
			PropertyRule[] array = new PropertyRule[1];
			array[0] = new SequenceCompositePropertyRule(string.Empty, delegate(ILocationIdentifierSetter lidSetter)
			{
				lidSetter.SetLocationIdentifier(52508U, LastChangeAction.SequenceCompositePropertyRuleApplied);
			}, new PropertyRule[]
			{
				PropertyRuleLibrary.NativeStartTimeForCalendar,
				PropertyRuleLibrary.NativeEndTimeForCalendar,
				PropertyRuleLibrary.StartTimeEndTimeToDuration,
				PropertyRuleLibrary.NativeStartTimeToReminderTime,
				PropertyRuleLibrary.DefaultReminderNextTimeFromStartTimeAndOffset,
				PropertyRuleLibrary.ClipEndTimeForSingleMeeting,
				PropertyRuleLibrary.ClipStartTimeForSingleMeeting
			});
			CalendarItemInstanceSchema.CalendarItemInstancePropertyRules = array;
		}

		private static CalendarItemInstanceSchema instance = null;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition EndTime = InternalSchema.EndTime;

		[Autoload]
		public static readonly StorePropertyDefinition PropertyChangeMetadata = InternalSchema.PropertyChangeMetadata;

		[Autoload]
		public static readonly StorePropertyDefinition PropertyChangeMetadataRaw = InternalSchema.PropertyChangeMetadataRaw;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition StartTime = InternalSchema.StartTime;

		public static readonly StorePropertyDefinition StartWallClock = InternalSchema.StartWallClock;

		public static readonly StorePropertyDefinition EndWallClock = InternalSchema.EndWallClock;

		public static readonly PropertyComparisonConstraint StartTimeMustBeLessThanOrEqualToEndTimeConstraint = new PropertyComparisonConstraint(InternalSchema.StartTime, InternalSchema.EndTime, ComparisonOperator.LessThanOrEqual);

		[Autoload]
		internal static readonly StorePropertyDefinition MapiPRStartDate = InternalSchema.MapiPRStartDate;

		[Autoload]
		internal static readonly StorePropertyDefinition MapiPREndDate = InternalSchema.MapiPREndDate;

		private static readonly PropertyRule[] CalendarItemInstancePropertyRules;

		private ICollection<PropertyRule> propertyRulesCache;
	}
}
