using System;
using System.Net;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal abstract class RequestEventInspectorBase
	{
		internal abstract void Init();

		internal abstract void OnBeginRequest(object sender, EventArgs e, out bool stopExecution);

		internal abstract void OnPostAuthorizeRequest(object sender, EventArgs e);

		internal void OnPreRequestHandlerExecute(OwaContext owaContext)
		{
			if (owaContext.HttpContext.Handler is IRegistryOnlyForm && !owaContext.LoadedByFormsRegistry)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Form can not be accessed directly. url = {0}", owaContext.HttpContext.Request.Path);
				Utilities.EndResponse(owaContext.HttpContext, HttpStatusCode.BadRequest);
			}
		}

		internal abstract void OnEndRequest(OwaContext owaContext);
	}
}
