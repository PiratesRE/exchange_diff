using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PFMessageContext : PFRuleEvaluationContext
	{
		public PFMessageContext(Folder folder, ICoreItem message, StoreSession session, ProxyAddress recipientAddress, IADRecipientCache recipientCache, long mimeSize) : base(folder, message, session, recipientAddress, recipientCache, mimeSize)
		{
		}

		protected PFMessageContext(PFRuleEvaluationContext parentContext) : base(parentContext)
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
			return new PFAttachmentContext(this, attachment);
		}

		public override IRuleEvaluationContext GetRecipientContext(Recipient recipient)
		{
			return new PFRecipientContext(this, recipient);
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
			else if (tag <= PropTag.MessageSize)
			{
				if (tag == PropTag.SenderSearchKey)
				{
					return RuleUtil.SearchKeyFromParticipant(base.Message.Sender);
				}
				if (tag == PropTag.MessageSize)
				{
					if (base.MimeSize > 2147483647L)
					{
						return int.MaxValue;
					}
					return (int)base.MimeSize;
				}
			}
			else
			{
				if (tag == PropTag.MessageSizeExtended)
				{
					return base.MimeSize;
				}
				if (tag == PropTag.Body)
				{
					return this.GetMessageBody();
				}
				if (tag == (PropTag)1716650242U)
				{
					return base.Message.PropertyBag.TryGetProperty(InternalSchema.SenderEntryId);
				}
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

		private const PropTag TagActiveUserEntryId = (PropTag)1716650242U;

		private string body;

		private RecipientItemType? recipientType;
	}
}
