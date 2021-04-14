using System;
using System.Net;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core
{
	internal static class BailOut
	{
		internal static void SetHTTPStatusAndClose(HttpStatusCode statusCode)
		{
			ExTraceGlobals.ExceptionTracer.TraceDebug<HttpStatusCode>(0L, "[BailOut::SetHTTPStatusAndClose] Creating renderer for HTTP status code {0}.", statusCode);
			EWSSettings.ResponseRenderer = HttpResponseRenderer.Create(statusCode);
			throw new BailOutException();
		}
	}
}
