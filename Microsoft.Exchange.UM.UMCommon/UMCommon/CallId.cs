using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class CallId : DisposableBase
	{
		internal CallId(string currentCallId)
		{
			if (CallId.id == null)
			{
				CallId.id = currentCallId;
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "Call-id has been set to {0}.", new object[]
				{
					CallId.id ?? "<null>"
				});
				return;
			}
			if (!string.Equals(CallId.id, currentCallId, StringComparison.InvariantCulture))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "Call-id already set to a different value, Existing call-id {0}, Required call-id {1}", new object[]
				{
					CallId.id ?? "<null>",
					currentCallId ?? "<null>"
				});
				throw new InvalidOperationException(Strings.CallIdNotNull(CallId.Id, currentCallId));
			}
			this.disposeCallId = false;
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "Call-id already set to the required value, Existing call-id {0}, Required call-id {1}", new object[]
			{
				CallId.id ?? "<null>",
				currentCallId ?? "<null>"
			});
		}

		internal static string Id
		{
			get
			{
				return CallId.id;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.disposeCallId)
			{
				CallId.id = null;
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "Call-id has been disposed.", new object[0]);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CallId>(this);
		}

		[ThreadStatic]
		private static string id;

		private bool disposeCallId = true;
	}
}
