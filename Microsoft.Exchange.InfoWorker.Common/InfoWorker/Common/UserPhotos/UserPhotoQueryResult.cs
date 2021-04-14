using System;
using System.Net;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.UserPhotos
{
	internal sealed class UserPhotoQueryResult : BaseQueryResult
	{
		public byte[] UserPhotoBytes { get; private set; }

		public string CacheId { get; private set; }

		public HttpStatusCode StatusCode { get; private set; }

		public string Expires { get; private set; }

		public bool IsPhotoServedFromADFallback { get; private set; }

		public string ContentType { get; private set; }

		internal UserPhotoQueryResult(byte[] image, string cacheId, HttpStatusCode code, string expires, string contentType, ITracer upstreamTracer)
		{
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
			this.UserPhotoBytes = ((image == null) ? Array<byte>.Empty : image);
			this.CacheId = cacheId;
			this.StatusCode = code;
			this.Expires = expires;
			this.IsPhotoServedFromADFallback = false;
			this.ContentType = contentType;
		}

		internal UserPhotoQueryResult(LocalizedException exception, ITracer upstreamTracer) : base(exception)
		{
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
			HttpStatusCode httpStatusCode = UserPhotoQueryResult.TranslateExceptionToHttpStatusCode(exception);
			this.tracer.TraceDebug<HttpStatusCode, string>((long)this.GetHashCode(), "Translated exception in query to HTTP {0}.  Exception: {1}", httpStatusCode, (exception != null) ? exception.Message : string.Empty);
			this.StatusCode = httpStatusCode;
			this.Expires = null;
			HttpStatusCode httpStatusCode2 = httpStatusCode;
			if (httpStatusCode2 <= HttpStatusCode.NotModified)
			{
				if (httpStatusCode2 == HttpStatusCode.OK || httpStatusCode2 == HttpStatusCode.NotModified)
				{
					return;
				}
			}
			else if (httpStatusCode2 != HttpStatusCode.NotFound && httpStatusCode2 != HttpStatusCode.InternalServerError)
			{
			}
			if (this.FallbackToADPhoto(exception))
			{
				return;
			}
			this.CacheId = null;
			this.UserPhotoBytes = Array<byte>.Empty;
		}

		private static HttpStatusCode TranslateExceptionToHttpStatusCode(Exception e)
		{
			if (e == null)
			{
				return HttpStatusCode.InternalServerError;
			}
			if (e is UserPhotoNotFoundException)
			{
				return HttpStatusCode.NotFound;
			}
			if (e is MailRecipientNotFoundException)
			{
				return HttpStatusCode.NotFound;
			}
			if (e is ProxyServerWithMinimumRequiredVersionNotFound)
			{
				return HttpStatusCode.NotFound;
			}
			if (e is AccessDeniedException)
			{
				return HttpStatusCode.Forbidden;
			}
			if (e is NoFreeBusyAccessException)
			{
				return HttpStatusCode.Forbidden;
			}
			if (e is WebException)
			{
				return UserPhotoQueryResult.GetHttpStatusCodeFromWebException((WebException)e);
			}
			if (e is ProxyWebRequestProcessingException)
			{
				return UserPhotoQueryResult.GetHttpStatusCodeFromProxyWebRequestProcessingException((ProxyWebRequestProcessingException)e);
			}
			return HttpStatusCode.InternalServerError;
		}

		private static HttpStatusCode GetHttpStatusCodeFromWebException(WebException e)
		{
			HttpWebResponse httpWebResponse = e.Response as HttpWebResponse;
			if (httpWebResponse == null)
			{
				return HttpStatusCode.InternalServerError;
			}
			return httpWebResponse.StatusCode;
		}

		private static HttpStatusCode GetHttpStatusCodeFromProxyWebRequestProcessingException(ProxyWebRequestProcessingException e)
		{
			if (e == null)
			{
				return HttpStatusCode.InternalServerError;
			}
			if (e.InnerException is WebException)
			{
				return UserPhotoQueryResult.GetHttpStatusCodeFromWebException((WebException)e.InnerException);
			}
			if (e.InnerException is AddressSpaceNotFoundException)
			{
				return HttpStatusCode.NotFound;
			}
			return HttpStatusCode.InternalServerError;
		}

		private bool FallbackToADPhoto(Exception e)
		{
			byte[] thumbnailPhoto = this.GetThumbnailPhoto(e);
			if (thumbnailPhoto == null || thumbnailPhoto.Length == 0)
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "AD photo fall-back not possible: no AD photo.");
				this.IsPhotoServedFromADFallback = false;
				return false;
			}
			this.tracer.TraceDebug((long)this.GetHashCode(), "AD photo fall-back successful.");
			this.UserPhotoBytes = thumbnailPhoto;
			this.CacheId = null;
			this.StatusCode = HttpStatusCode.OK;
			this.Expires = null;
			this.IsPhotoServedFromADFallback = true;
			return true;
		}

		private byte[] GetThumbnailPhoto(Exception e)
		{
			if (!e.Data.Contains("ThumbnailPhotoKey"))
			{
				return null;
			}
			return (byte[])e.Data["ThumbnailPhotoKey"];
		}

		private readonly ITracer tracer = ExTraceGlobals.UserPhotosTracer;
	}
}
