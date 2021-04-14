using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Win32;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class ExchangeCertificateValidator
	{
		public static void Initialize()
		{
			if (ExchangeCertificateValidator.isInitialized)
			{
				return;
			}
			lock (ExchangeCertificateValidator.locker)
			{
				if (!ExchangeCertificateValidator.isInitialized)
				{
					using (RegistryKey registryKey = ExchangeCertificateValidator.SafeOpenKey())
					{
						if (registryKey != null)
						{
							object value = registryKey.GetValue("AllowInternalUntrustedCerts");
							if (value is int)
							{
								ExchangeCertificateValidator.allowInternalUntrustedCerts = ((int)value != 0);
							}
							value = registryKey.GetValue("AllowCertificateNameMismatchForMRS");
							if (value is string[])
							{
								string[] values = (string[])value;
								ExchangeCertificateValidator.AddToList(ExchangeCertificateValidator.TrustedSubjects, values);
							}
						}
					}
					string config = ConfigBase<MRSConfigSchema>.GetConfig<string>("ProxyClientTrustedCertificateThumbprints");
					if (!string.IsNullOrWhiteSpace(config))
					{
						ExchangeCertificateValidator.AddToList(ExchangeCertificateValidator.TrustedThumbprints, config.Split(new char[]
						{
							','
						}));
					}
					if (ExchangeCertificateValidator.allowInternalUntrustedCerts || ExchangeCertificateValidator.TrustedSubjects.Count > 0 || ExchangeCertificateValidator.TrustedThumbprints.Count > 0)
					{
						ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ExchangeCertificateValidator.CertificateValidatorCallback);
					}
					else
					{
						ServicePointManager.ServerCertificateValidationCallback = null;
					}
					ExchangeCertificateValidator.isInitialized = true;
				}
			}
		}

		public static bool CertificateValidatorCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
				return true;
			}
			if (ExchangeCertificateValidator.allowInternalUntrustedCerts)
			{
				return true;
			}
			if (certificate != null)
			{
				string item = certificate.Subject.ToLower(CultureInfo.InvariantCulture);
				if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch && ExchangeCertificateValidator.TrustedSubjects.Contains(item))
				{
					return true;
				}
				string certHashString = certificate.GetCertHashString();
				if (ExchangeCertificateValidator.TrustedThumbprints.Contains(certHashString.ToLower()))
				{
					return true;
				}
			}
			MrsTracer.ProxyClient.Debug("ExchangeCertificateValidator: PolicyErrors={0}, Subject={1}, Thumbprint={2}", new object[]
			{
				sslPolicyErrors,
				(certificate != null) ? certificate.Subject : "null",
				(certificate != null) ? certificate.GetCertHashString() : "null"
			});
			return false;
		}

		private static RegistryKey SafeOpenKey()
		{
			RegistryKey result = null;
			try
			{
				result = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA", false);
			}
			catch (SecurityException)
			{
			}
			catch (ArgumentException)
			{
			}
			return result;
		}

		private static void AddToList(HashSet<string> list, string[] values)
		{
			foreach (string text in values)
			{
				string text2 = text.ToLower(CultureInfo.InvariantCulture).Trim();
				if (!string.IsNullOrEmpty(text2) && !list.Contains(text2))
				{
					list.Add(text2);
				}
			}
		}

		private const string ContainerPath = "SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA";

		private const string KeyName = "AllowInternalUntrustedCerts";

		private const string AllowCertificateNameMismatchKeyName = "AllowCertificateNameMismatchForMRS";

		private static readonly object locker = new object();

		private static readonly HashSet<string> TrustedSubjects = new HashSet<string>();

		private static readonly HashSet<string> TrustedThumbprints = new HashSet<string>();

		private static bool isInitialized = false;

		private static bool allowInternalUntrustedCerts = false;
	}
}
