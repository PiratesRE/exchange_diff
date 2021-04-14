using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CalendarItemSchema : CalendarItemInstanceSchema
	{
		public new static CalendarItemSchema Instance
		{
			get
			{
				if (CalendarItemSchema.instance == null)
				{
					CalendarItemSchema.instance = new CalendarItemSchema();
				}
				return CalendarItemSchema.instance;
			}
		}

		protected override ICollection<PropertyRule> PropertyRules
		{
			get
			{
				if (this.propertyRulesCache == null)
				{
					List<PropertyRule> list = base.PropertyRules.Concat(CalendarItemSchema.CalendarItemPropertyRules).ToList<PropertyRule>();
					list.AddRange(CalendarItemSchema.PropertyChangesTrackingMetadataRules);
					this.propertyRulesCache = new SequenceCompositePropertyRule[]
					{
						new SequenceCompositePropertyRule(string.Empty, null, list.ToArray())
					};
				}
				return this.propertyRulesCache;
			}
		}

		protected override void AddConstraints(List<StoreObjectConstraint> constraints)
		{
			base.AddConstraints(constraints);
			constraints.Add(new RecurrenceBlobConstraint());
		}

		protected override void CoreObjectUpdateAllAttachmentsHidden(CoreItem coreItem)
		{
			CalendarItem.CoreObjectUpdateAllAttachmentsHidden(coreItem);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static CalendarItemSchema()
		{
			PropertyRule[] array = new PropertyRule[4];
			array[0] = PropertyRuleLibrary.DefaultOwnerAppointmentId;
			array[1] = PropertyRuleLibrary.DefaultRecurrencePattern;
			array[2] = PropertyRuleLibrary.DefaultIsAllDayEvent;
			array[3] = new SequenceCompositePropertyRule(string.Empty, delegate(ILocationIdentifierSetter lidSetter)
			{
				lidSetter.SetLocationIdentifier(40095U, LastChangeAction.SequenceCompositePropertyRuleApplied);
			}, new PropertyRule[]
			{
				PropertyRuleLibrary.DefaultInvitedForCalendarItem,
				PropertyRuleLibrary.DefaultIsExceptionFromItemClass,
				PropertyRuleLibrary.DefaultAppointmentStateFromItemClass,
				PropertyRuleLibrary.RecurrenceBlobToFlags,
				PropertyRuleLibrary.RecurringTimeZone,
				PropertyRuleLibrary.GlobalObjectIdOnRecurringMaster,
				PropertyRuleLibrary.DefaultCleanGlobalObjectIdFromGlobalObjectId,
				PropertyRuleLibrary.CalendarOriginatorId,
				PropertyRuleLibrary.RemoveAppointmentMadeRecurrentFromSeriesRule,
				PropertyRuleLibrary.DefaultOrganizerForAppointments,
				PropertyRuleLibrary.CalendarViewProperties
			});
			CalendarItemSchema.CalendarItemPropertyRules = array;
			CalendarItemSchema.PropertyChangesTrackingMetadataRules = new PropertyRule[]
			{
				PropertyRuleLibrary.MasterPropertyOverrideProtection,
				PropertyRuleLibrary.PropertyChangeMetadataTracking,
				PropertyRuleLibrary.CleanupSeriesOperationFlagsProperty
			};
		}

		[Autoload]
		public static readonly StorePropertyDefinition LastExecutedCalendarInteropAction = InternalSchema.LastExecutedCalendarInteropAction;

		[Autoload]
		public static readonly StorePropertyDefinition InstanceCreationIndex = InternalSchema.InstanceCreationIndex;

		public static readonly StorePropertyDefinition HasExceptionalInboxReminders = InternalSchema.HasExceptionalInboxReminders;

		[Autoload]
		public static readonly StorePropertyDefinition SeriesMasterId = InternalSchema.SeriesMasterId;

		[Autoload]
		public static readonly StorePropertyDefinition PropertyChangeMetadataProcessingFlags = InternalSchema.PropertyChangeMetadataProcessingFlags;

		[Autoload]
		public static readonly StorePropertyDefinition ViewStartTime = InternalSchema.ViewStartTime;

		[Autoload]
		public static readonly StorePropertyDefinition ViewEndTime = InternalSchema.ViewEndTime;

		private static readonly PropertyRule[] CalendarItemPropertyRules;

		private static readonly PropertyRule[] PropertyChangesTrackingMetadataRules;

		private static CalendarItemSchema instance;

		private ICollection<PropertyRule> propertyRulesCache;
	}
}
