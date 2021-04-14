using System;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;

namespace Microsoft.Exchange.HttpProxy
{
	internal class FaultInjection
	{
		public static void GenerateFault(FaultInjection.LIDs faultLid)
		{
			ExTraceGlobals.FaultInjectionTracer.TraceTest((uint)faultLid);
		}

		public static T TraceTest<T>(FaultInjection.LIDs faultLid)
		{
			T result = default(T);
			ExTraceGlobals.FaultInjectionTracer.TraceTest<T>((uint)faultLid, ref result);
			return result;
		}

		internal enum LIDs : uint
		{
			ShouldFailSmtpAnchorMailboxADLookup = 1378318050U,
			ProxyToLowerVersionEws = 2357603645U,
			ProxyToLowerVersionEwsOAuthIdentityActAsUserNullSid = 3431345469U,
			ExceptionDuringProxyDownLevelCheckNullSid_ChangeValue = 3548785981U,
			AnchorMailboxDatabaseCacheEntry = 4134939965U
		}
	}
}
