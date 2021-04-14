using System;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyAccessors.StorageAccessors;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.TypeConversion;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyTranslationRules
{
	internal class DraftStateRules : ITranslationRule<ICalendarItemBase, Event>
	{
		public void FromLeftToRightType(ICalendarItemBase left, Event right)
		{
			bool isDraft;
			CalendarItemAccessors.IsDraft.TryGetValue(left, out isDraft);
			right.IsDraft = isDraft;
		}

		public void FromRightToLeftType(ICalendarItemBase left, Event right)
		{
			bool flag;
			CalendarItemAccessors.HasAttendees.TryGetValue(left, out flag);
			bool flag2;
			CalendarItemAccessors.IsDraft.TryGetValue(left, out flag2);
			bool flag3 = right.IsPropertySet(right.Schema.AttendeesProperty);
			bool flag4 = right.IsPropertySet(right.Schema.IsDraftProperty) ? right.IsDraft : flag2;
			bool flag5 = flag || (flag3 && right.Attendees != null && right.Attendees.Count > 0);
			bool flag6 = flag3 && !left.MeetingRequestWasSent && (right.Attendees == null || right.Attendees.Count == 0);
			if (flag4 && (!flag5 || flag6))
			{
				flag4 = false;
			}
			if (flag && left.MeetingRequestWasSent && flag4 && this.CriticalMeetingPropertiesChanged(right))
			{
				throw new InvalidRequestException(CalendaringStrings.ErrorNeedToSendMessagesWhenCriticalPropertiesAreChanging);
			}
			CalendarItemAccessors.IsDraft.Set(left, flag4);
		}

		private bool CriticalMeetingPropertiesChanged(Event update)
		{
			return DraftStateRules.CriticalProperties.Any(new Func<PropertyDefinition, bool>(update.IsPropertySet));
		}

		private static readonly PropertyDefinition[] CriticalProperties = new PropertyDefinition[]
		{
			SchematizedObject<EventSchema>.SchemaInstance.AttendeesProperty,
			SchematizedObject<EventSchema>.SchemaInstance.EndProperty,
			SchematizedObject<EventSchema>.SchemaInstance.LocationProperty,
			SchematizedObject<EventSchema>.SchemaInstance.StartProperty,
			SchematizedObject<EventSchema>.SchemaInstance.PatternedRecurrenceProperty
		};
	}
}
