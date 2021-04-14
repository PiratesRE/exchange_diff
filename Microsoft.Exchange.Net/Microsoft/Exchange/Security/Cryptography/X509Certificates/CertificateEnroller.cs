using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CertEnroll;
using Microsoft.Exchange.Diagnostics.Components.SecurityCommon;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal static class CertificateEnroller
	{
		public static string ReadPkcs10Request(X509Certificate2 cert)
		{
			if (cert == null)
			{
				return string.Empty;
			}
			uint num = 0U;
			string result;
			using (SafeCertContextHandle safeCertContextHandle = SafeCertContextHandle.Clone(cert.Handle))
			{
				if (!CapiNativeMethods.CertGetCertificateContextProperty(safeCertContextHandle, CapiNativeMethods.CertificatePropertyId.XEnrollmentRequest, SafeHGlobalHandle.InvalidHandle, ref num))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (lastWin32Error != -2146885628)
					{
						throw new CryptographicException(lastWin32Error);
					}
					CertificateEnroller.TraceError("CertGetCertificateContextProperty returned an error: {0}", new object[]
					{
						lastWin32Error
					});
					result = string.Empty;
				}
				else
				{
					SafeHGlobalHandle safeHGlobalHandle = NativeMethods.AllocHGlobal((int)num);
					if (!CapiNativeMethods.CertGetCertificateContextProperty(safeCertContextHandle, CapiNativeMethods.CertificatePropertyId.XEnrollmentRequest, safeHGlobalHandle, ref num))
					{
						throw new CryptographicException(Marshal.GetLastWin32Error());
					}
					result = Marshal.PtrToStringAnsi(safeHGlobalHandle.DangerousGetHandle(), (int)num);
				}
			}
			return result;
		}

		public static string ToBase64String(byte[] bytes)
		{
			string result;
			foreach (byte b in bytes)
			{
				if (b >= 127)
				{
					result = Convert.ToBase64String(bytes);
					return result;
				}
			}
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				using (StreamReader streamReader = new StreamReader(memoryStream))
				{
					result = streamReader.ReadToEnd();
				}
			}
			return result;
		}

		public static string GeneratePkcs10Request(CertificateRequestInfo info, out string thumbprint)
		{
			info.ValidateDomainNamesAndSetSubject();
			IX509CertificateRequest ix509CertificateRequest = CertificateEnroller.ConvertRequest(info);
			IX509Enrollment ix509Enrollment = new CX509EnrollmentClass();
			ix509Enrollment.InitializeFromRequest(ix509CertificateRequest);
			ix509Enrollment.CertificateFriendlyName = info.FriendlyName;
			string text = ix509Enrollment.CreateRequest(1);
			IX509CertificateRequestPkcs10 ix509CertificateRequestPkcs = (IX509CertificateRequestPkcs10)ix509Enrollment.Request.GetInnerRequest(0);
			string identifier = ix509CertificateRequestPkcs.PublicKey.ComputeKeyIdentifier(1, 1073741828);
			thumbprint = CertificateEnroller.WritePkcs10Request(identifier, X509FindType.FindBySubjectKeyIdentifier, text);
			return text;
		}

		public static bool TryAcceptPkcs7(string data, out string thumbprint, out bool untrustedRoot)
		{
			IX509Enrollment ix509Enrollment = new CX509EnrollmentClass();
			untrustedRoot = false;
			try
			{
				ix509Enrollment.Initialize(2);
				ix509Enrollment.InstallResponse(4, data, 7, null);
			}
			catch (COMException ex)
			{
				CertificateEnroller.TraceException(ex);
				if (ex.ErrorCode == -2146762487)
				{
					untrustedRoot = true;
				}
				thumbprint = null;
				return false;
			}
			try
			{
				X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
				x509Certificate2Collection.Import(Convert.FromBase64String(data));
				thumbprint = CertificateEnroller.FindImportedCertificate(x509Certificate2Collection);
				if (thumbprint == null)
				{
					return false;
				}
				return true;
			}
			catch (FormatException e)
			{
				CertificateEnroller.TraceException(e);
			}
			catch (CryptographicException e2)
			{
				CertificateEnroller.TraceException(e2);
			}
			thumbprint = null;
			return false;
		}

		private static string FindImportedCertificate(X509Certificate2Collection certs)
		{
			if (certs.Count == 1)
			{
				return certs[0].Thumbprint;
			}
			foreach (X509Certificate2 x509Certificate in certs)
			{
				foreach (X509Extension x509Extension in x509Certificate.Extensions)
				{
					X509KeyUsageExtension x509KeyUsageExtension = x509Extension as X509KeyUsageExtension;
					if (x509KeyUsageExtension != null && (x509KeyUsageExtension.KeyUsages & X509KeyUsageFlags.KeyCertSign) == X509KeyUsageFlags.None)
					{
						return x509Certificate.Thumbprint;
					}
				}
			}
			return null;
		}

		private static string WritePkcs10Request(string identifier, X509FindType identifierType, string data)
		{
			X509Store x509Store = new X509Store("REQUEST", StoreLocation.LocalMachine);
			string result;
			try
			{
				x509Store.Open(OpenFlags.ReadWrite | OpenFlags.OpenExistingOnly | OpenFlags.IncludeArchived);
				X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(identifierType, identifier, false);
				if (x509Certificate2Collection.Count > 0)
				{
					X509Certificate2 x509Certificate = x509Certificate2Collection[0];
					byte[] array = new byte[data.Length];
					for (int i = 0; i < data.Length; i++)
					{
						array[i] = (byte)data[i];
					}
					using (SafeHGlobalHandle safeHGlobalHandle = NativeMethods.AllocHGlobal(array.Length))
					{
						Marshal.Copy(array, 0, safeHGlobalHandle.DangerousGetHandle(), array.Length);
						CapiNativeMethods.CryptoApiBlob cryptoApiBlob = new CapiNativeMethods.CryptoApiBlob((uint)array.Length, safeHGlobalHandle);
						using (SafeCertContextHandle safeCertContextHandle = SafeCertContextHandle.Clone(x509Certificate.Handle))
						{
							CapiNativeMethods.CertSetCertificateContextProperty(safeCertContextHandle, CapiNativeMethods.CertificatePropertyId.XEnrollmentRequest, 0U, ref cryptoApiBlob);
						}
					}
					result = x509Certificate.Thumbprint;
				}
				else
				{
					CertificateEnroller.TraceError("Certificate with identifier {0} not found.", new object[]
					{
						identifier
					});
					result = string.Empty;
				}
			}
			finally
			{
				x509Store.Close();
			}
			return result;
		}

		private static void TraceException(Exception e)
		{
			ExTraceGlobals.CertificateEnrollmentTracer.TraceError<Exception>(0L, "An exception was caught", e);
		}

		private static void TraceError(string s, params object[] args)
		{
			ExTraceGlobals.CertificateEnrollmentTracer.TraceError(0L, s, args);
		}

		private static IX509CertificateRequest ConvertRequest(CertificateRequestInfo info)
		{
			IX509CertificateRequestPkcs10 ix509CertificateRequestPkcs = new CX509CertificateRequestPkcs10Class();
			ix509CertificateRequestPkcs.Initialize(2);
			ix509CertificateRequestPkcs.SmimeCapabilities = false;
			ix509CertificateRequestPkcs.Subject = new CX500DistinguishedNameClass();
			ix509CertificateRequestPkcs.Subject.Encode(info.Subject.Name, 3);
			if (info.SourceProvider == CertificateCreationOption.RSAProvider)
			{
				ix509CertificateRequestPkcs.PrivateKey.KeySpec = 1;
				ix509CertificateRequestPkcs.PrivateKey.ProviderType = 12;
			}
			else
			{
				ix509CertificateRequestPkcs.PrivateKey.KeySpec = 2;
				ix509CertificateRequestPkcs.PrivateKey.ProviderType = 13;
			}
			ix509CertificateRequestPkcs.PrivateKey.ExportPolicy = (info.IsExportable ? 1 : 0);
			ix509CertificateRequestPkcs.PrivateKey.Length = info.KeySize;
			foreach (X509Extension x509Extension in info.GetExtensions())
			{
				CX509Extension cx509Extension = new CX509ExtensionClass();
				CObjectId cobjectId = new CObjectIdClass();
				cobjectId.InitializeFromValue(x509Extension.Oid.Value);
				string text = Convert.ToBase64String(x509Extension.RawData);
				cx509Extension.Initialize(cobjectId, 1, text);
				ix509CertificateRequestPkcs.X509Extensions.Add(cx509Extension);
			}
			return ix509CertificateRequestPkcs;
		}
	}
}
