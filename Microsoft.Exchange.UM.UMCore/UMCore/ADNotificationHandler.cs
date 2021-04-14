using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class ADNotificationHandler : DisposableBase
	{
		internal event ADNotificationEvent ConfigChanged;

		protected static void DebugTrace(string formatString, params object[] formatObjects)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, 0, formatString, formatObjects);
		}

		protected static void ErrorTrace(string formatString, params object[] formatObjects)
		{
			CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, 0, formatString, formatObjects);
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ADNotificationHandler>(this);
		}

		protected abstract void LogRegistrationError(TimeSpan retryInterval, LocalizedException exception);

		protected void FireConfigChangedEvent(ADNotificationEventArgs args)
		{
			if (this.ConfigChanged != null)
			{
				this.ConfigChanged(args);
			}
		}
	}
}
