using System;
using System.Collections.Generic;
using Microsoft.Exchange.Entities.DataModel.Messaging;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public class MeetingMessageSchema : EmailMessageSchema
	{
		public MeetingMessageSchema()
		{
			base.RegisterPropertyDefinition(MeetingMessageSchema.StaticOccurrencesExceptionalViewPropertiesProperty);
			base.RegisterPropertyDefinition(MeetingMessageSchema.StaticTypeProperty);
		}

		public TypedPropertyDefinition<List<Event>> OccurrencesExceptionalViewPropertiesProperty
		{
			get
			{
				return MeetingMessageSchema.StaticOccurrencesExceptionalViewPropertiesProperty;
			}
		}

		public TypedPropertyDefinition<MeetingMessageType> TypeProperty
		{
			get
			{
				return MeetingMessageSchema.StaticTypeProperty;
			}
		}

		private static readonly TypedPropertyDefinition<List<Event>> StaticOccurrencesExceptionalViewPropertiesProperty = new TypedPropertyDefinition<List<Event>>("MeetingMessage.OccurrencesExceptionalViewProperties", null, true);

		private static readonly TypedPropertyDefinition<MeetingMessageType> StaticTypeProperty = new TypedPropertyDefinition<MeetingMessageType>("MeetingMessage.Type", MeetingMessageType.Unknown, true);
	}
}
