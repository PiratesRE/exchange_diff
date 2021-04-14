using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class MeetingMessageShape : Shape
	{
		static MeetingMessageShape()
		{
			MeetingMessageShape.defaultProperties.Add(ItemSchema.ItemId);
			MeetingMessageShape.defaultProperties.Add(ItemSchema.Attachments);
			MeetingMessageShape.defaultProperties.Add(ItemSchema.Subject);
			MeetingMessageShape.defaultProperties.Add(ItemSchema.Sensitivity);
			MeetingMessageShape.defaultProperties.Add(ItemSchema.ResponseObjects);
			MeetingMessageShape.defaultProperties.Add(ItemSchema.HasAttachments);
			MeetingMessageShape.defaultProperties.Add(ItemSchema.IsAssociated);
			MeetingMessageShape.defaultProperties.Add(MessageSchema.ToRecipients);
			MeetingMessageShape.defaultProperties.Add(MessageSchema.CcRecipients);
			MeetingMessageShape.defaultProperties.Add(MessageSchema.BccRecipients);
			MeetingMessageShape.defaultProperties.Add(MessageSchema.IsRead);
			MeetingMessageShape.defaultProperties.Add(MeetingMessageSchema.AssociatedCalendarItemId);
			MeetingMessageShape.defaultProperties.Add(MeetingMessageSchema.IsDelegated);
			MeetingMessageShape.defaultProperties.Add(MeetingMessageSchema.IsOutOfDate);
			MeetingMessageShape.defaultProperties.Add(MeetingMessageSchema.HasBeenProcessed);
			MeetingMessageShape.defaultProperties.Add(MeetingMessageSchema.ResponseType);
		}

		private MeetingMessageShape() : base(Schema.MeetingMessage, MeetingMessageSchema.GetSchema(), MessageShape.CreateShape(), MeetingMessageShape.defaultProperties)
		{
		}

		internal static MeetingMessageShape CreateShape()
		{
			return new MeetingMessageShape();
		}

		private static List<PropertyInformation> defaultProperties = new List<PropertyInformation>();
	}
}
