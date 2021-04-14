using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Cryptography;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal static class CertificateUtils
	{
		internal static X509Certificate2 UMCertificate { get; set; }

		internal static X509Certificate2 FindCertByThumbprint(string thumbprint)
		{
			X509Certificate2 result = null;
			X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			x509Store.Open(OpenFlags.ReadOnly);
			try
			{
				X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
				result = ((x509Certificate2Collection.Count > 0) ? x509Certificate2Collection[0] : null);
			}
			finally
			{
				x509Store.Close();
			}
			return result;
		}

		internal static bool TryFindCertificateByThumbprint(string thumbprint, out X509Certificate2 cert)
		{
			cert = null;
			try
			{
				cert = CertificateUtils.FindCertByThumbprint(thumbprint);
				return cert != null;
			}
			catch (CryptographicException ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.UMCertificateTracer, 0, ex.ToString(), new object[0]);
			}
			catch (SecurityException ex2)
			{
				CallIdTracer.TraceError(ExTraceGlobals.UMCertificateTracer, 0, ex2.ToString(), new object[0]);
			}
			return false;
		}

		internal static X509Certificate2 GetCertificateByThumbprintOrServerCertificate(string thumbprint)
		{
			X509Certificate2 result = null;
			UMServer umserver;
			if (string.IsNullOrEmpty(thumbprint) && Utils.TryGetUMServerConfig(Utils.GetLocalHostFqdn(), out umserver))
			{
				thumbprint = umserver.UMCertificateThumbprint;
			}
			if (!string.IsNullOrEmpty(thumbprint))
			{
				CertificateUtils.TryFindCertificateByThumbprint(thumbprint, out result);
			}
			return result;
		}

		internal static void ExportCertToDiskFile(X509Certificate2 cert, string filePath)
		{
			using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
			{
				byte[] inArray = cert.Export(X509ContentType.Cert);
				string s = Convert.ToBase64String(inArray);
				byte[] bytes = Encoding.ASCII.GetBytes(s);
				fileStream.Write(bytes, 0, bytes.Length);
			}
		}

		internal static string GetSubjectFqdn(X509Certificate2 cert)
		{
			return CapiNativeMethods.GetCertNameInfo(cert, 0U, CapiNativeMethods.CertNameType.Attr);
		}

		internal static bool IsSelfSignedCertificate(X509Certificate2 cert)
		{
			return TlsCertificateInfo.IsSelfSignedCertificate(cert);
		}

		internal static bool IsExpired(X509Certificate2 cert)
		{
			ExDateTime dt = new ExDateTime(ExTimeZone.CurrentTimeZone, cert.NotAfter);
			return ExDateTime.Compare(dt, ExDateTime.Now) <= 0;
		}

		internal static TimeSpan TimeToExpire(X509Certificate2 cert)
		{
			ValidateArgument.NotNull(cert, "cert");
			ExDateTime exDateTime = new ExDateTime(ExTimeZone.CurrentTimeZone, cert.NotAfter);
			return exDateTime.Subtract(ExDateTime.Now);
		}

		internal static void CopyCertToRootStore(X509Certificate2 cert)
		{
			TlsCertificateInfo.TryInstallCertificateInTrustedRootCA(cert);
		}
	}
}
