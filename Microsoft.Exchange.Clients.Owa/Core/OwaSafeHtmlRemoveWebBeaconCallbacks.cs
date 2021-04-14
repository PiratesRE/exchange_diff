using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class OwaSafeHtmlRemoveWebBeaconCallbacks : OwaSafeHtmlOutboundCallbacks
	{
		public OwaSafeHtmlRemoveWebBeaconCallbacks(OwaContext owaContext, bool isEditableContent) : base(owaContext, isEditableContent)
		{
		}

		public OwaSafeHtmlRemoveWebBeaconCallbacks(Item item, bool userLogon, OwaContext owaContext, bool isEditableContent) : base(item, userLogon, false, owaContext, isEditableContent)
		{
		}

		public OwaSafeHtmlRemoveWebBeaconCallbacks(Item item, bool userLogon, bool isEmbedded, string itemUrl, OwaContext owaContext, bool isEditableContent) : base(item, userLogon, isEmbedded, itemUrl, false, owaContext, isEditableContent)
		{
		}

		public override bool HasBlockedImages
		{
			get
			{
				return this.hasBlockedImages;
			}
		}

		protected override void ProcessImageTag(HtmlTagContextAttribute filterAttribute, HtmlTagContext context, HtmlWriter writer)
		{
			AttachmentLink attachmentLink = base.IsInlineImage(filterAttribute);
			if (attachmentLink != null)
			{
				base.OutputInlineReference(filterAttribute, context, attachmentLink, writer);
				return;
			}
			if (base.IsSafeUrl(filterAttribute.Value, filterAttribute.Id))
			{
				this.hasBlockedImages = true;
			}
		}
	}
}
