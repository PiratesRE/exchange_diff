using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;

namespace Microsoft.Exchange.Autodiscover
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

		public static Exception Callback(string exceptionType)
		{
			Exception result = null;
			if (exceptionType != null)
			{
				if (exceptionType.Equals("Microsoft.Exchange.Data.Directory.Recipient.NonUniqueRecipientException"))
				{
					result = new NonUniqueRecipientException("SomeSid", new ObjectValidationError(new LocalizedString("someSid"), null, "provider"));
				}
				else if (exceptionType.Equals("Microsoft.Exchange.Data.Directory.SuitabilityDirectoryException"))
				{
					result = new SuitabilityDirectoryException("FQDN", 1, "SuitabilityDirectoryException");
				}
				else if (exceptionType.Equals("System.SystemException"))
				{
					result = new SystemException("SomeSystemException");
				}
				else if (exceptionType.Equals("Microsoft.Exchange.Data.Directory.ADPossibleOperationException"))
				{
					result = new ADPossibleOperationException(new LocalizedString("ADPossibleOperationException"));
				}
			}
			return result;
		}

		internal enum LIDs : uint
		{
			ADExceptionsOnBudgetLookup = 2917543229U,
			AutodiscoverProxy = 2535861565U,
			AutodiscoverProxyForceLoopback = 2804297021U,
			AutodiscoverUseClientCertificateAuthentication = 4213583165U,
			AutodiscoverBasicAuthWindowsPrincipalMappingError = 4154862909U,
			AutodiscoverGetUserSettingForExternalUser = 2745576765U,
			ADExceptionsOnBudgetDispose = 4081462589U,
			AutoDiscoverMobileRedirectOptimizationOverrideOwaURL = 3866504509U
		}
	}
}
