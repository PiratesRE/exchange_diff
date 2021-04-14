using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class MessageShape : Shape
	{
		static MessageShape()
		{
			MessageShape.defaultProperties.Add(ItemSchema.Attachments);
			MessageShape.defaultProperties.Add(ItemSchema.Body);
			MessageShape.defaultProperties.Add(ItemSchema.Categories);
			MessageShape.defaultProperties.Add(ItemSchema.DateTimeCreated);
			MessageShape.defaultProperties.Add(ItemSchema.HasAttachments);
			MessageShape.defaultProperties.Add(ItemSchema.IsAssociated);
			MessageShape.defaultProperties.Add(ItemSchema.ItemId);
			MessageShape.defaultProperties.Add(ItemSchema.ResponseObjects);
			MessageShape.defaultProperties.Add(ItemSchema.Sensitivity);
			MessageShape.defaultProperties.Add(ItemSchema.DateTimeSent);
			MessageShape.defaultProperties.Add(ItemSchema.Size);
			MessageShape.defaultProperties.Add(ItemSchema.Subject);
			MessageShape.defaultProperties.Add(MessageSchema.BccRecipients);
			MessageShape.defaultProperties.Add(MessageSchema.CcRecipients);
			MessageShape.defaultProperties.Add(MessageSchema.From);
			MessageShape.defaultProperties.Add(MessageSchema.IsDeliveryReceiptRequested);
			MessageShape.defaultProperties.Add(MessageSchema.IsRead);
			MessageShape.defaultProperties.Add(MessageSchema.IsReadReceiptRequested);
			MessageShape.defaultProperties.Add(MessageSchema.ToRecipients);
			MessageShape.defaultPropertiesForPropertyBag = new List<PropertyInformation>();
			MessageShape.defaultPropertiesForPropertyBag.InsertRange(0, MessageShape.defaultProperties);
			MessageShape.defaultPropertiesForPropertyBag.Remove(ItemSchema.Body);
			MessageShape.defaultPropertiesForPropertyBag.Remove(MessageSchema.IsDeliveryReceiptRequested);
			MessageShape.defaultPropertiesForPropertyBag.Remove(MessageSchema.IsReadReceiptRequested);
		}

		private MessageShape(List<PropertyInformation> defaultProperties) : base(Schema.Message, MessageSchema.GetSchema(), ItemShape.CreateShape(), defaultProperties)
		{
		}

		internal static MessageShape CreateShape()
		{
			return new MessageShape(MessageShape.defaultProperties);
		}

		internal static MessageShape CreateShapeForPropertyBag()
		{
			return new MessageShape(MessageShape.defaultPropertiesForPropertyBag);
		}

		private static List<PropertyInformation> defaultProperties = new List<PropertyInformation>();

		private static List<PropertyInformation> defaultPropertiesForPropertyBag;
	}
}
