using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal static class CertificateErrorHandler
	{
		public static bool CertValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
				CertificateErrorHandler.SecurityTracer.TraceDebug(0L, "{0}: No policy errors found in certificate.", new object[]
				{
					TraceContext.Get()
				});
				return true;
			}
			CertificateErrorHandler.SecurityTracer.TraceDebug<object, SslPolicyErrors>(0L, "{0}: SSL policy errors found in certificate: {1}", TraceContext.Get(), sslPolicyErrors);
			if (SslConfiguration.AllowExternalUntrustedCerts && SslConfiguration.AllowInternalUntrustedCerts)
			{
				CertificateErrorHandler.SecurityTracer.TraceDebug(0L, "{0}: Policy errors found in certificate, but configuration tells us to ignore all untrusted certs.", new object[]
				{
					TraceContext.Get()
				});
				return true;
			}
			string userAgentString = null;
			if (sender is Service)
			{
				Service service = (Service)sender;
				userAgentString = service.UserAgent;
			}
			else if (sender is HttpWebRequest)
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)sender;
				userAgentString = httpWebRequest.UserAgent;
			}
			UserAgent userAgent = UserAgent.Parse(userAgentString);
			if (userAgent == null)
			{
				CertificateErrorHandler.SecurityTracer.TraceDebug(0L, "{0}: User agent string is unrecognizable. Cannot ignore certificate errors.", new object[]
				{
					TraceContext.Get()
				});
				return false;
			}
			return CertificateErrorHandler.CanIgnoreCertificateError(userAgent);
		}

		internal static bool CanIgnoreCertificateError(UserAgent userAgent)
		{
			if (string.Equals(userAgent.Category, "ASProxy"))
			{
				if (string.Equals(userAgent.Type, "CrossSite") && SslConfiguration.AllowInternalUntrustedCerts)
				{
					CertificateErrorHandler.SecurityTracer.TraceDebug<object, UserAgent>(0L, "{0}: Cert errors for cross-site request {1} are being ignored.", TraceContext.Get(), userAgent);
					return true;
				}
				if (string.Equals(userAgent.Type, "CrossForest") && string.Equals(userAgent.Protocol, "EXCH") && SslConfiguration.AllowInternalUntrustedCerts)
				{
					CertificateErrorHandler.SecurityTracer.TraceDebug<object, UserAgent>(0L, "{0}: Cert errors for cross-forest request {1} are being ignored.", TraceContext.Get(), userAgent);
					return true;
				}
			}
			if (string.Equals(userAgent.Category, "ASAutoDiscover") && string.Equals(userAgent.Source, "Directory") && SslConfiguration.AllowInternalUntrustedCerts)
			{
				CertificateErrorHandler.SecurityTracer.TraceDebug<object, UserAgent>(0L, "{0}: Cert errors for autodiscover request {1} are being ignored.", TraceContext.Get(), userAgent);
				return true;
			}
			if (SslConfiguration.AllowExternalUntrustedCerts)
			{
				CertificateErrorHandler.SecurityTracer.TraceDebug<object, UserAgent>(0L, "{0}: Request {1} is deemed external and AllowExternalUntrustedCerts is true. Ignoring cert errors.", TraceContext.Get(), userAgent);
				return true;
			}
			CertificateErrorHandler.SecurityTracer.TraceError<object, UserAgent>(0L, "{0}: Cert errors for request {1} can not be ignored. Failing the request.", TraceContext.Get(), userAgent);
			return false;
		}

		private static readonly Trace SecurityTracer = ExTraceGlobals.SecurityTracer;
	}
}
