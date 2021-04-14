using System;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal static class InstantMessageCertUtils
	{
		internal static void GetIMCertInfo(string thumbprint, out string certificateIssuer, out byte[] certificateSerial)
		{
			certificateIssuer = null;
			certificateSerial = null;
			X509Certificate2 x509Certificate = InstantMessageCertUtils.FindCertByThumbprint(ManageExchangeCertificate.UnifyThumbprintFormat(thumbprint));
			if (x509Certificate != null && InstantMessageCertUtils.IsValid(x509Certificate))
			{
				InstantMessageCertUtils.DoesCertificateExpireSoon(x509Certificate);
				certificateIssuer = x509Certificate.Issuer;
				certificateSerial = x509Certificate.GetSerialNumber();
			}
		}

		private static X509Certificate2 FindCertByThumbprint(string thumbprint)
		{
			X509Certificate2 x509Certificate = null;
			X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			try
			{
				x509Store.Open(OpenFlags.ReadOnly);
				X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
				x509Certificate = ((x509Certificate2Collection.Count > 0) ? x509Certificate2Collection[0] : null);
				if (x509Certificate == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<string>(0L, "IM Certificate with thumbprint {0} could not be found.", thumbprint);
					OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorIMCertificateNotFound, string.Empty, new object[]
					{
						thumbprint
					});
				}
			}
			catch (CryptographicException arg)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError<CryptographicException>(0L, "Cryptographic exception is thrown: {0}", arg);
			}
			catch (SecurityException arg2)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError<SecurityException>(0L, "Security exception is thrown: {0}", arg2);
			}
			catch (ArgumentException arg3)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError<ArgumentException>(0L, "Argument exception is thrown: {0}", arg3);
			}
			finally
			{
				x509Store.Close();
			}
			return x509Certificate;
		}

		private static bool IsExpired(X509Certificate2 cert)
		{
			bool result = false;
			ExDateTime exDateTime = new ExDateTime(ExTimeZone.CurrentTimeZone, cert.NotAfter);
			if (ExDateTime.Compare(exDateTime, ExDateTime.Now) <= 0)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "IM Certificate has expired.");
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorIMCertificateExpired, string.Empty, new object[]
				{
					exDateTime
				});
				result = true;
			}
			return result;
		}

		private static bool IsInvalidDate(X509Certificate2 cert)
		{
			bool result = false;
			ExDateTime exDateTime = new ExDateTime(ExTimeZone.CurrentTimeZone, cert.NotBefore);
			if (ExDateTime.Compare(exDateTime, ExDateTime.Now) >= 0)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "IM Certificate has not become valid yet.");
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorIMCertificateInvalidDate, string.Empty, new object[]
				{
					exDateTime
				});
				result = true;
			}
			return result;
		}

		private static bool HasPrivateKey(X509Certificate2 cert)
		{
			bool result = true;
			if (!cert.HasPrivateKey)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "IM Certificate does not have a private key.");
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorIMCertificateNoPrivateKey, string.Empty, new object[0]);
				result = false;
			}
			return result;
		}

		private static bool IsValid(X509Certificate2 cert)
		{
			bool result = false;
			if (cert.HasPrivateKey && !InstantMessageCertUtils.IsExpired(cert) && !InstantMessageCertUtils.IsInvalidDate(cert))
			{
				result = true;
			}
			return result;
		}

		private static void DoesCertificateExpireSoon(X509Certificate2 cert)
		{
			ExDateTime exDateTime = new ExDateTime(ExTimeZone.CurrentTimeZone, cert.NotAfter);
			TimeSpan threshold = new TimeSpan(30, 0, 0, 0);
			if (ExDateTime.Compare(exDateTime, ExDateTime.Now) >= 0 && ExDateTime.Compare(exDateTime, ExDateTime.Now, threshold) == 0)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "IM Certificate will expire soon.");
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorIMCertificateWillExpireSoon, string.Empty, new object[]
				{
					exDateTime
				});
			}
		}
	}
}
