using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class HtmlBodyCallback : HtmlCallbackBase
	{
		static HtmlBodyCallback()
		{
			HtmlBodyCallback.WellKnownInlineImageOnLoadTemplates.Add("InlineImageLoader.GetLoader().Load(this)");
		}

		public HtmlBodyCallback(Item item, IdAndSession idAndSession = null, bool getAttachmentCollectionWhenClientSmimeInstalled = false) : base(Util.GetEffectiveAttachmentCollection(item, getAttachmentCollectionWhenClientSmimeInstalled), Util.GetEffectiveBody(item))
		{
			if (idAndSession == null)
			{
				if (PropertyCommand.InMemoryProcessOnly)
				{
					this.cachedItem = item;
				}
				else
				{
					this.idAndSession = IdAndSession.CreateFromItem(item);
				}
			}
			else
			{
				this.idAndSession = idAndSession;
			}
			this.BlockedUrlPageValue = string.Empty;
			this.BlockUnsafeImages = true;
			base.InitializeAttachmentLinks(null);
		}

		public override void ProcessTag(HtmlTagContext tagContext, HtmlWriter htmlWriter)
		{
			HtmlTagId tagId = tagContext.TagId;
			if (tagId <= HtmlTagId.Base)
			{
				if (tagId == HtmlTagId.A)
				{
					this.ProcessAnchorTag(tagContext, htmlWriter);
					return;
				}
				switch (tagId)
				{
				case HtmlTagId.Area:
					this.ProcessAreaTag(tagContext, htmlWriter);
					return;
				case HtmlTagId.Base:
					this.ProcessBaseTag(tagContext, htmlWriter);
					return;
				}
			}
			else
			{
				if (tagId == HtmlTagId.Form)
				{
					this.ProcessFormTag(tagContext, htmlWriter);
					return;
				}
				switch (tagId)
				{
				case HtmlTagId.Img:
					this.ProcessImageTag(tagContext, htmlWriter);
					return;
				case HtmlTagId.Input:
					this.ProcessInputTag(tagContext, htmlWriter);
					return;
				}
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

		private static void WriteAllAttributesExcept(HtmlTagContext tagContext, Func<HtmlAttributeId, bool> shouldSkipFunc)
		{
			foreach (HtmlTagContextAttribute htmlTagContextAttribute in tagContext.Attributes)
			{
				if (!shouldSkipFunc(htmlTagContextAttribute.Id))
				{
					htmlTagContextAttribute.Write();
				}
			}
		}

		private void ProcessAnchorTag(HtmlTagContext tagContext, HtmlWriter htmlWriter)
		{
			if (this.AddBlankTargetToLinks)
			{
				this.AddBlankTarget(tagContext, htmlWriter, HtmlAttributeId.Href);
				return;
			}
			tagContext.WriteTag(true);
		}

		private void AddBlankTarget(HtmlTagContext tagContext, HtmlWriter htmlWriter, HtmlAttributeId anchorAttribute = HtmlAttributeId.Href)
		{
			string text;
			this.GetAnchorReference(tagContext, anchorAttribute, out text);
			string value = text;
			HtmlBodyCallback.TypeOfUrl typeOfUrl = HtmlBodyCallback.TypeOfUrl.Unknown;
			if (text != null)
			{
				value = this.GetAbsoluteUrl(text, this.BlockedUrlPageValue, HtmlAttributeId.Href, out typeOfUrl);
			}
			if (string.IsNullOrEmpty(value))
			{
				tagContext.DeleteTag();
				if (!string.IsNullOrEmpty(text))
				{
					htmlWriter.WriteText(string.Format("[{0}]", text));
					return;
				}
			}
			else
			{
				tagContext.WriteTag(false);
				htmlWriter.WriteAttribute(anchorAttribute, value);
				if (typeOfUrl != HtmlBodyCallback.TypeOfUrl.Local)
				{
					htmlWriter.WriteAttribute(HtmlAttributeId.Target, "_blank");
				}
				HtmlBodyCallback.WriteAllAttributesExcept(tagContext, (HtmlAttributeId id) => id == anchorAttribute || id == HtmlAttributeId.Target);
			}
		}

		private string GetAbsoluteUrl(string hrefValue, string blockedUrlValue, HtmlAttributeId attrId)
		{
			HtmlBodyCallback.TypeOfUrl typeOfUrl;
			return this.GetAbsoluteUrl(hrefValue, blockedUrlValue, attrId, out typeOfUrl);
		}

		private string GetAbsoluteUrl(string hrefValue, string blockedUrlValue, HtmlAttributeId attrId, out HtmlBodyCallback.TypeOfUrl urlType)
		{
			string text = hrefValue;
			urlType = HtmlBodyCallback.GetTypeOfUrl(hrefValue, attrId);
			if (urlType == HtmlBodyCallback.TypeOfUrl.Unknown && !string.IsNullOrEmpty(hrefValue))
			{
				if (this.baseRef == null && !this.triedToLoadBaseHref && this.IsBodyFragment)
				{
					this.TryToDetermineBaseRef();
				}
				text = HtmlBodyCallback.GetAbsoluteUrl(hrefValue, HtmlAttributeId.Href, this.baseRef);
				urlType = HtmlBodyCallback.GetTypeOfUrl(text, HtmlAttributeId.Href);
			}
			if (urlType == HtmlBodyCallback.TypeOfUrl.Unknown)
			{
				text = blockedUrlValue;
			}
			return text;
		}

		private void TryToDetermineBaseRef()
		{
			HtmlBodyCallbackForBaseTag htmlBodyCallbackForBaseTag = new HtmlBodyCallbackForBaseTag();
			using (Item item = this.cachedItem ?? ServiceCommandBase.GetXsoItem(this.idAndSession.Session, this.idAndSession.Id, new PropertyDefinition[0]))
			{
				BodyReadConfiguration bodyReadConfiguration = new BodyReadConfiguration(BodyFormat.TextHtml, "utf-8");
				bodyReadConfiguration.SetHtmlOptions(HtmlStreamingFlags.FilterHtml, htmlBodyCallbackForBaseTag);
				Body effectiveBody = Util.GetEffectiveBody(item);
				using (TextReader textReader = effectiveBody.OpenTextReader(bodyReadConfiguration))
				{
					int num = 5000;
					char[] buffer = new char[num];
					textReader.Read(buffer, 0, num);
				}
			}
			this.baseRef = htmlBodyCallbackForBaseTag.BaseHref;
			this.triedToLoadBaseHref = true;
		}

		private void ProcessImageTag(HtmlTagContext tagContext, HtmlWriter htmlWriter)
		{
			AttachmentLink attachmentLink;
			string srcValue;
			this.GetAttachmentLinkAndLinkSource(tagContext, out attachmentLink, out srcValue);
			if (attachmentLink == null)
			{
				this.ProcessExternalImageRef(tagContext, htmlWriter, srcValue, HtmlAttributeId.Src);
				return;
			}
			this.ProcessInlineImageRef(tagContext, htmlWriter, attachmentLink, srcValue);
		}

		private void ProcessInlineImageRef(HtmlTagContext tagContext, HtmlWriter htmlWriter, AttachmentLink link, string srcValue)
		{
			tagContext.WriteTag(false);
			if (link.IsOriginallyInline)
			{
				link.MarkInline(true);
			}
			else
			{
				if (srcValue != null && srcValue.StartsWith("objattph://"))
				{
					link.MarkInline(false);
					return;
				}
				link.MarkInline(true);
			}
			if (string.IsNullOrEmpty(link.ContentId))
			{
				link.ContentId = string.Format("{0}@1", HexConverter.ByteArrayToHexString(Guid.NewGuid().ToByteArray()));
			}
			string text = "cid:" + link.ContentId;
			string value = text;
			if (!string.IsNullOrEmpty(this.InlineImageUrlTemplate))
			{
				value = this.ApplyInlineImageIdToTemplate(link, this.InlineImageUrlTemplate);
				if (!string.IsNullOrEmpty(value))
				{
					htmlWriter.WriteAttribute("originalSrc", text);
					this.DoInlineAttachmentIdAction(link);
				}
				else
				{
					value = text;
				}
			}
			else if (this.CalculateAttachmentInlineProps)
			{
				this.DoInlineAttachmentIdAction(link);
			}
			if (this.IsWellKnownInlineImageOnLoadTemplate(this.InlineImageUrlOnLoadTemplate))
			{
				string inlineImageUrlOnLoadTemplate = this.InlineImageUrlOnLoadTemplate;
				htmlWriter.WriteAttribute("onload", inlineImageUrlOnLoadTemplate);
			}
			if (!string.IsNullOrEmpty(this.InlineImageCustomDataTemplate))
			{
				string value2 = this.ApplyInlineImageIdToTemplate(link, this.InlineImageCustomDataTemplate);
				if (!string.IsNullOrEmpty(value2))
				{
					htmlWriter.WriteAttribute("data-custom", value2);
				}
			}
			htmlWriter.WriteAttribute(HtmlAttributeId.Src, value);
			HtmlBodyCallback.WriteAllAttributesExcept(tagContext, HtmlAttributeId.Src);
		}

		private bool IsWellKnownInlineImageOnLoadTemplate(string template)
		{
			return HtmlBodyCallback.WellKnownInlineImageOnLoadTemplates.Contains(template);
		}

		private void ProcessExternalImageRef(HtmlTagContext tagContext, HtmlWriter htmlWriter, string srcValue, HtmlAttributeId imgSrcAttributeId)
		{
			if (HtmlBodyCallback.IsEmptyLink(srcValue))
			{
				if (this.IncludeEmptyLinks)
				{
					tagContext.WriteTag(true);
				}
				return;
			}
			string text = this.GetAbsoluteUrl(srcValue, this.BlockedUrlImageValue, imgSrcAttributeId);
			if (this.BlockUnsafeImages)
			{
				if (HtmlBodyCallback.IsSafeUrl(text, imgSrcAttributeId))
				{
					if (this.BlockExternalImages)
					{
						tagContext.WriteTag(false);
						htmlWriter.WriteAttribute("blockedImageSrc", text);
						HtmlBodyCallback.WriteAllAttributesExcept(tagContext, imgSrcAttributeId);
						if (this.hasBlockedImagesAction != null)
						{
							this.hasBlockedImagesAction(true);
						}
						return;
					}
				}
				else
				{
					text = this.BlockedUrlImageValue;
				}
			}
			tagContext.WriteTag(false);
			htmlWriter.WriteAttribute(imgSrcAttributeId, text);
			HtmlBodyCallback.WriteAllAttributesExcept(tagContext, imgSrcAttributeId);
		}

		private void ProcessBaseTag(HtmlTagContext tagContext, HtmlWriter htmlWriter)
		{
			foreach (HtmlTagContextAttribute htmlTagContextAttribute in tagContext.Attributes)
			{
				if (htmlTagContextAttribute.Id == HtmlAttributeId.Href)
				{
					string value = htmlTagContextAttribute.Value;
					this.baseRef = HtmlBodyCallback.TryParseUri(value, UriKind.Absolute);
					break;
				}
			}
		}

		private void ProcessAreaTag(HtmlTagContext tagContext, HtmlWriter htmlWriter)
		{
			if (this.AddBlankTargetToLinks)
			{
				this.AddBlankTarget(tagContext, htmlWriter, HtmlAttributeId.Href);
				return;
			}
			tagContext.WriteTag(true);
		}

		private void ProcessFormTag(HtmlTagContext tagContext, HtmlWriter htmlWriter)
		{
			if (this.AddBlankTargetToLinks)
			{
				this.AddBlankTarget(tagContext, htmlWriter, HtmlAttributeId.Action);
				return;
			}
			tagContext.WriteTag(true);
		}

		private void ProcessInputTag(HtmlTagContext tagContext, HtmlWriter htmlWriter)
		{
			bool flag = false;
			bool flag2 = false;
			foreach (HtmlTagContextAttribute htmlTagContextAttribute in tagContext.Attributes)
			{
				if (htmlTagContextAttribute.Id == HtmlAttributeId.Src)
				{
					flag = true;
				}
				else if (htmlTagContextAttribute.Id == HtmlAttributeId.Type && htmlTagContextAttribute.Value.ToLower() == "image")
				{
					flag2 = true;
				}
			}
			if (flag && flag2)
			{
				AttachmentLink attachmentLink;
				string srcValue;
				this.GetAttachmentLinkAndLinkSource(tagContext, out attachmentLink, out srcValue);
				if (attachmentLink == null)
				{
					this.ProcessExternalImageRef(tagContext, htmlWriter, srcValue, HtmlAttributeId.Src);
					return;
				}
			}
			tagContext.WriteTag(true);
		}

		private string ApplyInlineImageIdToTemplate(AttachmentLink link, string srcTemplate)
		{
			if (link.AttachmentType == AttachmentType.EmbeddedMessage)
			{
				return null;
			}
			if (this.idAndSession != null)
			{
				IdAndSession idAndSession = this.idAndSession.Clone();
				idAndSession.AttachmentIds.Add(link.AttachmentId);
				string id = idAndSession.GetConcatenatedId().Id;
				string newValue = Uri.EscapeDataString(id);
				return srcTemplate.Replace("{id}", newValue);
			}
			if (!string.IsNullOrEmpty(link.ContentId))
			{
				string stringToEscape = Convert.ToBase64String(Encoding.UTF8.GetBytes(link.ContentId));
				string newValue2 = Uri.EscapeDataString(stringToEscape);
				return srcTemplate.Replace("{id}", newValue2);
			}
			return null;
		}

		private void GetAttachmentLinkAndLinkSource(HtmlTagContext tagContext, out AttachmentLink link, out string srcValue)
		{
			link = null;
			srcValue = null;
			foreach (HtmlTagContextAttribute htmlTagContextAttribute in tagContext.Attributes)
			{
				if (htmlTagContextAttribute.Id == HtmlAttributeId.Src)
				{
					srcValue = htmlTagContextAttribute.Value;
					break;
				}
			}
			if (!string.IsNullOrEmpty(srcValue))
			{
				link = base.FindAttachmentByBodyReference(srcValue);
			}
		}

		private void GetAnchorReference(HtmlTagContext tagContext, HtmlAttributeId anchorAttribute, out string hrefValue)
		{
			hrefValue = null;
			foreach (HtmlTagContextAttribute htmlTagContextAttribute in tagContext.Attributes)
			{
				if (htmlTagContextAttribute.Id == anchorAttribute)
				{
					hrefValue = htmlTagContextAttribute.Value;
					break;
				}
			}
		}

		private string GetContentType(AttachmentLink link)
		{
			if (link.AttachmentType == AttachmentType.Ole)
			{
				link.ConvertToImage();
				return "image/jpeg";
			}
			return link.ContentType;
		}

		private void DoInlineAttachmentIdAction(AttachmentLink link)
		{
			if (this.inlineAttachmentIdAction != null)
			{
				if (this.idAndSession != null)
				{
					IdAndSession idAndSession = this.idAndSession.Clone();
					idAndSession.AttachmentIds.Add(link.AttachmentId);
					this.inlineAttachmentIdAction(idAndSession.GetConcatenatedId().Id);
					return;
				}
				if (!string.IsNullOrEmpty(link.ContentId))
				{
					this.inlineAttachmentIdAction(Convert.ToBase64String(Encoding.UTF8.GetBytes(link.ContentId)));
				}
			}
		}

		internal static bool IsEmptyLink(string src)
		{
			return string.Compare(src, "objattph://", StringComparison.CurrentCultureIgnoreCase) == 0 || string.Compare(src, "rtfimage://", StringComparison.CurrentCultureIgnoreCase) == 0 || string.Compare(src, "cid:", StringComparison.CurrentCultureIgnoreCase) == 0;
		}

		internal static Uri TryParseUri(string uriString, UriKind uriKind)
		{
			Uri result = null;
			if (!Uri.TryCreate(uriString, uriKind, out result))
			{
				return null;
			}
			return result;
		}

		internal static HtmlBodyCallback.TypeOfUrl GetTypeOfUrl(string urlString, HtmlAttributeId htmlAttrId)
		{
			if (string.IsNullOrEmpty(urlString))
			{
				return HtmlBodyCallback.TypeOfUrl.Unknown;
			}
			if (urlString.StartsWith("#", StringComparison.Ordinal))
			{
				return HtmlBodyCallback.TypeOfUrl.Local;
			}
			Uri uri;
			if (!Uri.TryCreate(urlString, UriKind.Absolute, out uri))
			{
				return HtmlBodyCallback.TypeOfUrl.Unknown;
			}
			string scheme = uri.Scheme;
			if (string.IsNullOrEmpty(scheme))
			{
				return HtmlBodyCallback.TypeOfUrl.Unknown;
			}
			if (CultureInfo.InvariantCulture.CompareInfo.Compare(scheme, "file", CompareOptions.IgnoreCase) != 0)
			{
				return HtmlBodyCallback.TypeOfUrl.Trusted;
			}
			if (htmlAttrId != HtmlAttributeId.Href)
			{
				return HtmlBodyCallback.TypeOfUrl.Unknown;
			}
			return HtmlBodyCallback.TypeOfUrl.Trusted;
		}

		internal static string GetAbsoluteUrl(string urlString, HtmlAttributeId htmlAttributeId, Uri baseRef)
		{
			if (string.IsNullOrEmpty(urlString))
			{
				return null;
			}
			Uri uri;
			if (!Uri.TryCreate(urlString, UriKind.RelativeOrAbsolute, out uri))
			{
				return null;
			}
			if (!uri.IsAbsoluteUri && baseRef != null)
			{
				Uri uri2;
				if (Uri.TryCreate(baseRef, uri, out uri2))
				{
					return uri2.AbsoluteUri;
				}
			}
			else if (uri.IsAbsoluteUri)
			{
				return uri.OriginalString;
			}
			return null;
		}

		internal static bool IsSafeUrl(string urlString, HtmlAttributeId htmlAttr)
		{
			HtmlBodyCallback.TypeOfUrl typeOfUrl = HtmlBodyCallback.GetTypeOfUrl(urlString, htmlAttr);
			return typeOfUrl != HtmlBodyCallback.TypeOfUrl.Unknown;
		}

		internal bool IncludeEmptyLinks { get; set; }

		internal bool RemoveLinksToNonImageAttachments { get; set; }

		internal string InlineImageUrlTemplate { get; set; }

		internal string InlineImageUrlOnLoadTemplate { get; set; }

		internal string InlineImageCustomDataTemplate { get; set; }

		internal bool AddBlankTargetToLinks { get; set; }

		internal bool BlockExternalImages { get; set; }

		internal string BlockedUrlPageValue { get; set; }

		internal string BlockedUrlImageValue { get; set; }

		internal bool BlockUnsafeImages { get; set; }

		internal bool IsBodyFragment { get; set; }

		internal bool CalculateAttachmentInlineProps { get; set; }

		internal Action<bool> HasBlockedImagesAction
		{
			set
			{
				this.hasBlockedImagesAction = value;
				if (value == null)
				{
					this.hasBlockedImagesAction(false);
				}
			}
		}

		internal Action<string> InlineAttachmentIdAction
		{
			set
			{
				this.inlineAttachmentIdAction = value;
			}
		}

		private const string DefaultAnchorTarget = "_blank";

		private const string LocalUrlPrefix = "#";

		public const string OriginalSrcAttributeName = "originalSrc";

		public const string OnLoadAttributeName = "onload";

		public const string CustomDataAttributeName = "data-custom";

		public const string BlockedImageSrcAttributeName = "blockedImageSrc";

		private const string InlineRTFattachmentScheme = "objattph://";

		private const string EmbeddedRTFImage = "rtfimage://";

		public const string ContentIdUrlScheme = "cid:";

		private const string DataUrlContentEncoding = "base64";

		private const string AttachmentIdParameter = "{id}";

		private const string BlockedUrlFormatString = "[{0}]";

		private const string ImageTypeAttributeValue = "image";

		private static readonly HashSet<string> WellKnownInlineImageOnLoadTemplates = new HashSet<string>();

		private Action<bool> hasBlockedImagesAction;

		private Action<string> inlineAttachmentIdAction;

		private Uri baseRef;

		private bool triedToLoadBaseHref;

		private IdAndSession idAndSession;

		private Item cachedItem;

		internal enum TypeOfUrl
		{
			Trusted,
			Local,
			Unknown
		}
	}
}
