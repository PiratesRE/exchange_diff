using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class MeetingCancellationShape : Shape
	{
		static MeetingCancellationShape()
		{
			MeetingCancellationShape.defaultProperties.Add(ItemSchema.ItemId);
			MeetingCancellationShape.defaultProperties.Add(ItemSchema.Attachments);
			MeetingCancellationShape.defaultProperties.Add(ItemSchema.Subject);
			MeetingCancellationShape.defaultProperties.Add(ItemSchema.Sensitivity);
			MeetingCancellationShape.defaultProperties.Add(ItemSchema.ResponseObjects);
			MeetingCancellationShape.defaultProperties.Add(ItemSchema.HasAttachments);
			MeetingCancellationShape.defaultProperties.Add(ItemSchema.IsAssociated);
			MeetingCancellationShape.defaultProperties.Add(MessageSchema.ToRecipients);
			MeetingCancellationShape.defaultProperties.Add(MessageSchema.CcRecipients);
			MeetingCancellationShape.defaultProperties.Add(MessageSchema.BccRecipients);
			MeetingCancellationShape.defaultProperties.Add(MessageSchema.IsRead);
			MeetingCancellationShape.defaultProperties.Add(MeetingMessageSchema.AssociatedCalendarItemId);
			MeetingCancellationShape.defaultProperties.Add(MeetingMessageSchema.IsDelegated);
			MeetingCancellationShape.defaultProperties.Add(MeetingMessageSchema.IsOutOfDate);
			MeetingCancellationShape.defaultProperties.Add(MeetingMessageSchema.HasBeenProcessed);
			MeetingCancellationShape.defaultProperties.Add(MeetingMessageSchema.ResponseType);
		}

		private MeetingCancellationShape() : base(Schema.MeetingCancellation, MeetingCancellationSchema.GetSchema(), MeetingMessageShape.CreateShape(), MeetingCancellationShape.defaultProperties)
		{
		}

		internal static MeetingCancellationShape CreateShape()
		{
			return new MeetingCancellationShape();
		}

		private static List<PropertyInformation> defaultProperties = new List<PropertyInformation>();
	}
}
