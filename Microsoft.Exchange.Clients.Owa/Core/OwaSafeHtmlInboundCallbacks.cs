using System;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class OwaSafeHtmlInboundCallbacks : OwaSafeHtmlCallbackBase
	{
		public OwaSafeHtmlInboundCallbacks()
		{
		}

		public OwaSafeHtmlInboundCallbacks(Item item, UserContext userContext) : base(Utilities.GetAttachmentCollection(item, false, userContext), item.Body)
		{
			base.RemoveUnlinkedAttachments = true;
			this.item = item;
		}

		public override void ProcessTag(HtmlTagContext context, HtmlWriter writer)
		{
			context.WriteTag(false);
			foreach (HtmlTagContextAttribute htmlTagContextAttribute in context.Attributes)
			{
				if (OwaSafeHtmlCallbackBase.IsUrlTag(context.TagId, htmlTagContextAttribute))
				{
					OwaSafeHtmlInboundCallbacks.RestoreUrl(htmlTagContextAttribute, writer);
				}
				else if (OwaSafeHtmlCallbackBase.IsImageTag(context.TagId, htmlTagContextAttribute) || OwaSafeHtmlCallbackBase.IsBackgroundAttribute(htmlTagContextAttribute))
				{
					this.RestoreInlineAttachment(htmlTagContextAttribute, context, writer);
				}
				else
				{
					htmlTagContextAttribute.Write();
				}
			}
		}

		private static void RestoreUrl(HtmlTagContextAttribute filterAttribute, HtmlWriter writer)
		{
			string value = filterAttribute.Value;
			if (!string.IsNullOrEmpty(value))
			{
				int num = value.IndexOf("redir.aspx?", StringComparison.Ordinal);
				string text = null;
				if (num != -1)
				{
					string text2 = value.Substring(num + "redir.aspx?".Length);
					if (text2.StartsWith("URL=", StringComparison.Ordinal))
					{
						text = text2.Substring("URL=".Length);
					}
					else
					{
						bool legacyFormat = true;
						string text3 = null;
						if (text2.StartsWith("SURL", StringComparison.Ordinal))
						{
							text3 = text2.Substring("SURL".Length);
						}
						else if (text2.StartsWith("REF", StringComparison.Ordinal))
						{
							text3 = text2.Substring("REF".Length);
							legacyFormat = false;
						}
						if (text3 != null)
						{
							text = CryptoMessage.ExtractUrl(text3, legacyFormat);
						}
					}
				}
				else
				{
					num = OwaSafeHtmlCallbackBase.JSLocalLink.Length + OwaSafeHtmlCallbackBase.JSMethodPrefix.Length;
					if (value.StartsWith(OwaSafeHtmlCallbackBase.JSLocalLink + OwaSafeHtmlCallbackBase.JSMethodPrefix, StringComparison.Ordinal) && value.EndsWith(OwaSafeHtmlCallbackBase.JSMethodSuffix, StringComparison.Ordinal))
					{
						text = OwaSafeHtmlCallbackBase.LocalUrlPrefix + value.Substring(num, value.Length - OwaSafeHtmlCallbackBase.JSMethodSuffix.Length - num);
					}
				}
				if (text != null)
				{
					writer.WriteAttribute(filterAttribute.Id, HttpUtility.UrlDecode(text));
					return;
				}
				filterAttribute.Write();
			}
		}

		private void RestoreInlineAttachment(HtmlTagContextAttribute filterAttribute, HtmlTagContext context, HtmlWriter writer)
		{
			string text = filterAttribute.Value;
			string value = filterAttribute.Value;
			text = HttpUtility.UrlDecode(text);
			if (!string.IsNullOrEmpty(text))
			{
				int num = value.IndexOf(OwaSafeHtmlCallbackBase.AttachmentBaseUrl, StringComparison.Ordinal);
				if (num != -1 && this.Item != null && this.Item.Id != null)
				{
					if (AttachmentUtility.VerifyInlineAttachmentUrlValidity(value, this.item))
					{
						int contentIdIndex;
						string contentId = AttachmentUtility.ParseInlineAttachmentContentId(text, out contentIdIndex);
						string attachmentIdString = AttachmentUtility.ParseInlineAttachmentIdString(text, contentIdIndex);
						AttachmentLink attachmentLink = AttachmentUtility.GetAttachmentLink(attachmentIdString, contentId, this.item, this);
						if (attachmentLink != null)
						{
							filterAttribute.WriteName();
							writer.WriteAttributeValue(this.inlineHTMLAttachmentScheme + AttachmentUtility.GetOrGenerateAttachContentId(attachmentLink));
							return;
						}
					}
				}
				else
				{
					if (text.IndexOf(OwaSafeHtmlCallbackBase.blankImageFileName, StringComparison.Ordinal) != -1)
					{
						filterAttribute.WriteName();
						writer.WriteAttributeValue(OwaSafeHtmlCallbackBase.DoubleBlank);
						return;
					}
					filterAttribute.Write();
				}
			}
		}

		public bool AttachmentNeedsSave()
		{
			return base.NeedsSave();
		}

		private Item Item
		{
			get
			{
				return this.item;
			}
		}

		private Item item;
	}
}
