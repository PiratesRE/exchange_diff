using System;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class Xso14AttachmentsProperty : XsoAttachments12Property
	{
		public Xso14AttachmentsProperty(IdMapping idmapping, PropertyType propertyType = PropertyType.ReadOnly) : base(idmapping, propertyType, Xso14AttachmentsProperty.prefectedProperties)
		{
		}

		public override void Unbind()
		{
			this.evmAttachmentOrders = null;
			base.Unbind();
		}

		protected override Attachment12Data GetAttachmentData(Item message, Attachment attachment, string idbase, ref int index)
		{
			Attachment14Data attachment14Data = (Attachment14Data)base.GetAttachmentData(message, attachment, idbase, ref index);
			if (attachment14Data == null)
			{
				return null;
			}
			if (this.IsEvmMessage())
			{
				if (this.evmAttachmentOrders == null && base.Count > 1)
				{
					this.evmAttachmentOrders = new List<int>(base.Count);
				}
				attachment14Data.Order = Xso14AttachmentsProperty.GetEvmAttachmentOrder(message.GetValueOrDefault<string>(MessageItemSchema.VoiceMessageAttachmentOrder), attachment14Data.DisplayName);
				if (this.evmAttachmentOrders != null)
				{
					if (this.evmAttachmentOrders.Contains(attachment14Data.Order))
					{
						attachment14Data.Order = -1;
					}
					else
					{
						this.evmAttachmentOrders.Add(attachment14Data.Order);
					}
				}
				if (Xso14AttachmentsProperty.IsTheLatestEvmAttachment(attachment14Data.Order))
				{
					attachment14Data.Duration = message.GetValueOrDefault<int>(MessageItemSchema.VoiceMessageDuration, -1);
				}
			}
			return attachment14Data;
		}

		private static int GetEvmAttachmentOrder(string evmAttachmentOrder, string attachmentName)
		{
			if (string.IsNullOrEmpty(evmAttachmentOrder) || string.IsNullOrEmpty(attachmentName))
			{
				return -1;
			}
			int endIndex = evmAttachmentOrder.Length;
			int num = 1;
			for (int i = evmAttachmentOrder.Length - 1; i >= -1; i--)
			{
				if (i == -1 || evmAttachmentOrder[i] == ';')
				{
					if (Xso14AttachmentsProperty.ContainsAttachmentName(evmAttachmentOrder, i + 1, endIndex, attachmentName))
					{
						return num;
					}
					endIndex = i;
					if (i != evmAttachmentOrder.Length - 1)
					{
						num++;
					}
				}
			}
			return -1;
		}

		private static bool ContainsAttachmentName(string evmAttachmentOrder, int startIndex, int endIndex, string attachmentName)
		{
			string text = evmAttachmentOrder.Substring(startIndex, endIndex - startIndex);
			text = text.Trim();
			if (AttachmentHelper.IsProtectedVoiceAttachment(text))
			{
				text = AttachmentHelper.GetUnprotectedVoiceAttachmentName(text);
			}
			return string.CompareOrdinal(text, attachmentName) == 0;
		}

		private static bool IsTheLatestEvmAttachment(int attOrder)
		{
			return attOrder == 1;
		}

		private bool IsEvmMessage()
		{
			foreach (string value in Constants.EvmSupportedItemClassPrefixes)
			{
				if (base.XsoItem.ClassName.StartsWith(value, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private static PropertyDefinition[] prefectedProperties = new PropertyDefinition[]
		{
			MessageItemSchema.VoiceMessageAttachmentOrder,
			MessageItemSchema.VoiceMessageDuration
		};

		private List<int> evmAttachmentOrders;
	}
}
