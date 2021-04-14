using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Services;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class LinkPreviewBuilder
	{
		public LinkPreviewBuilder(GetLinkPreviewRequest request, RequestDetailsLogger logger, Uri responseUri, bool isVideo)
		{
			this.id = request.Id;
			this.url = request.Url;
			this.requestStartTimeMilliseconds = request.RequestStartTimeMilliseconds;
			this.logger = logger;
			this.responseUri = responseUri;
			this.isVideo = isVideo;
		}

		internal abstract GetLinkPreviewResponse Execute();

		protected bool IsVideo
		{
			get
			{
				return this.isVideo;
			}
		}

		protected virtual LinkPreview CreateLinkPreviewInstance()
		{
			return new LinkPreview();
		}

		public static Match ExecuteRegEx(Regex regEx, string matchString, string propertyName)
		{
			Match result = null;
			try
			{
				result = regEx.Match(matchString);
			}
			catch (RegexMatchTimeoutException)
			{
				GetLinkPreview.ThrowInvalidRequestException("RegExTimeout", string.Format("The regex timed out on property {0}.", propertyName));
			}
			return result;
		}

		protected static MatchCollection ExecuteRegExForMatchCollection(Regex regEx, string matchString, string propertyName)
		{
			MatchCollection result = null;
			try
			{
				result = regEx.Matches(matchString);
			}
			catch (RegexMatchTimeoutException)
			{
				GetLinkPreview.ThrowInvalidRequestException("RegExTimeout", string.Format("The regex timed out on property {0}.", propertyName));
			}
			return result;
		}

		protected static string ConvertToSafeHtml(string html)
		{
			string text = null;
			if (html != null)
			{
				HtmlToHtml htmlToHtml = new HtmlToHtml();
				htmlToHtml.FilterHtml = true;
				htmlToHtml.OutputHtmlFragment = true;
				using (TextReader textReader = new StringReader(html))
				{
					using (TextWriter textWriter = new StringWriter())
					{
						try
						{
							htmlToHtml.Convert(textReader, textWriter);
							text = textWriter.ToString().Trim();
							if (text.StartsWith("<div>", StringComparison.OrdinalIgnoreCase))
							{
								text = text.Substring("<div>".Length, text.Length - "<div>".Length - "</div>".Length);
							}
						}
						catch (ExchangeDataException localizedException)
						{
							GetLinkPreview.ThrowLocalizedException("HtmlConversionFailed", localizedException);
						}
					}
				}
			}
			return text;
		}

		protected const string TitlePropertyName = "title";

		protected const string DescriptionPropertyName = "description";

		protected const string ImagePropertyName = "image";

		protected const int TitleTruncationLength = 400;

		protected const int DescriptionTruncationLength = 1000;

		protected const int MaxImageUrlLength = 500;

		private readonly bool isVideo;

		protected readonly string id;

		protected readonly string url;

		protected readonly long requestStartTimeMilliseconds;

		protected readonly RequestDetailsLogger logger;

		protected readonly Uri responseUri;
	}
}
