using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class SafeHtmlCallback : HtmlCallbackBase
	{
		public SafeHtmlCallback(Item item) : base(item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			this.itemId = ((item.Id != null) ? item.Id.ObjectId : StoreObjectId.DummyId);
			base.InitializeAttachmentLinks(null);
		}

		public static bool HasBlockedImages
		{
			get
			{
				return false;
			}
		}

		public bool HasBlockedInlineAttachments
		{
			get
			{
				return this.hasBlockedInlineAttachments;
			}
		}

		public override void ProcessTag(HtmlTagContext context, HtmlWriter writer)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			string text = null;
			string text2 = null;
			string text3 = null;
			if (context.TagId == HtmlTagId.Link)
			{
				bool writeTag = SafeHtmlCallback.WriteTagWithMicroData(context);
				SafeHtmlCallback.ProcessMicrodataTag(writeTag, context, SafeHtmlCallback.linkTagAttributes);
				return;
			}
			if (context.TagId == HtmlTagId.Head)
			{
				context.WriteTag(true);
				return;
			}
			if (context.TagId == HtmlTagId.Meta)
			{
				bool writeTag2 = SafeHtmlCallback.WriteTagWithMicroData(context);
				SafeHtmlCallback.ProcessMicrodataTag(writeTag2, context, SafeHtmlCallback.metaTagAttributes);
				return;
			}
			if (context.TagId == HtmlTagId.Base)
			{
				foreach (HtmlTagContextAttribute attribute in context.Attributes)
				{
					if (SafeHtmlCallback.IsBaseTag(context.TagId, attribute))
					{
						string value = attribute.Value;
						if (!Uri.TryCreate(value, UriKind.Absolute, out this.baseRef))
						{
							this.baseRef = null;
							break;
						}
						break;
					}
				}
			}
			context.WriteTag();
			bool flag5 = false;
			foreach (HtmlTagContextAttribute htmlTagContextAttribute in context.Attributes)
			{
				if (context.TagId == HtmlTagId.Form || context.TagId == HtmlTagId.Input)
				{
					if (htmlTagContextAttribute.Id != HtmlAttributeId.Src && htmlTagContextAttribute.Id != HtmlAttributeId.Action && htmlTagContextAttribute.Id != HtmlAttributeId.Method && htmlTagContextAttribute.Id != HtmlAttributeId.Target)
					{
						htmlTagContextAttribute.Write();
					}
				}
				else if (htmlTagContextAttribute.Id != HtmlAttributeId.UseMap)
				{
					if (SafeHtmlCallback.IsUrlTag(context.TagId, htmlTagContextAttribute))
					{
						if (!flag)
						{
							this.ProcessHtmlUrlTag(htmlTagContextAttribute, writer);
							flag = true;
						}
					}
					else if (SafeHtmlCallback.IsImageTag(context.TagId, htmlTagContextAttribute))
					{
						if ((htmlTagContextAttribute.Id == HtmlAttributeId.Src && !flag2) || (htmlTagContextAttribute.Id == HtmlAttributeId.DynSrc && !flag3) || (htmlTagContextAttribute.Id == HtmlAttributeId.LowSrc && !flag4))
						{
							this.ProcessImageTag(htmlTagContextAttribute, context, writer);
							if (htmlTagContextAttribute.Value == "rtfimage://")
							{
								flag5 = true;
							}
							if (htmlTagContextAttribute.Id == HtmlAttributeId.Src)
							{
								flag2 = true;
							}
							else if (htmlTagContextAttribute.Id == HtmlAttributeId.DynSrc)
							{
								flag3 = true;
							}
							else if (htmlTagContextAttribute.Id == HtmlAttributeId.LowSrc)
							{
								flag4 = true;
							}
						}
					}
					else if (SafeHtmlCallback.IsBackgroundAttribute(htmlTagContextAttribute))
					{
						this.ProcessImageTag(htmlTagContextAttribute, context, writer);
					}
					else if (!SafeHtmlCallback.IsTargetTagInAnchor(context.TagId, htmlTagContextAttribute))
					{
						if (SafeHtmlCallback.IsSanitizingAttribute(htmlTagContextAttribute))
						{
							if (htmlTagContextAttribute.Id == HtmlAttributeId.Border)
							{
								text = htmlTagContextAttribute.Value;
							}
							else if (htmlTagContextAttribute.Id == HtmlAttributeId.Height)
							{
								text2 = htmlTagContextAttribute.Value;
							}
							else if (htmlTagContextAttribute.Id == HtmlAttributeId.Width)
							{
								text3 = htmlTagContextAttribute.Value;
							}
						}
						else
						{
							htmlTagContextAttribute.Write();
						}
					}
				}
			}
			if (!string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(text3) && this.hasBlockedImagesInCurrentPass)
			{
				SafeHtmlCallback.SanitizeImage(writer, text, text2, text3);
			}
			else if (!this.hasBlockedImagesInCurrentPass)
			{
				if (flag5)
				{
					writer.WriteAttribute(HtmlAttributeId.Height, "0");
					writer.WriteAttribute(HtmlAttributeId.Width, "0");
				}
				else
				{
					if (!string.IsNullOrEmpty(text2))
					{
						writer.WriteAttribute(HtmlAttributeId.Height, text2);
					}
					if (!string.IsNullOrEmpty(text3))
					{
						writer.WriteAttribute(HtmlAttributeId.Width, text3);
					}
					if (!string.IsNullOrEmpty(text))
					{
						writer.WriteAttribute(HtmlAttributeId.Border, text);
					}
				}
			}
			if (this.hasFoundRedirUrlInCurrentPass)
			{
				writer.WriteAttribute(HtmlAttributeId.Target, "_BLANK");
			}
			this.hasBlockedImagesInCurrentPass = false;
			this.hasFoundRedirUrlInCurrentPass = false;
		}

		protected static bool WriteTagWithMicroData(HtmlTagContext context)
		{
			foreach (HtmlTagContextAttribute htmlTagContextAttribute in context.Attributes)
			{
				if (htmlTagContextAttribute.Id == HtmlAttributeId.ItemProp || htmlTagContextAttribute.Id == HtmlAttributeId.ItemRef || htmlTagContextAttribute.Id == HtmlAttributeId.ItemScope || htmlTagContextAttribute.Id == HtmlAttributeId.ItemId || htmlTagContextAttribute.Id == HtmlAttributeId.ItemType)
				{
					context.WriteTag();
					return true;
				}
			}
			return false;
		}

		protected static void ProcessMicrodataTag(bool writeTag, HtmlTagContext context, List<HtmlAttributeId> safeAttributes)
		{
			if (!writeTag)
			{
				context.DeleteTag();
				return;
			}
			foreach (HtmlTagContextAttribute htmlTagContextAttribute in context.Attributes)
			{
				if (safeAttributes.Contains(htmlTagContextAttribute.Id))
				{
					bool flag = true;
					if (htmlTagContextAttribute.Id == HtmlAttributeId.Href && !SafeHtmlCallback.IsSafeUrl(htmlTagContextAttribute.Value, htmlTagContextAttribute.Id))
					{
						flag = false;
					}
					if (flag)
					{
						htmlTagContextAttribute.Write();
					}
				}
			}
		}

		protected static bool IsBackgroundAttribute(HtmlTagContextAttribute attribute)
		{
			return attribute.Id == HtmlAttributeId.Background;
		}

		protected static bool IsBaseTag(HtmlTagId tagId, HtmlTagContextAttribute attribute)
		{
			return tagId == HtmlTagId.Base && attribute.Id == HtmlAttributeId.Href;
		}

		protected static bool IsImageTag(HtmlTagId tagId, HtmlTagContextAttribute attribute)
		{
			return tagId == HtmlTagId.Img && (attribute.Id == HtmlAttributeId.Src || attribute.Id == HtmlAttributeId.DynSrc || attribute.Id == HtmlAttributeId.LowSrc);
		}

		protected static bool IsSanitizingAttribute(HtmlTagContextAttribute attribute)
		{
			return attribute.Id == HtmlAttributeId.Border || attribute.Id == HtmlAttributeId.Width || attribute.Id == HtmlAttributeId.Height;
		}

		protected static bool IsTargetTagInAnchor(HtmlTagId tagId, HtmlTagContextAttribute attr)
		{
			return (tagId == HtmlTagId.A || tagId == HtmlTagId.Area) && attr.Id == HtmlAttributeId.Target;
		}

		protected static bool IsUrlTag(HtmlTagId tagId, HtmlTagContextAttribute attribute)
		{
			return (tagId == HtmlTagId.A || tagId == HtmlTagId.Area) && attribute.Id == HtmlAttributeId.Href;
		}

		protected static SafeHtmlCallback.TypeOfUrl GetTypeOfUrl(string urlString, HtmlAttributeId htmlAttr)
		{
			if (string.IsNullOrEmpty(urlString))
			{
				return SafeHtmlCallback.TypeOfUrl.Unknown;
			}
			if (urlString.StartsWith("#", StringComparison.Ordinal))
			{
				return SafeHtmlCallback.TypeOfUrl.Local;
			}
			Uri uri;
			if (!Uri.TryCreate(urlString, UriKind.Absolute, out uri))
			{
				return SafeHtmlCallback.TypeOfUrl.Unknown;
			}
			string scheme = uri.Scheme;
			if (string.IsNullOrEmpty(scheme))
			{
				return SafeHtmlCallback.TypeOfUrl.Unknown;
			}
			if (string.Equals(scheme, "file", StringComparison.OrdinalIgnoreCase) && htmlAttr == HtmlAttributeId.Href)
			{
				return SafeHtmlCallback.TypeOfUrl.Trusted;
			}
			for (int i = 0; i < SafeHtmlCallback.redirProtocols.Length; i++)
			{
				if (string.Equals(scheme, SafeHtmlCallback.redirProtocols[i], StringComparison.OrdinalIgnoreCase))
				{
					return SafeHtmlCallback.TypeOfUrl.Redirection;
				}
			}
			return SafeHtmlCallback.TypeOfUrl.Trusted;
		}

		protected static bool IsSafeUrl(string urlString, HtmlAttributeId htmlAttr)
		{
			SafeHtmlCallback.TypeOfUrl typeOfUrl = SafeHtmlCallback.GetTypeOfUrl(urlString, htmlAttr);
			return typeOfUrl != SafeHtmlCallback.TypeOfUrl.Unknown;
		}

		protected static void SanitizeImage(HtmlWriter writer, string border, string height, string width)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			bool flag = true;
			if (border != null)
			{
				flag = int.TryParse(border, NumberStyles.Number, CultureInfo.InvariantCulture, out num);
			}
			if (flag)
			{
				flag = int.TryParse(height, NumberStyles.Number, CultureInfo.InvariantCulture, out num2);
			}
			if (flag)
			{
				flag = int.TryParse(width, NumberStyles.Number, CultureInfo.InvariantCulture, out num3);
			}
			if (num < 0 || num2 < 0 || num3 < 0)
			{
				return;
			}
			if (flag)
			{
				if (num > 1)
				{
					num3 = num3 + num - 1;
					num2 = num2 + num - 1;
				}
				writer.WriteAttribute(HtmlAttributeId.Border, "1");
				if (num3 >= 0)
				{
					writer.WriteAttribute(HtmlAttributeId.Width, num3.ToString(CultureInfo.InvariantCulture));
				}
				if (num2 >= 0)
				{
					writer.WriteAttribute(HtmlAttributeId.Height, num2.ToString(CultureInfo.InvariantCulture));
					return;
				}
			}
			else
			{
				if (border != null)
				{
					writer.WriteAttribute(HtmlAttributeId.Border, border);
				}
				writer.WriteAttribute(HtmlAttributeId.Width, width);
				writer.WriteAttribute(HtmlAttributeId.Height, height);
			}
		}

		protected AttachmentLink IsInlineReference(string bodyRef)
		{
			if (this.baseRef != null)
			{
				return base.FindAttachmentByBodyReference(bodyRef, this.baseRef);
			}
			return base.FindAttachmentByBodyReference(bodyRef);
		}

		protected AttachmentPolicy.Level GetAttachmentLevel(AttachmentLink link)
		{
			if (link.AttachmentType == AttachmentType.Stream || link.AttachmentType == AttachmentType.Ole)
			{
				this.isembeddedItem = true;
				return AttachmentPolicy.Level.Allow;
			}
			return AttachmentPolicy.Level.Block;
		}

		protected void OutputInlineReference(HtmlTagContextAttribute filterAttribute, AttachmentLink link, HtmlWriter writer)
		{
			AttachmentPolicy.Level attachmentLevel = this.GetAttachmentLevel(link);
			if (AttachmentPolicy.Level.Allow != attachmentLevel)
			{
				this.hasBlockedImagesInCurrentPass = true;
				this.hasBlockedInlineAttachments = true;
				writer.WriteAttribute(filterAttribute.Id, "  ");
				return;
			}
			if (!this.isembeddedItem)
			{
				StringBuilder stringBuilder = new StringBuilder("/Microsoft-Server-ActiveSync?Cmd=GetAttachment&AttachmentName=");
				int num = 0;
				foreach (AttachmentLink attachmentLink in base.AttachmentLinks)
				{
					if (link.AttachmentId == attachmentLink.AttachmentId)
					{
						break;
					}
					num++;
				}
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
				{
					this.itemId.ToBase64String(),
					num
				});
				writer.WriteAttribute(filterAttribute.Id, stringBuilder.ToString());
				return;
			}
			filterAttribute.WriteName();
			writer.WriteAttributeValue("cid:" + this.GetOrGenerateAttachContentId(link));
		}

		protected void ProcessHtmlUrlTag(HtmlTagContextAttribute filterAttribute, HtmlWriter writer)
		{
			string value = filterAttribute.Value;
			AttachmentLink attachmentLink = this.IsInlineReference(value);
			if (attachmentLink != null)
			{
				this.OutputInlineReference(filterAttribute, attachmentLink, writer);
				return;
			}
			SafeHtmlCallback.TypeOfUrl typeOfUrl = SafeHtmlCallback.GetTypeOfUrl(filterAttribute.Value, filterAttribute.Id);
			if (typeOfUrl == SafeHtmlCallback.TypeOfUrl.Redirection)
			{
				filterAttribute.Write();
				this.hasFoundRedirUrlInCurrentPass = true;
				return;
			}
			if (typeOfUrl == SafeHtmlCallback.TypeOfUrl.Trusted || typeOfUrl == SafeHtmlCallback.TypeOfUrl.Local)
			{
				filterAttribute.Write();
				return;
			}
			if (typeOfUrl == SafeHtmlCallback.TypeOfUrl.Unknown)
			{
				writer.WriteAttribute(filterAttribute.Id, "  ");
			}
		}

		protected void ProcessImageTag(HtmlTagContextAttribute filterAttribute, HtmlTagContext context, HtmlWriter writer)
		{
			AirSyncDiagnostics.Assert(context != null);
			string value = filterAttribute.Value;
			AttachmentLink attachmentLink = this.IsInlineReference(value);
			if (attachmentLink != null)
			{
				this.OutputInlineReference(filterAttribute, attachmentLink, writer);
				return;
			}
			if (SafeHtmlCallback.IsSafeUrl(filterAttribute.Value, filterAttribute.Id))
			{
				filterAttribute.Write();
			}
		}

		private string GetOrGenerateAttachContentId(AttachmentLink attachmentLink)
		{
			StoreObjectId key = this.itemId;
			if (this.existingContentIds == null && !Command.CurrentCommand.InlineAttachmentContentIdLookUp.TryGetValue(key, out this.existingContentIds))
			{
				this.existingContentIds = new Dictionary<AttachmentId, string>(1);
				Command.CurrentCommand.InlineAttachmentContentIdLookUp[key] = this.existingContentIds;
			}
			string text;
			if (this.existingContentIds.TryGetValue(attachmentLink.AttachmentId, out text) && !string.IsNullOrEmpty(text))
			{
				attachmentLink.ContentId = text;
			}
			else
			{
				if (string.IsNullOrEmpty(attachmentLink.ContentId))
				{
					attachmentLink.ContentId = Guid.NewGuid().ToString();
				}
				this.existingContentIds[attachmentLink.AttachmentId] = attachmentLink.ContentId;
			}
			attachmentLink.MarkInline(true);
			return attachmentLink.ContentId;
		}

		private const string AttachmentBaseUrl = "/Microsoft-Server-ActiveSync?Cmd=GetAttachment&AttachmentName=";

		private const string DoubleBlank = "  ";

		private const string LocalUrlPrefix = "#";

		private const string TargetValue = "_BLANK";

		private static readonly string[] redirProtocols = new string[]
		{
			"http",
			"https",
			"mhtml"
		};

		private static List<HtmlAttributeId> metaTagAttributes = new List<HtmlAttributeId>
		{
			HtmlAttributeId.ItemProp,
			HtmlAttributeId.Content
		};

		private static List<HtmlAttributeId> linkTagAttributes = new List<HtmlAttributeId>
		{
			HtmlAttributeId.ItemProp,
			HtmlAttributeId.Href
		};

		private Uri baseRef;

		private bool hasBlockedImagesInCurrentPass;

		private bool hasBlockedInlineAttachments;

		private bool hasFoundRedirUrlInCurrentPass;

		private bool isembeddedItem;

		private Dictionary<AttachmentId, string> existingContentIds;

		private StoreObjectId itemId;

		protected enum TypeOfUrl
		{
			Local = 1,
			Trusted,
			Redirection,
			Unknown
		}
	}
}
