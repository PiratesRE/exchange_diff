using System;
using System.Net;
using System.Web;
using Microsoft.Exchange.Data.ApplicationLogic.UserPhotos;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Performance;
using Microsoft.Exchange.Services.Core;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class GetUserPhotoRequestDispatcher
	{
		public GetUserPhotoRequestDispatcher(ITracer tracer)
		{
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			this.tracer = tracer;
		}

		public DispatchStepResult Dispatch(RequestContext context)
		{
			if (context == null || context.HttpContext == null || context.HttpContext.Request == null || context.HttpContext.Response == null)
			{
				return DispatchStepResult.Continue;
			}
			if (!context.HttpContext.Request.Path.EndsWith("/GetUserPhoto", StringComparison.OrdinalIgnoreCase))
			{
				return DispatchStepResult.Continue;
			}
			HttpContext httpContext = new OwaPhotoRequestorWriter(NullPerformanceDataLogger.Instance, this.tracer).Write(context.HttpContext, context.HttpContext);
			if (!GetUserPhotoRequestDispatcher.IsRequestorInPhotoStackV2Flight(httpContext))
			{
				return DispatchStepResult.Continue;
			}
			DispatchStepResult result;
			try
			{
				new PhotoRequestHandler(OwaApplication.GetRequestDetailsLogger).ProcessRequest(httpContext);
				context.HttpStatusCode = (HttpStatusCode)context.HttpContext.Response.StatusCode;
				result = DispatchStepResult.EndResponseWithPrivateCaching;
			}
			catch (TooComplexPhotoRequestException arg)
			{
				this.tracer.TraceDebug<TooComplexPhotoRequestException>((long)this.GetHashCode(), "[GetUserPhotoRequestDispatcher::DispatchIfGetUserPhotoRequest] too complex photo request.  Exception: {0}", arg);
				result = DispatchStepResult.Continue;
			}
			return result;
		}

		private static bool IsRequestorInPhotoStackV2Flight(HttpContext request)
		{
			return new PhotoRequestorReader().EnabledInFasterPhotoFlight(request);
		}

		private const string GetUserPhotoRequestPathSuffix = "/GetUserPhoto";

		private readonly ITracer tracer = NullTracer.Instance;
	}
}
