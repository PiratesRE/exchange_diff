using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class OrganizerProperty : SingleRecipientPropertyBase
	{
		private OrganizerProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static OrganizerProperty CreateCommand(CommandContext commandContext)
		{
			return new OrganizerProperty(commandContext);
		}

		protected override Participant GetParticipant(Item storeItem)
		{
			CalendarItemBase calendarItemBase = storeItem as CalendarItemBase;
			if (calendarItemBase == null)
			{
				calendarItemBase = ((MeetingRequest)storeItem).GetCachedEmbeddedItem();
				return calendarItemBase.Organizer;
			}
			return calendarItemBase.Organizer;
		}

		protected override void SetParticipant(Item storeItem, Participant participant)
		{
		}

		protected override PropertyDefinition GetParticipantDisplayNamePropertyDefinition()
		{
			return CalendarItemBaseSchema.OrganizerDisplayName;
		}

		protected override PropertyDefinition GetParticipantEmailAddressPropertyDefinition()
		{
			return CalendarItemBaseSchema.OrganizerEmailAddress;
		}

		protected override PropertyDefinition GetParticipantRoutingTypePropertyDefinition()
		{
			return CalendarItemBaseSchema.OrganizerType;
		}

		protected override PropertyDefinition GetParticipantSipUriPropertyDefinition()
		{
			return ParticipantSchema.SipUri;
		}
	}
}
