using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.ClientAccess.Messages;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class RequestHandler : DisposableBase
	{
		internal static ResponseBase ProcessRequest(RequestBase request)
		{
			string @namespace = typeof(RequestHandler).Namespace;
			Type type = Type.GetType(@namespace + "." + request.GetType().Name + "Handler", false);
			ResponseBase result;
			using (RequestHandler requestHandler = (RequestHandler)Activator.CreateInstance(type))
			{
				result = requestHandler.Execute(request);
			}
			return result;
		}

		protected abstract ResponseBase Execute(RequestBase request);

		protected override void InternalDispose(bool disposing)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, null, "RequestHandler.InternalDispose called", new object[0]);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<RequestHandler>(this);
		}

		private const string HandlerSuffix = "Handler";
	}
}
