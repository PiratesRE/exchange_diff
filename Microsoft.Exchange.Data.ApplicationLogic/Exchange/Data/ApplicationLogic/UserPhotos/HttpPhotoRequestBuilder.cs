using System;
using System.Net;
using System.Web;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class HttpPhotoRequestBuilder
	{
		public HttpPhotoRequestBuilder(PhotosConfiguration configuration, ITracer upstreamTracer)
		{
			ArgumentValidator.ThrowIfNull("configuration", configuration);
			ArgumentValidator.ThrowIfNull("upstreamTracer", upstreamTracer);
			this.configuration = configuration;
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
		}

		public HttpWebRequest Build(Uri ewsUri, PhotoRequest request, IPhotoRequestOutboundWebProxyProvider proxyProvider, bool traceRequest)
		{
			ArgumentValidator.ThrowIfNull("ewsUri", ewsUri);
			ArgumentValidator.ThrowIfInvalidValue<Uri>("ewsUri", ewsUri, (Uri x) => !string.IsNullOrEmpty(x.AbsolutePath));
			ArgumentValidator.ThrowIfNull("request", request);
			ArgumentValidator.ThrowIfNull("proxyProvider", proxyProvider);
			Uri uri = this.CreateUri(ewsUri, request);
			this.tracer.TraceDebug<Uri>((long)this.GetHashCode(), "REQUEST BUILDER: request URI: {0}", uri);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
			httpWebRequest.Headers.Set(HttpRequestHeader.IfNoneMatch, request.ETag);
			PhotoRequestorHeader.Default.Serialize(request.Requestor, httpWebRequest);
			httpWebRequest.Proxy = proxyProvider.Create();
			if (request.Trace || traceRequest)
			{
				PhotosDiagnostics.Instance.StampGetUserPhotoTraceEnabledHeaders(httpWebRequest);
			}
			return httpWebRequest;
		}

		public PhotoRequest Parse(HttpRequest httpRequest, IPerformanceDataLogger perfLogger)
		{
			ArgumentValidator.ThrowIfNull("httpRequest", httpRequest);
			ArgumentValidator.ThrowIfNull("perfLogger", perfLogger);
			return new PhotoRequest
			{
				ETag = httpRequest.Headers["If-None-Match"],
				PerformanceLogger = perfLogger,
				Preview = this.GetPreviewValue(httpRequest),
				Size = this.GetRequestedPhotoSize(httpRequest),
				TargetSmtpAddress = this.GetRequestedIdentity(httpRequest),
				Requestor = this.DeserializeRequestorFromContext(httpRequest),
				HandlersToSkip = PhotosDiagnostics.Instance.GetHandlersToSkip(httpRequest),
				Trace = PhotosDiagnostics.Instance.ShouldTraceGetUserPhotoRequest(httpRequest)
			};
		}

		public Uri CreateUri(Uri ewsUri, string emailAddress)
		{
			ArgumentValidator.ThrowIfNull("ewsUri", ewsUri);
			return new UriBuilder(ewsUri)
			{
				Path = HttpPhotoRequestBuilder.RemoveTrailingSlash(ewsUri.AbsolutePath) + this.configuration.PhotoServiceEndpointRelativeToEwsWithLeadingSlash,
				Query = "email=" + emailAddress
			}.Uri;
		}

		private Uri CreateUri(Uri ewsUri, PhotoRequest request)
		{
			return new UriBuilder(ewsUri)
			{
				Path = HttpPhotoRequestBuilder.RemoveTrailingSlash(ewsUri.AbsolutePath) + this.configuration.PhotoServiceEndpointRelativeToEwsWithLeadingSlash,
				Query = string.Concat(new string[]
				{
					PhotoSizeArgumentStrings.Get(request.Size),
					"&email=",
					HttpPhotoRequestBuilder.GetTargetEmailAddress(request),
					HttpPhotoRequestBuilder.GetPreviewQueryStringParameter(request),
					HttpPhotoRequestBuilder.GetHandlersToSkipQueryStringParametersWithLeadingAmpersand(request)
				})
			}.Uri;
		}

		private bool GetPreviewValue(HttpRequest request)
		{
			string text = request.QueryString["isPreview"];
			if (text == null)
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "REQUEST BUILDER: photo preview not requested");
				return false;
			}
			this.tracer.TraceDebug<string>((long)this.GetHashCode(), "REQUEST BUILDER: photo preview request value {0}", text);
			return true;
		}

		private string GetRequestedIdentity(HttpRequest request)
		{
			return request.QueryString["email"];
		}

		private UserPhotoSize GetRequestedPhotoSize(HttpRequest request)
		{
			string text = request.QueryString["size"];
			if (string.IsNullOrEmpty(text))
			{
				this.tracer.TraceDebug<UserPhotoSize>((long)this.GetHashCode(), "REQUEST BUILDER: request did not specify a photo size.  Using default: {0}", UserPhotoSize.HR96x96);
				return UserPhotoSize.HR96x96;
			}
			UserPhotoSize result;
			if (!UserPhotoSizeUppercaseStrings.TryMapStringToSize(text, out result))
			{
				this.tracer.TraceDebug<string, UserPhotoSize>((long)this.GetHashCode(), "REQUEST BUILDER: photo size {0} not recognized.  Using default: {1}", text, UserPhotoSize.HR96x96);
				return UserPhotoSize.HR96x96;
			}
			return result;
		}

		private PhotoPrincipal DeserializeRequestorFromContext(HttpRequest request)
		{
			return PhotoRequestorHeader.Default.Deserialize(request, this.tracer);
		}

		private static string RemoveTrailingSlash(string path)
		{
			if (path.EndsWith("/", StringComparison.OrdinalIgnoreCase))
			{
				return path.Substring(0, path.Length - 1);
			}
			return path;
		}

		private static string GetPreviewQueryStringParameter(PhotoRequest request)
		{
			if (request.Preview)
			{
				return "&isPreview=true";
			}
			return string.Empty;
		}

		private static string GetTargetEmailAddress(PhotoRequest request)
		{
			if (!string.IsNullOrEmpty(request.TargetPrimarySmtpAddress))
			{
				return request.TargetPrimarySmtpAddress;
			}
			return request.TargetSmtpAddress;
		}

		private static string GetHandlersToSkipQueryStringParametersWithLeadingAmpersand(PhotoRequest request)
		{
			return PhotosDiagnostics.Instance.GetHandlersToSkipQueryStringParametersWithLeadingAmpersand(request);
		}

		internal const string SizeParameterName = "size";

		private const string PreviewParameterName = "isPreview";

		private const string IfNoneMatchHeader = "If-None-Match";

		private const UserPhotoSize DefaultPhotoSize = UserPhotoSize.HR96x96;

		private const string EmailParameterName = "email";

		private const string EmailParameterWithTrailingEqualsSign = "email=";

		private const string EmailParameterWithLeadingAmpersandAndTrailingEqualsSign = "&email=";

		private const string PreviewQueryParameterString = "&isPreview=true";

		private readonly ITracer tracer = ExTraceGlobals.UserPhotosTracer;

		private readonly PhotosConfiguration configuration;
	}
}
