using System;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal static class RouterCallHandler
	{
		public static void Handle(CafeRoutingContext context)
		{
			RouterCallHandler.InternalHandle(RouterCallHandler.callHandlers, context);
		}

		public static void HandleServiceRequest(CafeRoutingContext context)
		{
			RouterCallHandler.InternalHandle(RouterCallHandler.serviceHandlers, context);
		}

		private static void InternalHandle(ICallHandler[] handlers, CafeRoutingContext context)
		{
			ValidateArgument.NotNull(handlers, "handlers");
			ValidateArgument.NotNull(context, "context");
			for (int i = 0; i < handlers.Length; i++)
			{
				handlers[i].HandleCall(context);
				if (context.RedirectUri != null)
				{
					return;
				}
			}
			throw CallRejectedException.Create(Strings.CallCouldNotBeHandled(context.CallInfo.CallId, context.CallInfo.RemotePeer.ToString()), CallEndingReason.TransientError, null, new object[0]);
		}

		private static ICallHandler[] callHandlers = new ICallHandler[]
		{
			new ForestResolver(),
			new GatewayResolver(),
			new AccessProxyCallHandler(),
			new DialplanResolver(),
			new ReferredByCallHandler(),
			new DiversionResolver(),
			new AutoAttendantCallHandler(),
			new CallAnsweringCallHandler(),
			new SubscriberAccessCallHandler()
		};

		private static ICallHandler[] serviceHandlers = new ICallHandler[]
		{
			new ForestResolver(),
			new LyncServiceRequestHandler()
		};
	}
}
