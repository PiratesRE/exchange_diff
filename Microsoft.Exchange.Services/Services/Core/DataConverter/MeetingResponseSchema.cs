using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class MeetingResponseSchema : Schema
	{
		static MeetingResponseSchema()
		{
			XmlElementInformation[] xmlElements = new XmlElementInformation[]
			{
				MeetingResponseSchema.Start,
				MeetingResponseSchema.End,
				MeetingResponseSchema.Location,
				CalendarItemSchema.AttendeeSpecific.Recurrence,
				MeetingResponseSchema.CalendarItemType,
				MeetingResponseSchema.ProposedStart,
				MeetingResponseSchema.ProposedEnd,
				MeetingResponseSchema.EnhancedLocation
			};
			MeetingResponseSchema.schema_Exchange2012AndLater = new MeetingResponseSchema(xmlElements);
		}

		private MeetingResponseSchema(XmlElementInformation[] xmlElements) : base(xmlElements)
		{
			IList<PropertyInformation> propertyInformationListByShapeEnum = base.GetPropertyInformationListByShapeEnum(ShapeEnum.AllProperties);
			propertyInformationListByShapeEnum.Remove(MeetingResponseSchema.EnhancedLocation);
		}

		public static Schema GetSchema()
		{
			if (!ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012))
			{
				return MeetingMessageSchema.GetSchema();
			}
			return MeetingResponseSchema.schema_Exchange2012AndLater;
		}

		public static readonly PropertyInformation Start = new PropertyInformation(PropertyUriEnum.Start, ExchangeVersion.Exchange2012, CalendarItemInstanceSchema.StartTime, new PropertyCommand.CreatePropertyCommand(StartEndProperty.CreateCommandForStart), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation End = new PropertyInformation(PropertyUriEnum.End, ExchangeVersion.Exchange2012, CalendarItemInstanceSchema.EndTime, new PropertyCommand.CreatePropertyCommand(StartEndProperty.CreateCommandForEnd), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation Location = new PropertyInformation(PropertyUriEnum.Location, ExchangeVersion.Exchange2012, CalendarItemBaseSchema.Location, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation CalendarItemType = new PropertyInformation(PropertyUriEnum.CalendarItemType, ExchangeVersion.Exchange2012, CalendarItemBaseSchema.CalendarItemType, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation ProposedStart = new PropertyInformation(PropertyUriEnum.ProposedStart, ExchangeVersion.Exchange2013, MeetingResponseSchema.AppointmentCounterStartWhole, new PropertyCommand.CreatePropertyCommand(StartEndProperty.CreateCommandForProposedStart), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation ProposedEnd = new PropertyInformation(PropertyUriEnum.ProposedEnd, ExchangeVersion.Exchange2013, MeetingResponseSchema.AppointmentCounterEndWhole, new PropertyCommand.CreatePropertyCommand(StartEndProperty.CreateCommandForProposedEnd), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation EnhancedLocation = CalendarItemSchema.EnhancedLocation;

		private static Schema schema_Exchange2012AndLater;
	}
}
