using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Common;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.Authorization
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class CommonCertificateValidationCallbacks
	{
		internal static bool InternalServerToServer(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
				return true;
			}
			string text = CommonCertificateValidationCallbacks.ExtractUrlFromSenderIfTracingEnabled(sender);
			if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
			{
				CommonCertificateValidationCallbacks.Tracer.TraceDebug<string>((long)sender.GetHashCode(), "CommonCertificateValidationCallbacks::InternalServerToServer: accepting SSL certificate because the only error is invalid hostname.  URL: {0}", text);
				return true;
			}
			if (SslConfiguration.AllowInternalUntrustedCerts)
			{
				CommonCertificateValidationCallbacks.Tracer.TraceDebug<string, SslPolicyErrors, string>((long)sender.GetHashCode(), "CommonCertificateValidationCallbacks::InternalServerToServer: allowed SSL certificate {0} with error {1}.  URL: {2}", certificate.Subject, sslPolicyErrors, text);
				return true;
			}
			CommonCertificateValidationCallbacks.Tracer.TraceError<SslPolicyErrors, string>((long)sender.GetHashCode(), "CommonCertificateValidationCallbacks::InternalServerToServer: failed because SSL certificate contains the following errors: {0}.  URL: {1}", sslPolicyErrors, text);
			return false;
		}

		private static string ExtractUrlFromSenderIfTracingEnabled(object sender)
		{
			if (!CommonCertificateValidationCallbacks.IsDebugOrErrorTracingEnabled())
			{
				return string.Empty;
			}
			HttpWebRequest httpWebRequest = sender as HttpWebRequest;
			if (httpWebRequest != null)
			{
				if (httpWebRequest.RequestUri != null)
				{
					return httpWebRequest.RequestUri.AbsoluteUri;
				}
				return string.Empty;
			}
			else
			{
				SoapHttpClientProtocol soapHttpClientProtocol = sender as SoapHttpClientProtocol;
				if (soapHttpClientProtocol != null)
				{
					return soapHttpClientProtocol.Url;
				}
				return string.Empty;
			}
		}

		private static bool IsDebugOrErrorTracingEnabled()
		{
			return CommonCertificateValidationCallbacks.Tracer.IsTraceEnabled(TraceType.DebugTrace) || CommonCertificateValidationCallbacks.Tracer.IsTraceEnabled(TraceType.ErrorTrace);
		}

		private static readonly Trace Tracer = ExTraceGlobals.CertificateValidationTracer;
	}
}
