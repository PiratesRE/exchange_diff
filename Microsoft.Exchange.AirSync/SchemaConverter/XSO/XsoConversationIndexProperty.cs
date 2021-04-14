using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoConversationIndexProperty : XsoProperty, IByteArrayProperty, IProperty
	{
		public XsoConversationIndexProperty(PropertyType type) : base(ItemSchema.ConversationIndex, type)
		{
		}

		public byte[] ByteArrayData
		{
			get
			{
				byte[] array = (byte[])base.XsoItem.TryGetProperty(ItemSchema.ConversationIndex);
				if (array == null)
				{
					return null;
				}
				BufferBuilder bufferBuilder = null;
				ConversationIndex index;
				if (ConversationIndex.TryCreate(array, out index) && index != ConversationIndex.Empty && index.Components != null && index.Components.Count > 0)
				{
					bufferBuilder = new BufferBuilder(index.Components.Count * index.Components[0].Length);
					for (int i = 0; i < index.Components.Count; i++)
					{
						bufferBuilder.Append(index.Components[i]);
					}
				}
				if (bufferBuilder == null)
				{
					return null;
				}
				return bufferBuilder.GetBuffer();
			}
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
		}
	}
}
