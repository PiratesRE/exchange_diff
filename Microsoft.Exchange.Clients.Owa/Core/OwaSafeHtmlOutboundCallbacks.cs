using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class OwaSafeHtmlOutboundCallbacks : OwaSafeHtmlCallbackBase
	{
		public OwaSafeHtmlOutboundCallbacks(OwaContext owaContext, bool isEditableContent)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			this.owaContext = owaContext;
			this.allowForms = isEditableContent;
		}

		public OwaSafeHtmlOutboundCallbacks(OwaContext owaContext, bool openMailtoInNewWindow, bool isEditableContent)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			this.owaContext = owaContext;
			this.openMailtoInNewWindow = openMailtoInNewWindow;
			this.allowForms = isEditableContent;
		}

		public OwaSafeHtmlOutboundCallbacks(Item item, bool userLogon, bool isJunkOrPhishing, OwaContext owaContext, bool isEditableContent) : this(item, userLogon, false, null, isJunkOrPhishing, owaContext, isEditableContent)
		{
		}

		public OwaSafeHtmlOutboundCallbacks(Item item, bool userLogon, bool isEmbedded, string itemUrl, bool isJunkOrPhishing, OwaContext owaContext, bool isEditableContent) : base(OwaSafeHtmlOutboundCallbacks.GetAttachmentCollection(item, owaContext.UserContext), item.Body)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			this.owaContext = owaContext;
			this.isLoggedOnFromPublicComputer = userLogon;
			this.isEmbeddedItem = isEmbedded;
			this.embeddedItemUrl = itemUrl;
			this.isJunkOrPhishing = isJunkOrPhishing;
			this.isOutputFragment = owaContext.UserContext.IsBasicExperience;
			this.allowForms = isEditableContent;
			if (item.Id != null)
			{
				this.itemId = item.Id.ObjectId;
			}
			this.parentId = (item.TryGetProperty(StoreObjectSchema.ParentItemId) as StoreObjectId);
			this.objectClass = item.ClassName;
			if (base.ItemBody != null)
			{
				this.charSet = base.ItemBody.Charset;
			}
			if (Utilities.IsOtherMailbox(item) || Utilities.IsInArchiveMailbox(item))
			{
				this.legacyDN = Utilities.GetMailboxSessionLegacyDN(item);
			}
			else
			{
				this.legacyDN = owaContext.UserContext.MailboxOwnerLegacyDN;
			}
			if (Utilities.IsOtherMailbox(item))
			{
				this.owaStoreObjectIdType = OwaStoreObjectIdType.OtherUserMailboxObject;
			}
			else if (Utilities.IsInArchiveMailbox(item))
			{
				this.owaStoreObjectIdType = OwaStoreObjectIdType.ArchiveMailboxObject;
			}
			else if (Utilities.IsPublic(item))
			{
				this.owaStoreObjectIdType = OwaStoreObjectIdType.PublicStoreItem;
			}
			this.bodyFormat = base.ItemBody.Format;
			string queryStringParameter = Utilities.GetQueryStringParameter(owaContext.HttpContext.Request, "t", false);
			this.isConversations = (!string.IsNullOrEmpty(queryStringParameter) && string.Equals("IPM.Conversation", queryStringParameter, StringComparison.Ordinal));
			this.isConversationsOrUnknownType = (this.isConversations || queryStringParameter == null);
		}

		private static AttachmentCollection GetAttachmentCollection(Item item, UserContext userContext)
		{
			AttachmentCollection attachmentCollection = null;
			if (Utilities.IsSMimeButNotSecureSign(item))
			{
				Item item2 = Utilities.OpenSMimeContent(item);
				if (item2 != null)
				{
					attachmentCollection = item2.AttachmentCollection;
				}
			}
			else if (userContext.IsIrmEnabled && !userContext.IsBasicExperience && Utilities.IsIrmDecrypted(item))
			{
				attachmentCollection = ((RightsManagedMessageItem)item).ProtectedAttachmentCollection;
			}
			if (attachmentCollection != null)
			{
				return attachmentCollection;
			}
			return item.AttachmentCollection;
		}

		public virtual bool HasBlockedForms
		{
			get
			{
				return this.hasBlockedForms;
			}
		}

		public virtual bool HasInlineImages
		{
			get
			{
				return this.hasInlineImages;
			}
		}

		public override void ProcessTag(HtmlTagContext context, HtmlWriter writer)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			string text = null;
			string text2 = null;
			string text3 = null;
			AttachmentLink attachmentLink = null;
			if (context.TagId == HtmlTagId.Link)
			{
				context.DeleteTag();
				return;
			}
			if (context.TagId == HtmlTagId.Head)
			{
				if (this.owaContext.UserContext.IsBasicExperience || this.bodyFormat != BodyFormat.TextPlain)
				{
					context.WriteTag(true);
					return;
				}
				if (!context.IsEndTag)
				{
					context.WriteTag(true);
					context.InvokeCallbackForEndTag();
					return;
				}
				writer.WriteStartTag(HtmlTagId.Style);
				writer.WriteText("div.PlainText ");
				if (ObjectClass.IsSmsMessage(this.objectClass))
				{
					writer.WriteText(OwaPlainTextStyle.GetStyleFromUserOption(this.owaContext.UserContext.UserOptions));
				}
				else
				{
					writer.WriteText(OwaPlainTextStyle.GetStyleFromCharset(this.charSet));
				}
				writer.WriteEndTag(HtmlTagId.Style);
				context.WriteTag(true);
				return;
			}
			else
			{
				if (this.isOutputFragment && context.TagId == HtmlTagId.Form)
				{
					context.DeleteTag();
					return;
				}
				if (context.TagId == HtmlTagId.Form || (!this.isOutputFragment && OwaSafeHtmlOutboundCallbacks.IsFormElementTag(context.TagId)))
				{
					this.ProcessUnfragFormTagContext(context, writer);
					return;
				}
				if (context.TagId == HtmlTagId.Base)
				{
					foreach (HtmlTagContextAttribute attribute in context.Attributes)
					{
						if (OwaSafeHtmlCallbackBase.IsBaseTag(context.TagId, attribute))
						{
							string value = attribute.Value;
							this.baseRef = Utilities.TryParseUri(value);
							break;
						}
					}
					return;
				}
				foreach (HtmlTagContextAttribute filterAttribute in context.Attributes)
				{
					if (filterAttribute.Id == HtmlAttributeId.Src || filterAttribute.Id == HtmlAttributeId.Href)
					{
						if (context.TagId == HtmlTagId.Img && string.IsNullOrEmpty(filterAttribute.Value))
						{
							return;
						}
						if (string.CompareOrdinal(filterAttribute.Value, this.inlineRTFattachmentScheme) == 0 || filterAttribute.Value.StartsWith(this.inlineHTMLAttachmentScheme, StringComparison.OrdinalIgnoreCase))
						{
							attachmentLink = this.IsInlineImage(filterAttribute);
							if (attachmentLink == null)
							{
								return;
							}
							if (context.TagId != HtmlTagId.Img)
							{
								writer.WriteEmptyElementTag(HtmlTagId.Img);
								this.OutputInlineReference(filterAttribute, context, attachmentLink, writer);
								context.DeleteTag(false);
								context.DeleteInnerContent();
								return;
							}
							flag2 = true;
							break;
						}
						else
						{
							if (string.CompareOrdinal(filterAttribute.Value, this.embeddedRTFImage) == 0)
							{
								this.hasRtfEmbeddedImages = true;
								break;
							}
							break;
						}
					}
				}
				context.WriteTag();
				foreach (HtmlTagContextAttribute htmlTagContextAttribute in context.Attributes)
				{
					if (!this.isOutputFragment || htmlTagContextAttribute.Id != HtmlAttributeId.Name || !OwaSafeHtmlOutboundCallbacks.IsFormElementTag(context.TagId))
					{
						if (htmlTagContextAttribute.Id == HtmlAttributeId.UseMap)
						{
							this.ProcessUseMapAttribute(htmlTagContextAttribute, context, writer);
						}
						else if (OwaSafeHtmlCallbackBase.IsUrlTag(context.TagId, htmlTagContextAttribute))
						{
							if (!flag)
							{
								this.ProcessHtmlUrlTag(htmlTagContextAttribute, context, writer);
								flag = true;
							}
						}
						else if (OwaSafeHtmlCallbackBase.IsImageTag(context.TagId, htmlTagContextAttribute))
						{
							if (htmlTagContextAttribute.Id == HtmlAttributeId.Src && attachmentLink != null)
							{
								this.OutputInlineReference(htmlTagContextAttribute, context, attachmentLink, writer);
							}
							else
							{
								if ((htmlTagContextAttribute.Id != HtmlAttributeId.Src || flag2) && (htmlTagContextAttribute.Id != HtmlAttributeId.DynSrc || flag3) && (htmlTagContextAttribute.Id != HtmlAttributeId.LowSrc || flag4))
								{
									continue;
								}
								this.ProcessImageTag(htmlTagContextAttribute, context, writer);
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
						else if (OwaSafeHtmlCallbackBase.IsBackgroundAttribute(htmlTagContextAttribute))
						{
							if (this.isOutputFragment && context.TagId == HtmlTagId.Div)
							{
								attachmentLink = this.IsInlineImage(htmlTagContextAttribute);
								if (attachmentLink != null)
								{
									AttachmentPolicy.Level attachmentLevel = this.GetAttachmentLevel(attachmentLink);
									if (AttachmentPolicy.Level.Allow == attachmentLevel)
									{
										writer.WriteAttribute(HtmlAttributeId.Style, "background:url('" + this.GetInlineReferenceUrl(attachmentLevel, attachmentLink, writer) + "');");
									}
								}
							}
							else
							{
								this.ProcessImageTag(htmlTagContextAttribute, context, writer);
							}
						}
						else if (!OwaSafeHtmlOutboundCallbacks.IsTargetTagInAnchor(context.TagId, htmlTagContextAttribute))
						{
							if (OwaSafeHtmlCallbackBase.IsSanitizingAttribute(htmlTagContextAttribute))
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
					OwaSafeHtmlOutboundCallbacks.SanitizeImage(writer, text, text2, text3);
				}
				else if (!this.hasBlockedImagesInCurrentPass)
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
				if (this.hasFoundNonLocalUrlInCurrentPass)
				{
					if (this.owaContext.UserContext.IsBasicExperience)
					{
						if (!this.hasFoundMailToUrlInCurrentPass || this.openMailtoInNewWindow)
						{
							this.WriteSafeTargetBlank(writer);
						}
					}
					else
					{
						this.WriteSafeTargetBlank(writer);
					}
				}
				this.hasBlockedImagesInCurrentPass = false;
				this.hasFoundNonLocalUrlInCurrentPass = false;
				this.hasFoundMailToUrlInCurrentPass = false;
				return;
			}
		}

		protected AttachmentLink IsInlineImage(HtmlTagContextAttribute filterAttribute)
		{
			AttachmentLink attachmentLink = this.IsInlineReference(filterAttribute.Value);
			if (attachmentLink != null)
			{
				string text;
				if (attachmentLink.AttachmentType == AttachmentType.Ole)
				{
					text = "image/jpeg";
				}
				else
				{
					text = attachmentLink.ContentType;
					if (string.Compare(text, "image/tiff", StringComparison.OrdinalIgnoreCase) == 0)
					{
						return null;
					}
				}
				if (text.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
				{
					this.hasInlineImages = true;
					return attachmentLink;
				}
			}
			return null;
		}

		protected virtual void ProcessImageTag(HtmlTagContextAttribute filterAttribute, HtmlTagContext context, HtmlWriter writer)
		{
			AttachmentLink attachmentLink = this.IsInlineImage(filterAttribute);
			if (attachmentLink != null)
			{
				this.OutputInlineReference(filterAttribute, context, attachmentLink, writer);
				return;
			}
			if (this.IsSafeUrl(filterAttribute.Value, filterAttribute.Id))
			{
				this.hasBlockedImagesInCurrentPass = true;
				this.hasBlockedImages = true;
				writer.WriteAttribute(filterAttribute.Id, OwaSafeHtmlCallbackBase.blankImageFileName);
			}
		}

		protected virtual void ProcessUnfragFormTagContext(HtmlTagContext context, HtmlWriter writer)
		{
			if (this.allowForms)
			{
				context.WriteTag();
				foreach (HtmlTagContextAttribute htmlTagContextAttribute in context.Attributes)
				{
					if ((htmlTagContextAttribute.Id == HtmlAttributeId.Src || htmlTagContextAttribute.Id == HtmlAttributeId.Action) && (!this.IsSafeUrl(htmlTagContextAttribute.Value, htmlTagContextAttribute.Id) || !Redir.IsSafeUrl(htmlTagContextAttribute.Value, this.owaContext.HttpContext.Request)))
					{
						writer.WriteAttribute(htmlTagContextAttribute.Id, OwaSafeHtmlOutboundCallbacks.BlockedUrlPageValue);
					}
					else if (htmlTagContextAttribute.Id != HtmlAttributeId.Target)
					{
						htmlTagContextAttribute.Write();
					}
				}
				this.WriteSafeTargetBlank(writer);
				return;
			}
			this.hasBlockedForms = true;
		}

		private void WriteSafeTargetBlank(HtmlWriter writer)
		{
			writer.WriteAttribute(HtmlAttributeId.Target, "_blank");
			writer.WriteAttribute(HtmlAttributeId.Rel, "noopener noreferrer");
		}

		protected void ProcessHtmlUrlTag(HtmlTagContextAttribute filterAttribute, HtmlTagContext context, HtmlWriter writer)
		{
			OwaSafeHtmlOutboundCallbacks.TypeOfUrl typeOfUrl = this.GetTypeOfUrl(filterAttribute.Value, filterAttribute.Id);
			string text;
			if (typeOfUrl == OwaSafeHtmlOutboundCallbacks.TypeOfUrl.Unknown || typeOfUrl == OwaSafeHtmlOutboundCallbacks.TypeOfUrl.Local)
			{
				if (this.baseRef == null && this.isConversationsOrUnknownType && !this.triedLoadingBaseHref)
				{
					OwaLightweightHtmlCallback owaLightweightHtmlCallback = new OwaLightweightHtmlCallback();
					using (Item item = Utilities.GetItem<Item>(this.owaContext.UserContext, this.itemId, new PropertyDefinition[0]))
					{
						BodyReadConfiguration bodyReadConfiguration = new BodyReadConfiguration(BodyFormat.TextHtml, "utf-8");
						bodyReadConfiguration.SetHtmlOptions(HtmlStreamingFlags.FilterHtml, owaLightweightHtmlCallback);
						Body body = item.Body;
						if (this.owaContext.UserContext.IsIrmEnabled)
						{
							Utilities.IrmDecryptIfRestricted(item, this.owaContext.UserContext, true);
							if (Utilities.IsIrmRestrictedAndDecrypted(item))
							{
								body = ((RightsManagedMessageItem)item).ProtectedBody;
							}
						}
						using (TextReader textReader = body.OpenTextReader(bodyReadConfiguration))
						{
							int num = 5000;
							char[] buffer = new char[num];
							textReader.Read(buffer, 0, num);
						}
					}
					this.baseRef = owaLightweightHtmlCallback.BaseRef;
					this.triedLoadingBaseHref = true;
				}
				text = this.GetAbsoluteUrl(filterAttribute.Value, filterAttribute.Id);
				typeOfUrl = this.GetTypeOfUrl(text, filterAttribute.Id);
			}
			else
			{
				text = filterAttribute.Value;
			}
			switch (typeOfUrl)
			{
			case OwaSafeHtmlOutboundCallbacks.TypeOfUrl.Local:
				if (this.owaContext.UserContext.BrowserType != BrowserType.Safari && !this.owaContext.UserContext.IsBasicExperience && !this.isConversations)
				{
					writer.WriteAttribute(filterAttribute.Id, OwaSafeHtmlCallbackBase.JSLocalLink + OwaSafeHtmlCallbackBase.JSMethodPrefix + filterAttribute.Value.Substring(1) + OwaSafeHtmlCallbackBase.JSMethodSuffix);
					return;
				}
				filterAttribute.Write();
				return;
			case OwaSafeHtmlOutboundCallbacks.TypeOfUrl.Trusted:
				filterAttribute.Write();
				this.hasFoundNonLocalUrlInCurrentPass = true;
				return;
			case OwaSafeHtmlOutboundCallbacks.TypeOfUrl.Redirection:
				filterAttribute.WriteName();
				writer.WriteAttributeValue(Redir.BuildRedirUrl(this.owaContext.UserContext, text));
				this.hasFoundNonLocalUrlInCurrentPass = true;
				return;
			case OwaSafeHtmlOutboundCallbacks.TypeOfUrl.Unknown:
				writer.WriteAttribute(filterAttribute.Id, OwaSafeHtmlOutboundCallbacks.BlockedUrlPageValue);
				this.hasFoundNonLocalUrlInCurrentPass = true;
				return;
			default:
				return;
			}
		}

		protected virtual void ProcessUseMapAttribute(HtmlTagContextAttribute filterAttribute, HtmlTagContext context, HtmlWriter writer)
		{
		}

		protected void OutputInlineReference(HtmlTagContextAttribute filterAttribute, HtmlTagContext context, AttachmentLink imageAttachmentLink, HtmlWriter writer)
		{
			AttachmentPolicy.Level attachmentLevel = this.GetAttachmentLevel(imageAttachmentLink);
			if (AttachmentPolicy.Level.Allow == attachmentLevel && filterAttribute.Id == HtmlAttributeId.Href)
			{
				writer.WriteAttribute(HtmlAttributeId.Src, this.GetInlineReferenceUrl(attachmentLevel, imageAttachmentLink, writer));
				return;
			}
			string value;
			if (this.owaContext.ShouldDeferInlineImages)
			{
				value = this.owaContext.UserContext.GetThemeFileUrl(ThemeFileId.Clear1x1) + "#" + OwaSafeHtmlOutboundCallbacks.DeferImageUrlDelimiter + this.GetInlineReferenceUrl(attachmentLevel, imageAttachmentLink, writer);
			}
			else
			{
				value = this.GetInlineReferenceUrl(attachmentLevel, imageAttachmentLink, writer);
			}
			writer.WriteAttribute(filterAttribute.Id, value);
		}

		private string GetInlineReferenceUrl(AttachmentPolicy.Level level, AttachmentLink imageAttachmentLink, HtmlWriter writer)
		{
			imageAttachmentLink.MarkInline(true);
			if (AttachmentPolicy.Level.Allow == level)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (this.isEmbeddedItem)
				{
					stringBuilder.Append(this.embeddedItemUrl);
				}
				else
				{
					stringBuilder.Append(OwaSafeHtmlCallbackBase.AttachmentBaseUrl);
					bool flag = this.owaStoreObjectIdType == OwaStoreObjectIdType.OtherUserMailboxObject || this.owaStoreObjectIdType == OwaStoreObjectIdType.ArchiveMailboxObject;
					OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromItemId(this.itemId, flag ? null : this.parentId, this.owaStoreObjectIdType, this.legacyDN);
					stringBuilder.Append(Utilities.UrlEncode(owaStoreObjectId.ToString()));
					stringBuilder.Append("&attcnt=1&attid0=");
				}
				stringBuilder.Append(Utilities.UrlEncode(imageAttachmentLink.AttachmentId.ToBase64String()));
				if (!this.isEmbeddedItem && !string.IsNullOrEmpty(imageAttachmentLink.ContentId))
				{
					stringBuilder.Append("&attcid0=");
					stringBuilder.Append(Utilities.UrlEncode(imageAttachmentLink.ContentId));
				}
				return stringBuilder.ToString();
			}
			this.hasBlockedImagesInCurrentPass = true;
			this.hasBlockedInlineAttachments = true;
			return OwaSafeHtmlOutboundCallbacks.BlockedUrlPageValue;
		}

		protected AttachmentLink IsInlineReference(string bodyRef)
		{
			if (this.baseRef != null)
			{
				return base.FindAttachmentByBodyReference(bodyRef, this.baseRef);
			}
			return base.FindAttachmentByBodyReference(bodyRef);
		}

		protected AttachmentPolicy.Level GetAttachmentLevel(AttachmentLink attachmentLink)
		{
			if (this.isJunkOrPhishing)
			{
				return AttachmentPolicy.Level.Block;
			}
			if (attachmentLink.AttachmentType == AttachmentType.EmbeddedMessage)
			{
				return AttachmentPolicy.Level.Block;
			}
			return AttachmentLevelLookup.GetAttachmentLevel(attachmentLink, this.owaContext.UserContext);
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
					writer.WriteAttribute(HtmlAttributeId.Width, num3.ToString());
				}
				if (num2 >= 0)
				{
					writer.WriteAttribute(HtmlAttributeId.Height, num2.ToString());
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

		protected bool IsSafeUrl(string urlString, HtmlAttributeId htmlAttrId)
		{
			string absoluteUrl = this.GetAbsoluteUrl(urlString, htmlAttrId);
			OwaSafeHtmlOutboundCallbacks.TypeOfUrl typeOfUrl = this.GetTypeOfUrl(absoluteUrl, htmlAttrId);
			return typeOfUrl != OwaSafeHtmlOutboundCallbacks.TypeOfUrl.Unknown;
		}

		protected OwaSafeHtmlOutboundCallbacks.TypeOfUrl GetTypeOfUrl(string urlString, HtmlAttributeId htmlAttrId)
		{
			if (string.IsNullOrEmpty(urlString))
			{
				return OwaSafeHtmlOutboundCallbacks.TypeOfUrl.Unknown;
			}
			if (urlString.StartsWith(OwaSafeHtmlCallbackBase.LocalUrlPrefix, StringComparison.Ordinal))
			{
				return OwaSafeHtmlOutboundCallbacks.TypeOfUrl.Local;
			}
			Uri uri;
			if (null == (uri = Utilities.TryParseUri(urlString)))
			{
				return OwaSafeHtmlOutboundCallbacks.TypeOfUrl.Unknown;
			}
			string scheme = uri.Scheme;
			if (string.IsNullOrEmpty(scheme))
			{
				return OwaSafeHtmlOutboundCallbacks.TypeOfUrl.Unknown;
			}
			if (CultureInfo.InvariantCulture.CompareInfo.Compare(scheme, "file", CompareOptions.IgnoreCase) == 0 && htmlAttrId == HtmlAttributeId.Href)
			{
				return OwaSafeHtmlOutboundCallbacks.TypeOfUrl.Redirection;
			}
			for (int i = 0; i < OwaSafeHtmlOutboundCallbacks.RedirProtocols.Length; i++)
			{
				if (CultureInfo.InvariantCulture.CompareInfo.Compare(scheme, OwaSafeHtmlOutboundCallbacks.RedirProtocols[i], CompareOptions.IgnoreCase) == 0)
				{
					if (CultureInfo.InvariantCulture.CompareInfo.Compare(scheme, "mailto", CompareOptions.IgnoreCase) == 0 && htmlAttrId == HtmlAttributeId.Href)
					{
						this.hasFoundMailToUrlInCurrentPass = true;
					}
					return OwaSafeHtmlOutboundCallbacks.TypeOfUrl.Redirection;
				}
			}
			return OwaSafeHtmlOutboundCallbacks.TypeOfUrl.Trusted;
		}

		protected string GetAbsoluteUrl(string urlString, HtmlAttributeId htmlAttributeId)
		{
			if (urlString == null)
			{
				return null;
			}
			Uri uri = Utilities.TryParseUri(urlString, UriKind.RelativeOrAbsolute);
			if (uri == null)
			{
				return null;
			}
			if (!uri.IsAbsoluteUri && this.baseRef != null)
			{
				Uri uri2;
				Uri.TryCreate(this.baseRef, uri, out uri2);
				if (uri2 != null)
				{
					return uri2.AbsoluteUri;
				}
			}
			else if (uri.IsAbsoluteUri || this.GetTypeOfUrl(urlString, htmlAttributeId) == OwaSafeHtmlOutboundCallbacks.TypeOfUrl.Local)
			{
				return uri.OriginalString;
			}
			return null;
		}

		protected static bool IsTargetTagInAnchor(HtmlTagId tagId, HtmlTagContextAttribute attr)
		{
			return (tagId == HtmlTagId.A && attr.Id == HtmlAttributeId.Target) || (tagId == HtmlTagId.Area && attr.Id == HtmlAttributeId.Target);
		}

		protected static bool IsFormElementTag(HtmlTagId tagId)
		{
			return tagId == HtmlTagId.Input || tagId == HtmlTagId.Button || tagId == HtmlTagId.Select || tagId == HtmlTagId.OptGroup || tagId == HtmlTagId.Option || tagId == HtmlTagId.FieldSet || tagId == HtmlTagId.TextArea || tagId == HtmlTagId.IsIndex || tagId == HtmlTagId.Label || tagId == HtmlTagId.Legend;
		}

		private static readonly string DeferImageUrlDelimiter = "__OWA_DEFERIMG000__";

		private static readonly string BlockedUrlPageValue = "UrlBlockedError.aspx";

		protected bool hasBlockedForms;

		protected bool hasInlineImages;

		protected bool allowForms = true;

		protected bool hasBlockedImagesInCurrentPass;

		protected bool hasFoundNonLocalUrlInCurrentPass;

		protected bool hasFoundMailToUrlInCurrentPass;

		protected bool isLoggedOnFromPublicComputer;

		protected bool openMailtoInNewWindow;

		protected bool isOutputFragment;

		protected OwaContext owaContext;

		protected bool isEmbeddedItem;

		protected string embeddedItemUrl;

		protected Uri baseRef;

		public static readonly string[] RedirProtocols = new string[]
		{
			"http",
			"https",
			"mailto",
			"mhtml"
		};

		private bool isJunkOrPhishing;

		private StoreObjectId itemId;

		private StoreObjectId parentId;

		private string objectClass;

		private string charSet;

		private BodyFormat bodyFormat;

		private OwaStoreObjectIdType owaStoreObjectIdType;

		private string legacyDN;

		private bool isConversations;

		private bool isConversationsOrUnknownType;

		private bool triedLoadingBaseHref;

		protected enum TypeOfUrl
		{
			Local = 1,
			Trusted,
			Redirection,
			Unknown
		}
	}
}
