using System;
using System.Net;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal abstract class UcwaRequestFactory
	{
		public abstract string LandingPageToken { get; }

		internal virtual UcwaWebRequest CreateRequest(string httpMethod, string url)
		{
			return new UcwaWebRequest(this.CreateHttpWebRequest(httpMethod, url));
		}

		protected virtual HttpWebRequest CreateHttpWebRequest(string httpMethod, string url)
		{
			if (httpMethod == null)
			{
				throw new ArgumentNullException("httpMethod");
			}
			if (!UcwaHttpMethod.IsSupportedMethod(httpMethod))
			{
				throw new ArgumentOutOfRangeException("httpMethod");
			}
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			Uri uri = new Uri(url, UriKind.Absolute);
			if (!uri.IsAbsoluteUri)
			{
				throw new ArgumentException("Argument must be an absolute URI", "url");
			}
			HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
			if (httpWebRequest == null)
			{
				throw new InvalidOperationException("HttpWebRequest could not be created");
			}
			httpWebRequest.Accept = "application/xml";
			httpWebRequest.AllowAutoRedirect = true;
			httpWebRequest.MaximumAutomaticRedirections = 3;
			httpWebRequest.Method = UcwaHttpMethod.Normalize(httpMethod);
			httpWebRequest.Timeout = 30000;
			httpWebRequest.UserAgent = "UCWA Online Meeting Scheduler Library";
			if (UcwaHttpMethod.IsPatchMethod(httpMethod) || UcwaHttpMethod.IsPostMethod(httpMethod) || UcwaHttpMethod.IsPutMethod(httpMethod))
			{
				httpWebRequest.ContentType = "application/xml";
			}
			return httpWebRequest;
		}

		public const string UserAgentDefault = "UCWA Online Meeting Scheduler Library";

		private const int RequestTimeoutDefault = 30000;

		private const string ContentTypeDefault = "application/xml";

		private const int MaximumAutomaticRedirectionsDefault = 3;

		private const bool AllowAutoRedirectDefault = true;
	}
}
