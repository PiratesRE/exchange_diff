using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class AttachmentWebBeaconFilterCallback : HtmlCallbackBase
	{
		public override void ProcessTag(HtmlTagContext tagContext, HtmlWriter htmlWriter)
		{
			HtmlTagId tagId = tagContext.TagId;
			if (tagId == HtmlTagId.Img)
			{
				this.ProcessImageTag(tagContext, htmlWriter);
				return;
			}
			tagContext.WriteTag(true);
		}

		private static void WriteAllAttributesExcept(HtmlTagContext tagContext, HtmlAttributeId attrToSkip)
		{
			foreach (HtmlTagContextAttribute htmlTagContextAttribute in tagContext.Attributes)
			{
				if (htmlTagContextAttribute.Id != attrToSkip)
				{
					htmlTagContextAttribute.Write();
				}
			}
		}

		private void ProcessImageTag(HtmlTagContext tagContext, HtmlWriter htmlWriter)
		{
			string value;
			this.GetLinkSource(tagContext, out value);
			tagContext.WriteTag(false);
			htmlWriter.WriteAttribute("blockedImageSrc", value);
			AttachmentWebBeaconFilterCallback.WriteAllAttributesExcept(tagContext, HtmlAttributeId.Src);
		}

		private void GetLinkSource(HtmlTagContext tagContext, out string srcValue)
		{
			srcValue = null;
			foreach (HtmlTagContextAttribute htmlTagContextAttribute in tagContext.Attributes)
			{
				if (htmlTagContextAttribute.Id == HtmlAttributeId.Src)
				{
					srcValue = htmlTagContextAttribute.Value;
					break;
				}
			}
		}
	}
}
