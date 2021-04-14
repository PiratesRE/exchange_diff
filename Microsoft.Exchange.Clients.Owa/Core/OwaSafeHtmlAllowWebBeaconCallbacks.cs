using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class OwaSafeHtmlAllowWebBeaconCallbacks : OwaSafeHtmlOutboundCallbacks
	{
		public OwaSafeHtmlAllowWebBeaconCallbacks(OwaContext owaContext, bool isEditableContent) : base(owaContext, isEditableContent)
		{
			this.allowForms = true;
		}

		public OwaSafeHtmlAllowWebBeaconCallbacks(Item item, bool userLogon, OwaContext owaContext, bool isEditableContent) : base(item, userLogon, false, owaContext, isEditableContent)
		{
			this.allowForms = true;
		}

		public OwaSafeHtmlAllowWebBeaconCallbacks(Item item, bool userLogon, bool isEmbedded, string itemUrl, OwaContext owaContext, bool isEditableContent) : base(item, userLogon, isEmbedded, itemUrl, false, owaContext, isEditableContent)
		{
			this.allowForms = true;
		}

		public override bool HasBlockedImages
		{
			get
			{
				return false;
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
				filterAttribute.Write();
			}
		}
	}
}
