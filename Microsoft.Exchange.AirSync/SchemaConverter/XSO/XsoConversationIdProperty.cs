using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoConversationIdProperty : XsoProperty, IByteArrayProperty, IProperty
	{
		public XsoConversationIdProperty(PropertyType type) : base(ItemSchema.ConversationId, type)
		{
		}

		public byte[] ByteArrayData
		{
			get
			{
				ConversationId conversationId = (ConversationId)base.XsoItem.TryGetProperty(ItemSchema.ConversationId);
				if (conversationId == null)
				{
					return null;
				}
				return conversationId.GetBytes();
			}
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
		}
	}
}
