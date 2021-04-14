using System;
using System.Web;
using Microsoft.Exchange.AirSync;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSyncHandler
{
	public class HandlerFactory : IHttpHandlerFactory
	{
		public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
		{
			Handler handler = new Handler();
			AirSyncDiagnostics.TraceDebug<int>(ExTraceGlobals.RequestsTracer, this, "IHttpHandlerFactory.GetHandler called. Handler {0} created.", handler.GetHashCode());
			return handler;
		}

		public void ReleaseHandler(IHttpHandler handler)
		{
			Handler handler2 = handler as Handler;
			if (handler2 == null)
			{
				throw new ArgumentNullException("handler");
			}
			AirSyncDiagnostics.TraceDebug<int>(ExTraceGlobals.RequestsTracer, this, "IHttpHandlerFactory.ReleaseHandler called on {0}.", handler2.GetHashCode());
		}
	}
}
