using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;

namespace Microsoft.Exchange.Entities.TypeConversion.Converters
{
	internal abstract class HtmlBodyCallback : HtmlCallbackBase
	{
		internal bool AddBlankTargetToLinks { get; set; }

		internal bool BlockExternalImages { get; set; }

		internal bool BlockUnsafeImages { get; set; }

		internal string BlockedUrlImageValue { get; set; }

		internal string BlockedUrlPageValue { get; set; }

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

		internal bool IncludeEmptyLinks { get; set; }

		internal string InlineImageCustomDataTemplate { get; set; }

		internal string InlineImageUrlOnLoadTemplate { get; set; }

		internal string InlineImageUrlTemplate { get; set; }

		internal bool IsBodyFragment { get; set; }

		internal bool RemoveLinksToNonImageAttachments { get; set; }

		private protected Uri BaseRef { protected get; private set; }

		protected Action<string> InlineAttachmentIdAction { get; set; }

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

		internal static HtmlBodyCallback.TypeOfUrl GetTypeOfUrl(string urlString, HtmlAttributeId htmlAttrId)
		{
			if (string.IsNullOrEmpty(urlString))
			{
				return HtmlBodyCallback.TypeOfUrl.Unknown;
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

		internal static bool IsSafeUrl(string urlString, HtmlAttributeId htmlAttr)
		{
			HtmlBodyCallback.TypeOfUrl typeOfUrl = HtmlBodyCallback.GetTypeOfUrl(urlString, htmlAttr);
			return typeOfUrl != HtmlBodyCallback.TypeOfUrl.Unknown;
		}

		internal static Uri TryParseUri(string uriString, UriKind uriKind)
		{
			Uri result;
			if (Uri.TryCreate(uriString, uriKind, out result))
			{
				return result;
			}
			return null;
		}

		protected static void WriteAllAttributesExcept(HtmlTagContext tagContext, HtmlAttributeId attrToSkip)
		{
			foreach (HtmlTagContextAttribute htmlTagContextAttribute in from attribute in tagContext.Attributes
			where attribute.Id != attrToSkip
			select attribute)
			{
				htmlTagContextAttribute.Write();
			}
		}

		protected static void WriteAllAttributesExcept(HtmlTagContext tagContext, Func<HtmlAttributeId, bool> shouldSkipFunc)
		{
			foreach (HtmlTagContextAttribute htmlTagContextAttribute in tagContext.Attributes)
			{
				if (!shouldSkipFunc(htmlTagContextAttribute.Id))
				{
					htmlTagContextAttribute.Write();
				}
			}
		}

		protected void GetAnchorReference(HtmlTagContext tagContext, out string hrefValue)
		{
			hrefValue = (from attr in tagContext.Attributes
			where attr.Id == HtmlAttributeId.Href
			select attr.Value).FirstOrDefault<string>();
		}

		protected void GetAttachmentLinkAndLinkSource(HtmlTagContext tagContext, out AttachmentLink link, out string srcValue)
		{
			link = null;
			srcValue = (from attr in tagContext.Attributes
			where attr.Id == HtmlAttributeId.Src
			select attr.Value).FirstOrDefault<string>();
			if (!string.IsNullOrEmpty(srcValue))
			{
				link = base.FindAttachmentByBodyReference(srcValue);
			}
		}

		protected string GetContentType(AttachmentLink link)
		{
			if (link.AttachmentType == AttachmentType.Ole)
			{
				link.ConvertToImage();
				return "image/jpeg";
			}
			return link.ContentType;
		}

		protected void ProcessBaseTag(HtmlTagContext tagContext, HtmlWriter htmlWriter)
		{
			using (IEnumerator<string> enumerator = (from filterAttribute in tagContext.Attributes
			where filterAttribute.Id == HtmlAttributeId.Href
			select filterAttribute.Value).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					string uriString = enumerator.Current;
					this.BaseRef = HtmlBodyCallback.TryParseUri(uriString, UriKind.Absolute);
				}
			}
		}

		public const string BlockedImageSrcAttributeName = "blockedImageSrc";

		public const string ContentIdUrlScheme = "cid:";

		public const string CustomDataAttributeName = "data-custom";

		public const string OnLoadAttributeName = "onload";

		public const string OriginalSrcAttributeName = "originalSrc";

		private Action<bool> hasBlockedImagesAction;

		internal enum TypeOfUrl
		{
			Trusted,
			Unknown
		}
	}
}
