using System;
using System.Collections.Generic;
using Microsoft.Exchange.Entities.DataModel.Messaging;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public class MeetingMessage : EmailMessage<MeetingMessageSchema>
	{
		public List<Event> OccurrencesExceptionalViewProperties
		{
			get
			{
				return base.GetPropertyValueOrDefault<List<Event>>(base.Schema.OccurrencesExceptionalViewPropertiesProperty);
			}
			set
			{
				base.SetPropertyValue<List<Event>>(base.Schema.OccurrencesExceptionalViewPropertiesProperty, value);
			}
		}

		public MeetingMessageType Type
		{
			get
			{
				return base.GetPropertyValueOrDefault<MeetingMessageType>(base.Schema.TypeProperty);
			}
			set
			{
				base.SetPropertyValue<MeetingMessageType>(base.Schema.TypeProperty, value);
			}
		}

		public new static class Accessors
		{
			public static readonly EntityPropertyAccessor<MeetingMessage, List<Event>> OccurrencesExceptionalViewProperties = new EntityPropertyAccessor<MeetingMessage, List<Event>>(SchematizedObject<MeetingMessageSchema>.SchemaInstance.OccurrencesExceptionalViewPropertiesProperty, (MeetingMessage theMessage) => theMessage.OccurrencesExceptionalViewProperties, delegate(MeetingMessage theMessage, List<Event> occurencesExceptionalViewProperties)
			{
				theMessage.OccurrencesExceptionalViewProperties = occurencesExceptionalViewProperties;
			});

			public static readonly EntityPropertyAccessor<MeetingMessage, MeetingMessageType> Type = new EntityPropertyAccessor<MeetingMessage, MeetingMessageType>(SchematizedObject<MeetingMessageSchema>.SchemaInstance.TypeProperty, (MeetingMessage theMessage) => theMessage.Type, delegate(MeetingMessage theMessage, MeetingMessageType type)
			{
				theMessage.Type = type;
			});
		}
	}
}
