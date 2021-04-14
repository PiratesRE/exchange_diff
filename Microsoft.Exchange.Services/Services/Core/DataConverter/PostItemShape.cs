using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class PostItemShape : Shape
	{
		static PostItemShape()
		{
			PostItemShape.defaultProperties.Add(ItemSchema.ItemId);
			PostItemShape.defaultProperties.Add(ItemSchema.Subject);
			PostItemShape.defaultProperties.Add(ItemSchema.Attachments);
			PostItemShape.defaultProperties.Add(ItemSchema.HasAttachments);
			PostItemShape.defaultProperties.Add(PostItemSchema.ConversationIndex);
			PostItemShape.defaultProperties.Add(PostItemSchema.ConversationTopic);
			PostItemShape.defaultProperties.Add(PostItemSchema.From);
			PostItemShape.defaultProperties.Add(PostItemSchema.InternetMessageId);
			PostItemShape.defaultProperties.Add(PostItemSchema.PostedTime);
			PostItemShape.defaultProperties.Add(PostItemSchema.References);
			PostItemShape.defaultProperties.Add(PostItemSchema.Sender);
		}

		private PostItemShape() : base(Schema.PostItem, PostItemSchema.GetSchema(), ItemShape.CreateShape(), PostItemShape.defaultProperties)
		{
		}

		internal static PostItemShape CreateShape()
		{
			return new PostItemShape();
		}

		private static List<PropertyInformation> defaultProperties = new List<PropertyInformation>();
	}
}
