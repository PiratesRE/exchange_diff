using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.CertificateAuthentication;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Configuration.CertificateAuthentication
{
	public class CertificateAuthenticationFaultInjection
	{
		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (CertificateAuthenticationFaultInjection.faultInjectionTracer == null)
				{
					lock (CertificateAuthenticationFaultInjection.lockObject)
					{
						if (CertificateAuthenticationFaultInjection.faultInjectionTracer == null)
						{
							FaultInjectionTrace faultInjectionTrace = ExTraceGlobals.FaultInjectionTracer;
							faultInjectionTrace.RegisterExceptionInjectionCallback(new ExceptionInjectionCallback(CertificateAuthenticationFaultInjection.Callback));
							CertificateAuthenticationFaultInjection.faultInjectionTracer = faultInjectionTrace;
						}
					}
				}
				return CertificateAuthenticationFaultInjection.faultInjectionTracer;
			}
		}

		public static Exception Callback(string exceptionType)
		{
			Exception result = null;
			if (exceptionType != null)
			{
				if (typeof(ADTransientException).FullName.Equals(exceptionType))
				{
					return new ADTransientException(new LocalizedString("fault injection!"));
				}
				if (typeof(ApplicationException).FullName.Equals(exceptionType))
				{
					return new ApplicationException(new LocalizedString("fault injection!"));
				}
			}
			return result;
		}

		private static object lockObject = new object();

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
