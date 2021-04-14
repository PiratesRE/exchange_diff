using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class MeetingCancellationSchema : Schema
	{
		static MeetingCancellationSchema()
		{
			XmlElementInformation[] xmlElements = new XmlElementInformation[]
			{
				MeetingCancellationSchema.Start,
				MeetingCancellationSchema.End,
				MeetingCancellationSchema.Location,
				CalendarItemSchema.AttendeeSpecific.Recurrence,
				MeetingCancellationSchema.CalendarItemType,
				MeetingCancellationSchema.EnhancedLocation
			};
			MeetingCancellationSchema.schema_Exchange2012AndLater = new MeetingCancellationSchema(xmlElements);
		}

		private MeetingCancellationSchema(XmlElementInformation[] xmlElements) : base(xmlElements)
		{
			IList<PropertyInformation> propertyInformationListByShapeEnum = base.GetPropertyInformationListByShapeEnum(ShapeEnum.AllProperties);
			propertyInformationListByShapeEnum.Remove(MeetingCancellationSchema.EnhancedLocation);
		}

		public static Schema GetSchema()
		{
			if (!ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012))
			{
				return MeetingMessageSchema.GetSchema();
			}
			return MeetingCancellationSchema.schema_Exchange2012AndLater;
		}

		public static readonly PropertyInformation Start = new PropertyInformation(PropertyUriEnum.Start, ExchangeVersion.Exchange2012, CalendarItemInstanceSchema.StartTime, new PropertyCommand.CreatePropertyCommand(StartEndProperty.CreateCommandForStart), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation End = new PropertyInformation(PropertyUriEnum.End, ExchangeVersion.Exchange2012, CalendarItemInstanceSchema.EndTime, new PropertyCommand.CreatePropertyCommand(StartEndProperty.CreateCommandForEnd), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation Location = new PropertyInformation(PropertyUriEnum.Location, ExchangeVersion.Exchange2012, CalendarItemBaseSchema.Location, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation CalendarItemType = new PropertyInformation(PropertyUriEnum.CalendarItemType, ExchangeVersion.Exchange2012, CalendarItemBaseSchema.CalendarItemType, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation EnhancedLocation = CalendarItemSchema.EnhancedLocation;

		private static Schema schema_Exchange2012AndLater;
	}
}
