using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Services;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class WebPageLinkPreviewBuilder : LinkPreviewBuilder
	{
		public WebPageLinkPreviewBuilder(GetLinkPreviewRequest request, string responseString, RequestDetailsLogger logger, Uri responseUri, bool isVideo) : base(request, logger, responseUri, isVideo)
		{
			this.responseString = responseString;
		}

		internal override GetLinkPreviewResponse Execute()
		{
			GetLinkPreviewResponse getLinkPreviewResponse = new GetLinkPreviewResponse();
			LinkPreview linkPreview = this.CreateLinkPreviewInstance();
			linkPreview.Id = this.id;
			linkPreview.Url = this.url;
			linkPreview.RequestStartTimeMilliseconds = this.requestStartTimeMilliseconds;
			int imageTagCount;
			linkPreview.ImageUrl = this.GetImage(out imageTagCount);
			linkPreview.Title = this.GetTitle();
			int descriptionTagCount;
			linkPreview.Description = this.GetDescription(out descriptionTagCount);
			linkPreview.IsVideo = base.IsVideo;
			if (string.IsNullOrWhiteSpace(linkPreview.Title))
			{
				if (string.IsNullOrWhiteSpace(linkPreview.Description))
				{
					GetLinkPreview.ThrowInvalidRequestException("TitleAndDescriptionNotFound", "No title or description were found.");
				}
				else if (string.IsNullOrWhiteSpace(linkPreview.ImageUrl))
				{
					GetLinkPreview.ThrowInvalidRequestException("TitleAndImageNotFound", "No title or image were found.");
				}
			}
			else if (string.IsNullOrWhiteSpace(linkPreview.Description) && string.IsNullOrWhiteSpace(linkPreview.ImageUrl))
			{
				GetLinkPreview.ThrowInvalidRequestException("DescriptionAndImageNotFound", "No description or image were found.");
			}
			this.SetAdditionalProperties(linkPreview);
			getLinkPreviewResponse.LinkPreview = linkPreview;
			getLinkPreviewResponse.ImageTagCount = imageTagCount;
			getLinkPreviewResponse.DescriptionTagCount = descriptionTagCount;
			return getLinkPreviewResponse;
		}

		protected string GetAttributeValue(string responseString, Regex getTagRegex, string tagRegexKey, Regex getAttributeRegex, string attributeRegexKey, string propertyName, out int tagCount)
		{
			string text = null;
			MatchCollection matchCollection = LinkPreviewBuilder.ExecuteRegExForMatchCollection(getTagRegex, responseString, propertyName);
			tagCount = matchCollection.Count;
			if (tagCount > 0)
			{
				Match match = LinkPreviewBuilder.ExecuteRegEx(getAttributeRegex, matchCollection[0].Value, propertyName);
				if (match.Groups[attributeRegexKey].Captures.Count > 0)
				{
					text = LinkPreviewBuilder.ConvertToSafeHtml(match.Groups[attributeRegexKey].Value);
					text = WebPageLinkPreviewBuilder.ReplaceSelectedHtmlEntities(text);
				}
			}
			return text;
		}

		protected static string ReplaceSelectedHtmlEntities(string stringWithEntities)
		{
			if (stringWithEntities == null || stringWithEntities.IndexOf("&", StringComparison.Ordinal) < 0)
			{
				return stringWithEntities;
			}
			string text = stringWithEntities.Replace("&amp;", "&");
			text = text.Replace("&#38;", "&");
			if (stringWithEntities.IndexOf("/", StringComparison.Ordinal) >= 0)
			{
				foreach (string oldValue in WebPageLinkPreviewBuilder.HtmlTagsToRemove)
				{
					text = text.Replace(oldValue, string.Empty);
				}
			}
			return text;
		}

		private static string[] CreateHtmlTagsToRemove()
		{
			List<string> list = new List<string>(12);
			WebPageLinkPreviewBuilder.AddToTagList(list, "i");
			WebPageLinkPreviewBuilder.AddToTagList(list, "b");
			WebPageLinkPreviewBuilder.AddToTagList(list, "u");
			return list.ToArray();
		}

		private static void AddToTagList(List<string> tagList, string tagName)
		{
			tagList.Add("&lt;" + tagName + "&gt;");
			tagList.Add("&lt;/" + tagName + "&gt;");
			tagList.Add("&#60;" + tagName + "&#62;");
			tagList.Add("&#60;/" + tagName + "&#62;");
		}

		protected static string Truncate(string stringToTruncate, int truncationLength)
		{
			if (stringToTruncate == null)
			{
				return stringToTruncate;
			}
			if (stringToTruncate.Length <= truncationLength)
			{
				return stringToTruncate;
			}
			return stringToTruncate.Substring(0, truncationLength);
		}

		protected virtual void SetAdditionalProperties(LinkPreview linkPreview)
		{
		}

		protected virtual string GetImage(out int imageTagCount)
		{
			string attributeValue = this.GetAttributeValue(this.responseString, WebPageLinkPreviewBuilder.GetImageTagRegEx, "imageTag", WebPageLinkPreviewBuilder.GetImageAttributeRegEx, "image", "image", out imageTagCount);
			return this.GetImageUrlAbsolutePath(attributeValue);
		}

		protected string GetImageUrlAbsolutePath(string imageUrl)
		{
			if (imageUrl == null)
			{
				return imageUrl;
			}
			if (imageUrl.StartsWith("//"))
			{
				this.logger.Set(GetLinkPreviewMetadata.InvalidImageUrl, imageUrl);
				return null;
			}
			Uri uri;
			if (!Uri.TryCreate(imageUrl, UriKind.RelativeOrAbsolute, out uri))
			{
				GetLinkPreview.ThrowInvalidRequestException("InvalidImageUrl", string.Format("Image url {0} is invalid.", imageUrl));
			}
			if (!uri.IsAbsoluteUri)
			{
				UriBuilder uriBuilder = new UriBuilder(this.responseUri.Scheme, this.responseUri.Host);
				if (Uri.TryCreate(uriBuilder.Uri, imageUrl, out uri))
				{
					imageUrl = uri.ToString();
				}
				else
				{
					GetLinkPreview.ThrowInvalidRequestException("InvalidImageUrl", string.Format("Image url {0} is invalid.", imageUrl));
				}
			}
			if (imageUrl != null && imageUrl.Length > 500)
			{
				GetLinkPreview.ThrowInvalidRequestException("MaxImageUrlLengthExceeded", string.Format("Image url length {0} exceeds the maximum length allowed.", imageUrl.Length));
			}
			return imageUrl;
		}

		protected virtual string GetTitle()
		{
			string text = null;
			Match match = LinkPreviewBuilder.ExecuteRegEx(WebPageLinkPreviewBuilder.GetHtmlTitleRegEx, this.responseString, "title");
			if (match.Groups["title"].Captures.Count > 0)
			{
				text = LinkPreviewBuilder.ConvertToSafeHtml(match.Groups["title"].Value);
				text = WebPageLinkPreviewBuilder.ReplaceSelectedHtmlEntities(text);
			}
			this.logger.Set(GetLinkPreviewMetadata.TitleLength, WebPageLinkPreviewBuilder.GetStringLength(text));
			return WebPageLinkPreviewBuilder.Truncate(text, 400);
		}

		protected virtual string GetDescription(out int descriptionTagCount)
		{
			string attributeValue = this.GetAttributeValue(this.responseString, WebPageLinkPreviewBuilder.GetDescriptionTagRegEx, "descriptionTag", WebPageLinkPreviewBuilder.GetDescriptionAttributeRegEx, "description", "description", out descriptionTagCount);
			this.logger.Set(GetLinkPreviewMetadata.DescriptionLength, WebPageLinkPreviewBuilder.GetStringLength(attributeValue));
			return WebPageLinkPreviewBuilder.Truncate(attributeValue, 1000);
		}

		protected static int GetStringLength(string text)
		{
			if (text == null)
			{
				return 0;
			}
			return text.Length;
		}

		private const string HtmlTitleRegExKey = "title";

		private const string DescriptionTagRegExKey = "descriptionTag";

		private const string DescriptionAttributeRegExKey = "description";

		private const string ImageTagRegExKey = "imageTag";

		private const string ImageAttributeRegExKey = "image";

		private const string HtmlTitleRegEx = "<title(?:>|\\s[^<]*?>)\\s*(?<title>.*?)\\s*</title>";

		private const string DescriptionTagRegEx = "<meta(?<descriptionTag>\\s[^<]*?(name=('|\")description\\2|(name|property)=('|\")og:description\\4)[^<]*?)>";

		private const string DescriptionAttributeRegEx = "\\scontent=('|\")(?<description>.*?)\\1";

		private const string ImageTagRegEx = "<meta(?<imageTag>\\s[^<]*?(property|name)=('|\")og:image\\2[^<]*?)>";

		private const string ImageAttributeRegEx = "\\scontent=('|\")(?<image>.*?)\\1";

		private const string LessThanEntityName = "&lt;";

		private const string LessThanEntityNumber = "&#60;";

		private const string GreaterThanEntityName = "&gt;";

		private const string GreaterThanEntityNumber = "&#62;";

		private const string Slash = "/";

		public static RegexOptions RegExOptions = RegexOptions.IgnoreCase | RegexOptions.Singleline;

		public static TimeSpan RegExTimeoutInterval = TimeSpan.FromMilliseconds(300.0);

		private static Regex GetHtmlTitleRegEx = new Regex("<title(?:>|\\s[^<]*?>)\\s*(?<title>.*?)\\s*</title>", WebPageLinkPreviewBuilder.RegExOptions, WebPageLinkPreviewBuilder.RegExTimeoutInterval);

		private static Regex GetDescriptionTagRegEx = new Regex("<meta(?<descriptionTag>\\s[^<]*?(name=('|\")description\\2|(name|property)=('|\")og:description\\4)[^<]*?)>", WebPageLinkPreviewBuilder.RegExOptions, WebPageLinkPreviewBuilder.RegExTimeoutInterval);

		private static Regex GetDescriptionAttributeRegEx = new Regex("\\scontent=('|\")(?<description>.*?)\\1", WebPageLinkPreviewBuilder.RegExOptions, WebPageLinkPreviewBuilder.RegExTimeoutInterval);

		private static Regex GetImageTagRegEx = new Regex("<meta(?<imageTag>\\s[^<]*?(property|name)=('|\")og:image\\2[^<]*?)>", WebPageLinkPreviewBuilder.RegExOptions, WebPageLinkPreviewBuilder.RegExTimeoutInterval);

		private static Regex GetImageAttributeRegEx = new Regex("\\scontent=('|\")(?<image>.*?)\\1", WebPageLinkPreviewBuilder.RegExOptions, WebPageLinkPreviewBuilder.RegExTimeoutInterval);

		private static string[] HtmlTagsToRemove = WebPageLinkPreviewBuilder.CreateHtmlTagsToRemove();

		protected string responseString;
	}
}
