using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Transport.MailboxRules
{
	internal class AttachmentContext : RuleEvaluationContext
	{
		public AttachmentContext(MessageContext messageContext, Attachment attachment) : base(messageContext)
		{
			this.attachment = attachment;
		}

		protected override IStorePropertyBag PropertyBag
		{
			get
			{
				return this.attachment;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		private Attachment attachment;
	}
}
