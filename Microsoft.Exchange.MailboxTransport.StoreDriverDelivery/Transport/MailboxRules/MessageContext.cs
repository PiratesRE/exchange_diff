using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.MailboxRules;
using Microsoft.Exchange.MailboxTransport.StoreDriverDelivery;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Transport.MailboxRules
{
	internal class MessageContext : RuleEvaluationContext
	{
		public MessageContext(Folder folder, MessageItem message, StoreSession session, ProxyAddress recipientAddress, ADRecipientCache<TransportMiniRecipient> recipientCache, long mimeSize, MailItemDeliver mailItemDeliver) : base(folder, message, session, recipientAddress, recipientCache, mimeSize, mailItemDeliver)
		{
		}

		protected MessageContext(RuleEvaluationContext parentContext) : base(parentContext)
		{
		}

		protected override IStorePropertyBag PropertyBag
		{
			get
			{
				return base.Message;
			}
		}

		public override IRuleEvaluationContext GetAttachmentContext(Attachment attachment)
		{
			return new AttachmentContext(this, attachment);
		}

		public override IRuleEvaluationContext GetRecipientContext(Recipient recipient)
		{
			return new RecipientContext(this, recipient);
		}

		public override bool CompareSingleValue(PropTag tag, Restriction.RelOp op, object messageValue, object ruleValue)
		{
			bool flag = base.CompareSingleValue(tag, op, messageValue, ruleValue);
			if (flag || tag != PropTag.SenderSearchKey)
			{
				return flag;
			}
			return base.CompareAddresses(messageValue, ruleValue);
		}

		internal override void SetRecipient(ProxyAddress recipient)
		{
			base.SetRecipient(recipient);
			this.recipientType = null;
		}

		protected override object CalculatePropertyValue(PropTag tag)
		{
			if (tag <= PropTag.MessageRecipMe)
			{
				if (tag <= PropTag.Sensitivity)
				{
					if (tag == PropTag.Importance)
					{
						object propertyValue = base.GetPropertyValue(tag);
						return propertyValue ?? 1;
					}
					if (tag == PropTag.Sensitivity)
					{
						object propertyValue2 = base.GetPropertyValue(tag);
						return propertyValue2 ?? 0;
					}
				}
				else
				{
					if (tag == PropTag.MessageToMe)
					{
						return this.GetRecipientType() == RecipientItemType.To;
					}
					if (tag == PropTag.MessageCcMe)
					{
						return this.GetRecipientType() == RecipientItemType.Cc;
					}
					if (tag == PropTag.MessageRecipMe)
					{
						return this.GetRecipientType() != RecipientItemType.Unknown;
					}
				}
			}
			else if (tag <= PropTag.MessageFlags)
			{
				if (tag == PropTag.SenderSearchKey)
				{
					return RuleUtil.SearchKeyFromParticipant(base.Message.Sender);
				}
				if (tag == PropTag.MessageFlags)
				{
					if (base.Message.AttachmentCollection.Count > 0)
					{
						return 16;
					}
					return 0;
				}
			}
			else if (tag != PropTag.MessageSize)
			{
				if (tag == PropTag.MessageSizeExtended)
				{
					return base.MimeSize;
				}
				if (tag == PropTag.Body)
				{
					return this.GetMessageBody();
				}
			}
			else
			{
				if (base.MimeSize > 2147483647L)
				{
					return int.MaxValue;
				}
				return (int)base.MimeSize;
			}
			return null;
		}

		private string GetMessageBody()
		{
			if (this.body == null)
			{
				this.body = RuleEvaluationContextBase.GetMessageBody(this);
			}
			return this.body;
		}

		private RecipientItemType GetRecipientType()
		{
			if (this.recipientType == null)
			{
				this.recipientType = new RecipientItemType?(RuleEvaluationContextBase.GetRecipientType(this));
			}
			return this.recipientType.Value;
		}

		private string body;

		private RecipientItemType? recipientType;
	}
}
