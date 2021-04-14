using System;
using Microsoft.Exchange.Diagnostics.Components.Security;

namespace Microsoft.Exchange.Security
{
	internal class FaultInjection
	{
		public static T TraceTest<T>(FaultInjection.LIDs faultLid)
		{
			T result = default(T);
			ExTraceGlobals.FaultInjectionTracer.TraceTest<T>((uint)faultLid, ref result);
			return result;
		}

		internal enum LIDs : uint
		{
			SerializedOAuthIdentity_ChangeValue = 2697342269U,
			OAuthAuthenticationRequest_SleepTime = 2177248573U,
			X509CertSubject_ChangeValue = 2634427709U,
			EnforceOAuthCommonAccessTokenVersion1_ChangeValue = 3481677117U,
			ExceptionDuringOAuthCATGeneration_ChangeValue = 3011915069U,
			ExceptionDuringRehydration_ChangeValue = 4085656893U
		}
	}
}
