using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ParkedMeetingMessageSchema : ItemSchema
	{
		public new static ParkedMeetingMessageSchema Instance
		{
			get
			{
				if (ParkedMeetingMessageSchema.instance == null)
				{
					ParkedMeetingMessageSchema.instance = new ParkedMeetingMessageSchema();
				}
				return ParkedMeetingMessageSchema.instance;
			}
		}

		[Autoload]
		public static readonly StorePropertyDefinition ParkedCorrelationId = InternalSchema.ParkedCorrelationId;

		[Autoload]
		public static readonly StorePropertyDefinition OriginalMessageId = InternalSchema.OriginalMessageId;

		[Autoload]
		public static readonly StorePropertyDefinition CleanGlobalObjectId = InternalSchema.CleanGlobalObjectId;

		[Autoload]
		public static readonly StorePropertyDefinition AppointmentSequenceNumber = InternalSchema.AppointmentSequenceNumber;

		private static ParkedMeetingMessageSchema instance;
	}
}
