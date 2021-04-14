using System;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DefaultHtmlCallbacks : HtmlCallbackBase
	{
		public DefaultHtmlCallbacks(IItem item, bool itemReadOnly) : this(item.CoreItem, itemReadOnly)
		{
		}

		internal DefaultHtmlCallbacks(ICoreItem coreItem, bool itemReadOnly) : base(coreItem)
		{
			this.readOnly = itemReadOnly;
			this.clearEmptyLinks = false;
			this.removeLinksToNonImageAttachments = false;
		}

		internal DefaultHtmlCallbacks(CoreAttachmentCollection collection, Body itemBody, bool itemReadOnly) : base(collection, itemBody)
		{
			this.readOnly = itemReadOnly;
			this.clearEmptyLinks = false;
			this.removeLinksToNonImageAttachments = false;
		}

		internal bool ClearingEmptyLinks
		{
			get
			{
				return this.clearEmptyLinks;
			}
			set
			{
				this.clearEmptyLinks = value;
			}
		}

		internal bool RemoveLinksToNonImageAttachments
		{
			get
			{
				return this.removeLinksToNonImageAttachments;
			}
			set
			{
				this.removeLinksToNonImageAttachments = value;
			}
		}

		public override void ProcessTag(HtmlTagContext tagContext, HtmlWriter htmlWriter)
		{
			if (tagContext.TagId != HtmlTagId.Img)
			{
				tagContext.WriteTag(true);
				return;
			}
			AttachmentLink attachmentLink = null;
			string text = null;
			foreach (HtmlTagContextAttribute htmlTagContextAttribute in tagContext.Attributes)
			{
				if (htmlTagContextAttribute.Id == HtmlAttributeId.Src)
				{
					text = htmlTagContextAttribute.Value;
					break;
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				attachmentLink = base.FindAttachmentByBodyReference(text);
			}
			if (attachmentLink == null)
			{
				if (!this.clearEmptyLinks || !DefaultHtmlCallbacks.IsEmptyLink(text))
				{
					tagContext.WriteTag(true);
				}
				return;
			}
			string text2;
			string text3;
			if (attachmentLink.AttachmentType == AttachmentType.Ole)
			{
				text2 = "image/jpeg";
				text3 = "jpg";
				attachmentLink.ConvertToImage();
			}
			else
			{
				text2 = attachmentLink.ContentType;
				text3 = attachmentLink.FileExtension;
			}
			bool flag = text.StartsWith("cid:Microsoft-Infopath-", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(text3) && !string.IsNullOrEmpty(text2) && text2.Equals("application/octet-stream");
			if (DefaultHtmlCallbacks.IsInlineImage(text2, text3) || flag)
			{
				tagContext.WriteTag(false);
				foreach (HtmlTagContextAttribute htmlTagContextAttribute2 in tagContext.Attributes)
				{
					if (htmlTagContextAttribute2.Id == HtmlAttributeId.Src)
					{
						string value = "cid:" + this.GetOrGenerateAttachContentId(attachmentLink);
						htmlWriter.WriteAttribute(HtmlAttributeId.Src, value);
					}
					else
					{
						htmlTagContextAttribute2.Write();
					}
				}
				attachmentLink.MarkInline(true);
				return;
			}
			if (!this.RemoveLinksToNonImageAttachments)
			{
				string text4 = attachmentLink.Filename;
				if (text4 == null)
				{
					text4 = ServerStrings.DefaultHtmlAttachmentHrefText;
				}
				htmlWriter.WriteStartTag(HtmlTagId.A);
				htmlWriter.WriteAttribute(HtmlAttributeId.Href, "cid:" + this.GetOrGenerateAttachContentId(attachmentLink));
				htmlWriter.WriteText(text4);
				htmlWriter.WriteEndTag(HtmlTagId.A);
				attachmentLink.MarkInline(false);
			}
		}

		public override bool SaveChanges()
		{
			return !this.readOnly && base.SaveChanges();
		}

		internal static bool IsEmptyLink(string src)
		{
			return string.Compare(src, "objattph://", StringComparison.CurrentCultureIgnoreCase) == 0 || string.Compare(src, "rtfimage://", StringComparison.CurrentCultureIgnoreCase) == 0 || string.Compare(src, "cid:", StringComparison.CurrentCultureIgnoreCase) == 0;
		}

		public static bool IsInlineImage(string contentType, string fileExtension)
		{
			if (contentType != null && DefaultHtmlCallbacks.IsInlineImage(contentType))
			{
				return true;
			}
			if (fileExtension != null)
			{
				contentType = Attachment.CalculateContentType(fileExtension);
				return DefaultHtmlCallbacks.IsInlineImage(contentType);
			}
			return false;
		}

		private static bool IsInlineImage(string contentType)
		{
			return contentType != null && (contentType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase) || contentType.Equals("image/pjpeg", StringComparison.OrdinalIgnoreCase) || contentType.Equals("image/gif", StringComparison.OrdinalIgnoreCase) || contentType.Equals("image/bmp", StringComparison.OrdinalIgnoreCase) || contentType.Equals("image/png", StringComparison.OrdinalIgnoreCase) || contentType.Equals("image/x-png", StringComparison.OrdinalIgnoreCase));
		}

		private string GetOrGenerateAttachContentId(AttachmentLink link)
		{
			string text = link.ContentId;
			if (string.IsNullOrEmpty(text))
			{
				text = AttachmentLink.CreateContentId(this.AttachmentCollection.ContainerItem, link.AttachmentId, this.contentIdDomain);
				link.ContentId = text;
			}
			return text;
		}

		public void SetContentIdDomain(string domain)
		{
			Util.ThrowOnNullOrEmptyArgument(domain, "domain");
			this.contentIdDomain = domain;
		}

		private bool readOnly;

		private bool clearEmptyLinks;

		private bool removeLinksToNonImageAttachments;

		private string contentIdDomain;
	}
}
