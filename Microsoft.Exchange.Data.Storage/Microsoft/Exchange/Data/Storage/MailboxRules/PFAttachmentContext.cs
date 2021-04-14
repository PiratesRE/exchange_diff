using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PFAttachmentContext : PFRuleEvaluationContext
	{
		public PFAttachmentContext(PFMessageContext messageContext, Attachment attachment) : base(messageContext)
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
