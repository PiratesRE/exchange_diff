using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class MeetingMessageSchema : Schema
	{
		static MeetingMessageSchema()
		{
			XmlElementInformation[] xmlElements = new XmlElementInformation[]
			{
				MeetingMessageSchema.AssociatedCalendarItemId,
				MeetingMessageSchema.IsDelegated,
				MeetingMessageSchema.IsOutOfDate,
				MeetingMessageSchema.HasBeenProcessed,
				MeetingMessageSchema.ResponseType,
				MeetingMessageSchema.ICalendarUid,
				CalendarItemSchema.ICalendarRecurrenceId,
				CalendarItemSchema.ICalendarDateTimeStamp,
				CalendarItemSchema.IsOrganizer
			};
			MeetingMessageSchema.schema = new MeetingMessageSchema(xmlElements);
		}

		private MeetingMessageSchema(XmlElementInformation[] xmlElements) : base(xmlElements)
		{
		}

		public static Schema GetSchema()
		{
			return MeetingMessageSchema.schema;
		}

		public static readonly PropertyInformation AssociatedCalendarItemId = new PropertyInformation(PropertyUriEnum.AssociatedCalendarItemId, ExchangeVersion.Exchange2007, null, new PropertyCommand.CreatePropertyCommand(AssociatedCalendarItemIdProperty.CreateCommand), PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

		public static readonly PropertyInformation HasBeenProcessed = new PropertyInformation(PropertyUriEnum.HasBeenProcessed, ExchangeVersion.Exchange2007, MeetingMessageSchema.CalendarProcessed, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation IsOutOfDate = new PropertyInformation(PropertyUriEnum.IsOutOfDate, ExchangeVersion.Exchange2007, MeetingMessageSchema.IsOutOfDate, new PropertyCommand.CreatePropertyCommand(IsOutOfDateProperty.CreateCommand), PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

		public static readonly PropertyInformation IsDelegated = new PropertyInformation(PropertyUriEnum.IsDelegated, ExchangeVersion.Exchange2007, null, new PropertyCommand.CreatePropertyCommand(IsDelegatedProperty.CreateCommand), PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

		public static readonly PropertyInformation ResponseType = new PropertyInformation(PropertyUriEnum.ResponseType, ExchangeVersion.Exchange2007, MeetingResponseSchema.ResponseType, new PropertyCommand.CreatePropertyCommand(ResponseTypeProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation ICalendarUid = new PropertyInformation(PropertyUriEnum.UID, ExchangeVersion.Exchange2007SP1, ICalendar.UidProperty.PropertyToLoad, new PropertyCommand.CreatePropertyCommand(ICalendar.UidProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation IsOrganizer = new PropertyInformation(PropertyUriEnum.IsOrganizer, ExchangeVersion.Exchange2012, CalendarItemBaseSchema.IsOrganizer, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		private static Schema schema;
	}
}
