using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class OwaSafeHtmlConversationsCallbacks : OwaSafeHtmlOutboundCallbacks
	{
		public OwaSafeHtmlConversationsCallbacks(Item item, bool userLogon, bool isJunkOrPhishing, OwaContext owaContext) : base(item, userLogon, false, null, isJunkOrPhishing, owaContext, false)
		{
			this.openMailtoInNewWindow = true;
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
				this.hasBlockedImagesInCurrentPass = true;
				this.hasBlockedImages = true;
				string value = this.owaContext.UserContext.GetBlankPage(Utilities.PremiumScriptPath) + "#" + OwaSafeHtmlConversationsCallbacks.UrlDelimiter + filterAttribute.Value;
				writer.WriteAttribute(HtmlAttributeId.Src, value);
			}
		}

		private static readonly string UrlDelimiter = "__OWA_FLT000__";
	}
}
