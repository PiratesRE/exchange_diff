using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.MailboxRules;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Transport.MailboxRules
{
	internal class RecipientContext : RuleEvaluationContext
	{
		public RecipientContext(MessageContext messageContext, Recipient recipient) : base(messageContext)
		{
			this.recipient = recipient;
		}

		protected override IStorePropertyBag PropertyBag
		{
			get
			{
				return this.recipient;
			}
		}

		public override bool CompareSingleValue(PropTag tag, Restriction.RelOp op, object messageValue, object ruleValue)
		{
			bool flag = base.CompareSingleValue(tag, op, messageValue, ruleValue);
			if (flag || tag != PropTag.SearchKey)
			{
				base.TraceDebug<bool>("RecipientContext.CompareSingleValue returning {0}.", flag);
				return flag;
			}
			return base.CompareAddresses(messageValue, ruleValue);
		}

		protected override object CalculatePropertyValue(PropTag tag)
		{
			if (tag == PropTag.SearchKey)
			{
				return RuleUtil.SearchKeyFromParticipant(this.recipient.Participant);
			}
			return null;
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		private Recipient recipient;
	}
}
