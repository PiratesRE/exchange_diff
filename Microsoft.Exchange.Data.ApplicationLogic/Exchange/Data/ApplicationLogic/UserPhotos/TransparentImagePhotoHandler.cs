using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TransparentImagePhotoHandler : IPhotoHandler
	{
		public TransparentImagePhotoHandler(PhotosConfiguration configuration, ITracer upstreamTracer)
		{
			ArgumentValidator.ThrowIfNull("configuration", configuration);
			ArgumentValidator.ThrowIfNull("upstreamTracer", upstreamTracer);
			this.configuration = configuration;
			this.tracer = upstreamTracer;
		}

		public PhotoResponse Retrieve(PhotoRequest request, PhotoResponse response)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			ArgumentValidator.ThrowIfNull("response", response);
			HttpStatusCode status = response.Status;
			if (status <= HttpStatusCode.NotModified)
			{
				if (status != HttpStatusCode.OK)
				{
					switch (status)
					{
					case HttpStatusCode.Found:
					case HttpStatusCode.NotModified:
						break;
					case HttpStatusCode.SeeOther:
						goto IL_5B;
					default:
						goto IL_5B;
					}
				}
				return response;
			}
			if (status != HttpStatusCode.NotFound && status != HttpStatusCode.InternalServerError)
			{
			}
			IL_5B:
			response.TransparentImageHandlerProcessed = true;
			this.tracer.TraceDebug((long)this.GetHashCode(), "TRANSPARENT IMAGE HANDLER: responding request with HTTP 200 OK and a transparent image.");
			response.Served = true;
			response.Status = HttpStatusCode.OK;
			response.HttpExpiresHeader = UserAgentPhotoExpiresHeader.Default.ComputeExpiresHeader(DateTime.UtcNow, HttpStatusCode.NotFound, this.configuration);
			response.ContentType = "image/gif";
			response.ContentLength = (long)TransparentImagePhotoHandler.Clear1x1GIF.Length;
			response.OutputPhotoStream.Write(TransparentImagePhotoHandler.Clear1x1GIF, 0, TransparentImagePhotoHandler.Clear1x1GIF.Length);
			return response;
		}

		public IPhotoHandler Then(IPhotoHandler next)
		{
			return new CompositePhotoHandler(this, next);
		}

		private static readonly byte[] Clear1x1GIF = new byte[]
		{
			71,
			73,
			70,
			56,
			57,
			97,
			1,
			0,
			1,
			0,
			128,
			0,
			0,
			0,
			0,
			0,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			33,
			249,
			4,
			1,
			0,
			0,
			1,
			0,
			44,
			0,
			0,
			0,
			0,
			1,
			0,
			1,
			0,
			0,
			2,
			1,
			76,
			0,
			59
		};

		private readonly PhotosConfiguration configuration;

		private readonly ITracer tracer;
	}
}
