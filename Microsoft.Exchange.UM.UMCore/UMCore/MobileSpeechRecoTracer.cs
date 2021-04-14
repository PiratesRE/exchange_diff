using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class MobileSpeechRecoTracer
	{
		public static void TraceDebug(object context, Guid recoRequestId, string message, params object[] args)
		{
			using (new CallId(recoRequestId.ToString()))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.MobileSpeechRecoTracer, context, message, args);
			}
		}

		public static void TraceDebug(object context, Guid recoRequestId, PIIMessage pii, string message, params object[] args)
		{
			using (new CallId(recoRequestId.ToString()))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.MobileSpeechRecoTracer, context, pii, message, args);
			}
		}

		public static void TraceError(object context, Guid recoRequestId, string message, params object[] args)
		{
			using (new CallId(recoRequestId.ToString()))
			{
				CallIdTracer.TraceError(ExTraceGlobals.MobileSpeechRecoTracer, context, message, args);
			}
		}

		public static void TraceWarning(object context, Guid recoRequestId, string message, params object[] args)
		{
			using (new CallId(recoRequestId.ToString()))
			{
				CallIdTracer.TraceWarning(ExTraceGlobals.MobileSpeechRecoTracer, context, message, args);
			}
		}

		public static void TracePerformance(object context, Guid recoRequestId, string message, params object[] args)
		{
			using (new CallId(recoRequestId.ToString()))
			{
				CallIdTracer.TracePerformance(ExTraceGlobals.MobileSpeechRecoTracer, context, message, args);
			}
		}
	}
}
