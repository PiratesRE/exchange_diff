using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class HtmlUpdateBodyCallback : HtmlCallbackBase
	{
		public HtmlUpdateBodyCallback(Item item) : base(HtmlUpdateBodyCallback.GetAttachmentCollection(item), item.Body)
		{
			this.item = item;
			base.RemoveUnlinkedAttachments = true;
			base.ClearInlineOnUnmarkedAttachments = true;
		}

		public override CoreAttachmentCollection AttachmentCollection
		{
			get
			{
				return this.item.AttachmentCollection.CoreAttachmentCollection;
			}
		}

		public override void ProcessTag(HtmlTagContext htmlTagContext, HtmlWriter htmlWriter)
		{
			HtmlTagId tagId = htmlTagContext.TagId;
			if (tagId == HtmlTagId.Img)
			{
				this.ProcessImgTag(htmlTagContext, htmlWriter);
				return;
			}
			htmlTagContext.WriteTag(true);
		}

		private static AttachmentCollection GetAttachmentCollection(Item item)
		{
			return item.AttachmentCollection;
		}

		private void ProcessImgTag(HtmlTagContext htmlTagContext, HtmlWriter htmlWriter)
		{
			string text = null;
			string text2 = null;
			htmlTagContext.WriteTag(false);
			foreach (HtmlTagContextAttribute htmlTagContextAttribute in htmlTagContext.Attributes)
			{
				if (htmlTagContextAttribute.Id == HtmlAttributeId.Src)
				{
					text = htmlTagContextAttribute.Value;
				}
				else if (htmlTagContextAttribute.Name.Equals("originalSrc", StringComparison.OrdinalIgnoreCase) || htmlTagContextAttribute.Name.Equals("blockedImageSrc", StringComparison.OrdinalIgnoreCase))
				{
					text2 = htmlTagContextAttribute.Value;
				}
				else
				{
					htmlTagContextAttribute.Write();
				}
			}
			if (text2 != null)
			{
				text = text2;
			}
			if (text != null)
			{
				this.MarkInlineAttachment(text);
				htmlWriter.WriteAttribute(HtmlAttributeId.Src, text);
			}
		}

		private void MarkInlineAttachment(string srcValue)
		{
			string contentId = this.GetContentId(srcValue);
			if (contentId == null)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>((long)this.GetHashCode(), "HtmlUpdateBodyCallback.MarkInlineAttachment. Content ID is empty for {0}", srcValue);
				return;
			}
			AttachmentId attachmentId = this.FindAttachmentId(contentId);
			if (attachmentId == null)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>((long)this.GetHashCode(), "HtmlUpdateBodyCallback.MarkInlineAttachment. Attachment ID not found for {0}", srcValue);
				return;
			}
			AttachmentLink attachmentLink = base.FindAttachmentByIdOrContentId(attachmentId, contentId);
			if (attachmentLink == null)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>((long)this.GetHashCode(), "HtmlUpdateBodyCallback.MarkInlineAttachment. No attachment link found for {0}", srcValue);
				return;
			}
			attachmentLink.MarkInline(true);
		}

		private string GetContentId(string srcValue)
		{
			string result = null;
			if (srcValue.StartsWith("cid:", StringComparison.OrdinalIgnoreCase) && srcValue.Length > "cid:".Length)
			{
				result = srcValue.Substring("cid:".Length);
			}
			return result;
		}

		private AttachmentId FindAttachmentId(string contentId)
		{
			AttachmentId result = null;
			foreach (AttachmentHandle handle in this.item.AttachmentCollection)
			{
				using (Attachment attachment = this.item.AttachmentCollection.Open(handle))
				{
					if (attachment.ContentId.Equals(contentId, StringComparison.OrdinalIgnoreCase))
					{
						result = attachment.Id;
					}
				}
			}
			return result;
		}

		private Item item;
	}
}
