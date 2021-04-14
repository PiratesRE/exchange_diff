using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class MeetingResponseShape : Shape
	{
		static MeetingResponseShape()
		{
			MeetingResponseShape.defaultProperties.Add(ItemSchema.ItemId);
			MeetingResponseShape.defaultProperties.Add(ItemSchema.Attachments);
			MeetingResponseShape.defaultProperties.Add(ItemSchema.Subject);
			MeetingResponseShape.defaultProperties.Add(ItemSchema.Sensitivity);
			MeetingResponseShape.defaultProperties.Add(ItemSchema.ResponseObjects);
			MeetingResponseShape.defaultProperties.Add(ItemSchema.HasAttachments);
			MeetingResponseShape.defaultProperties.Add(ItemSchema.IsAssociated);
			MeetingResponseShape.defaultProperties.Add(MessageSchema.ToRecipients);
			MeetingResponseShape.defaultProperties.Add(MessageSchema.CcRecipients);
			MeetingResponseShape.defaultProperties.Add(MessageSchema.BccRecipients);
			MeetingResponseShape.defaultProperties.Add(MessageSchema.IsRead);
			MeetingResponseShape.defaultProperties.Add(MeetingMessageSchema.AssociatedCalendarItemId);
			MeetingResponseShape.defaultProperties.Add(MeetingMessageSchema.IsDelegated);
			MeetingResponseShape.defaultProperties.Add(MeetingMessageSchema.IsOutOfDate);
			MeetingResponseShape.defaultProperties.Add(MeetingMessageSchema.HasBeenProcessed);
			MeetingResponseShape.defaultProperties.Add(MeetingMessageSchema.ResponseType);
			MeetingResponseShape.defaultProperties.Add(MeetingResponseSchema.ProposedStart);
			MeetingResponseShape.defaultProperties.Add(MeetingResponseSchema.ProposedEnd);
		}

		private MeetingResponseShape() : base(Schema.MeetingResponse, MeetingResponseSchema.GetSchema(), MeetingMessageShape.CreateShape(), MeetingResponseShape.defaultProperties)
		{
		}

		internal static MeetingResponseShape CreateShape()
		{
			return new MeetingResponseShape();
		}

		private static List<PropertyInformation> defaultProperties = new List<PropertyInformation>();
	}
}
