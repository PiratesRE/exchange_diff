using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MeetingMessageInstanceSchema : MeetingMessageSchema
	{
		protected MeetingMessageInstanceSchema()
		{
			base.AddDependencies(new Schema[]
			{
				CalendarItemSchema.Instance
			});
		}

		public new static MeetingMessageInstanceSchema Instance
		{
			get
			{
				if (MeetingMessageInstanceSchema.instance == null)
				{
					MeetingMessageInstanceSchema.instance = new MeetingMessageInstanceSchema();
				}
				return MeetingMessageInstanceSchema.instance;
			}
		}

		internal override void CoreObjectUpdate(CoreItem coreItem, CoreItemOperation operation)
		{
			CalendarItemBase.CoreObjectUpdateLocationAddress(coreItem);
			base.CoreObjectUpdate(coreItem, operation);
		}

		protected override ICollection<PropertyRule> PropertyRules
		{
			get
			{
				return base.PropertyRules.Concat(MeetingMessageInstanceSchema.MeetingMessageInstanceSchemaPropertyRules);
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static MeetingMessageInstanceSchema()
		{
			PropertyRule[] array = new PropertyRule[4];
			array[0] = PropertyRuleLibrary.DefaultCleanGlobalObjectIdFromGlobalObjectId;
			array[1] = PropertyRuleLibrary.LocationLidWhere;
			array[2] = new SequenceCompositePropertyRule(string.Empty, delegate(ILocationIdentifierSetter lidSetter)
			{
				lidSetter.SetLocationIdentifier(56479U, LastChangeAction.SequenceCompositePropertyRuleApplied);
			}, new PropertyRule[]
			{
				PropertyRuleLibrary.NativeStartTimeForMessage,
				PropertyRuleLibrary.NativeStartTimeToReminderTime,
				PropertyRuleLibrary.DefaultReminderNextTimeFromStartTimeAndOffset,
				PropertyRuleLibrary.NativeEndTimeForMessage,
				PropertyRuleLibrary.ClipEndTimeForSingleMeeting,
				PropertyRuleLibrary.ClipStartTimeForSingleMeeting
			});
			array[3] = new SequenceCompositePropertyRule(string.Empty, delegate(ILocationIdentifierSetter lidSetter)
			{
				lidSetter.SetLocationIdentifier(44191U, LastChangeAction.SequenceCompositePropertyRuleApplied);
			}, new PropertyRule[]
			{
				PropertyRuleLibrary.DefaultInvitedForMeetingMessage,
				PropertyRuleLibrary.DefaultAppointmentStateFromItemClass,
				PropertyRuleLibrary.SchedulePlusPropertiesToRecurrenceBlob,
				PropertyRuleLibrary.RecurrenceBlobToFlags
			});
			MeetingMessageInstanceSchema.MeetingMessageInstanceSchemaPropertyRules = array;
			MeetingMessageInstanceSchema.instance = null;
		}

		[Autoload]
		public static readonly StorePropertyDefinition OwnerAppointmentID = InternalSchema.OwnerAppointmentID;

		[Autoload]
		internal static readonly StorePropertyDefinition RecurrencePattern = InternalSchema.RecurrencePattern;

		[Autoload]
		internal static readonly StorePropertyDefinition RecurrenceType = InternalSchema.CalculatedRecurrenceType;

		[Autoload]
		internal static readonly StorePropertyDefinition ResponseState = InternalSchema.ResponseState;

		[Autoload]
		public static readonly StorePropertyDefinition CalendarProcessingSteps = InternalSchema.CalendarProcessingSteps;

		[Autoload]
		public static readonly StorePropertyDefinition OriginalMeetingType = InternalSchema.OriginalMeetingType;

		[Autoload]
		public static readonly StorePropertyDefinition SideEffects = InternalSchema.SideEffects;

		[Autoload]
		public static readonly StorePropertyDefinition IsProcessed = InternalSchema.IsProcessed;

		[Autoload]
		public static readonly StorePropertyDefinition MapiStartTime = InternalSchema.MapiStartTime;

		[Autoload]
		internal static readonly StorePropertyDefinition MapiEndTime = InternalSchema.MapiEndTime;

		[Autoload]
		internal static readonly StorePropertyDefinition MapiPRStartDate = InternalSchema.MapiPRStartDate;

		[Autoload]
		internal static readonly StorePropertyDefinition MapiPREndDate = InternalSchema.MapiPREndDate;

		[Autoload]
		internal static readonly StorePropertyDefinition StartRecurTime = InternalSchema.StartRecurTime;

		[Autoload]
		internal static readonly StorePropertyDefinition StartRecurDate = InternalSchema.StartRecurDate;

		[Autoload]
		internal static readonly StorePropertyDefinition EndRecureDate = InternalSchema.EndRecurDate;

		[Autoload]
		internal static readonly StorePropertyDefinition EndRecurTime = InternalSchema.EndRecurTime;

		[Autoload]
		internal static readonly StorePropertyDefinition LidSingleInvite = InternalSchema.LidSingleInvite;

		[Autoload]
		internal static readonly StorePropertyDefinition LidDayInterval = InternalSchema.LidDayInterval;

		[Autoload]
		internal static readonly StorePropertyDefinition LidWeekInterval = InternalSchema.LidWeekInterval;

		[Autoload]
		internal static readonly StorePropertyDefinition LidMonthInterval = InternalSchema.LidMonthInterval;

		[Autoload]
		internal static readonly StorePropertyDefinition LidYearInterval = InternalSchema.LidYearInterval;

		[Autoload]
		internal static readonly StorePropertyDefinition LidDayOfWeekMask = InternalSchema.LidDayOfWeekMask;

		[Autoload]
		internal static readonly StorePropertyDefinition LidDayOfMonthMask = InternalSchema.LidDayOfMonthMask;

		[Autoload]
		internal static readonly StorePropertyDefinition LidMonthOfYearMask = InternalSchema.LidMonthOfYearMask;

		[Autoload]
		internal static readonly StorePropertyDefinition LidFirstDayOfWeek = InternalSchema.LidFirstDayOfWeek;

		[Autoload]
		internal static readonly StorePropertyDefinition LidRecurType = InternalSchema.LidRecurType;

		[Autoload]
		internal static readonly StorePropertyDefinition LidTimeZone = InternalSchema.LidTimeZone;

		[Autoload]
		public static readonly StorePropertyDefinition GlobalObjectId = InternalSchema.GlobalObjectId;

		[Autoload]
		internal static readonly StorePropertyDefinition MasterGlobalObjectId = InternalSchema.MasterGlobalObjectId;

		[Autoload]
		internal static readonly StorePropertyDefinition LidWhere = InternalSchema.LidWhere;

		[Autoload]
		internal static readonly StorePropertyDefinition AppointmentRecurrenceBlob = InternalSchema.AppointmentRecurrenceBlob;

		[Autoload]
		public static readonly StorePropertyDefinition IsException = InternalSchema.IsException;

		[Autoload]
		public static readonly StorePropertyDefinition PropertyChangeMetadataRaw = InternalSchema.PropertyChangeMetadataRaw;

		private static readonly PropertyRule[] MeetingMessageInstanceSchemaPropertyRules;

		private static MeetingMessageInstanceSchema instance;
	}
}
