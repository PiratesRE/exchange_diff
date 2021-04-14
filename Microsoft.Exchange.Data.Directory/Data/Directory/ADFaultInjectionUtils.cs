using System;
using System.DirectoryServices.Protocols;
using System.ServiceModel;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ADFaultInjectionUtils
	{
		public static Exception DirectoryTracerCallback(string exceptionType)
		{
			Exception result = null;
			if (!string.IsNullOrEmpty(exceptionType))
			{
				if (exceptionType.Contains("_") && !string.IsNullOrEmpty(exceptionType.Substring(exceptionType.IndexOf("_") + 1)))
				{
					string strA = exceptionType.Substring(0, exceptionType.IndexOf("_"));
					int errorCode = int.Parse(exceptionType.Substring(exceptionType.IndexOf("_") + 1));
					if (string.Compare(strA, "LdapException", StringComparison.OrdinalIgnoreCase) == 0)
					{
						result = new LdapException(errorCode);
					}
				}
				else
				{
					if (exceptionType == "ADTransientException-ExceptionNativeErrorWhenLookingForServersInDomain")
					{
						throw new ADTransientException(DirectoryStrings.ExceptionNativeErrorWhenLookingForServersInDomain(-1, "Fault Injection domain", "Fault injection Error"));
					}
					if (exceptionType == "ThreadAbortException")
					{
						Thread.CurrentThread.Abort();
					}
					else
					{
						if (exceptionType == "MServTransientException")
						{
							throw new MServTransientException(DirectoryStrings.TransientMservError("Fault injection MSERV transient error"));
						}
						if (exceptionType == "MServPermanentException")
						{
							throw new MServPermanentException(DirectoryStrings.PermanentMservError("Fault injection MSERV permanent error"));
						}
						if (exceptionType == "GlsTransientException")
						{
							throw new EndpointNotFoundException("Fault injection GLS transient error");
						}
						if (exceptionType == "GlsPermanentException")
						{
							throw new QuotaExceededException("Fault injection GLS permanent error");
						}
						if (exceptionType == "GlsDelay")
						{
							Thread.Sleep(4000);
							throw new TimeoutException("Test timeout exception");
						}
						if (exceptionType == "GlsAsyncTransientException")
						{
							throw new EndpointNotFoundException("Fault injection GLS Async transient error");
						}
						if (exceptionType == "GlsAsyncPermanentException")
						{
							throw new QuotaExceededException("Fault injection GLS Async permanent error");
						}
						if (exceptionType == "GlsAsyncDelay")
						{
							Thread.Sleep(4000);
							throw new TimeoutException("Async test timeout exception");
						}
						if (exceptionType == "DirectoryExceptionNotification")
						{
							throw new LdapException(91);
						}
					}
				}
			}
			return result;
		}

		public static Exception TopologyServiceTracerCallback(string exceptionType)
		{
			if (string.IsNullOrEmpty(exceptionType))
			{
				return null;
			}
			if (exceptionType.Equals("Exception") || exceptionType.Equals("UnhandledExceptionInLoop"))
			{
				throw new Exception("Fault Injection");
			}
			if (exceptionType.Equals("GrayException"))
			{
				throw new ArgumentNullException("Fault Injection");
			}
			return null;
		}
	}
}
