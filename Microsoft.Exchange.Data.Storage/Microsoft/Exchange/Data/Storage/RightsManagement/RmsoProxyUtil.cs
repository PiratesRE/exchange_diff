using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RmsoProxyUtil
	{
		public static Uri OriginalCertServerUrl
		{
			get
			{
				return RmsoProxyUtil.originalCertServerUrl;
			}
		}

		public static Uri OriginalLicenseServerUrl
		{
			get
			{
				return RmsoProxyUtil.originalLicenseServerUrl;
			}
		}

		public static Uri GetCertificationServerRedirectUrl(Uri serviceUrl)
		{
			if (serviceUrl == null)
			{
				return null;
			}
			if (serviceUrl != RmsoProxyUtil.originalCertServerUrl)
			{
				RmsoProxyUtil.originalCertServerUrl = serviceUrl;
			}
			else if (DateTime.UtcNow < RmsoProxyUtil.certServerUrlExpirationTimeUTC)
			{
				return RmsoProxyUtil.checkedCertServerUrl;
			}
			RmsoProxyUtil.checkedCertServerUrl = RmsoProxyUtil.CheckRedirectUrl(serviceUrl, true);
			RmsoProxyUtil.certServerUrlExpirationTimeUTC = DateTime.UtcNow.AddSeconds(300.0);
			return RmsoProxyUtil.checkedCertServerUrl;
		}

		public static Uri GetLicenseServerRedirectUrl(Uri licenseUrl)
		{
			if (licenseUrl == null)
			{
				return null;
			}
			if (licenseUrl != RmsoProxyUtil.originalLicenseServerUrl)
			{
				RmsoProxyUtil.originalLicenseServerUrl = licenseUrl;
			}
			else if (DateTime.UtcNow < RmsoProxyUtil.licenseUrlExpirationTimeUTC)
			{
				return RmsoProxyUtil.checkedLicenseServerUrl;
			}
			RmsoProxyUtil.checkedLicenseServerUrl = RmsoProxyUtil.CheckRedirectUrl(licenseUrl, false);
			RmsoProxyUtil.licenseUrlExpirationTimeUTC = DateTime.UtcNow.AddSeconds(300.0);
			return RmsoProxyUtil.checkedLicenseServerUrl;
		}

		private static Uri CheckRedirectUrl(Uri originalUrl, bool isCertificationServerUrl)
		{
			Uri uri = originalUrl;
			string text = uri.AbsoluteUri;
			string name = isCertificationServerUrl ? "SOFTWARE\\Microsoft\\ExchangeServer\\V15\\IRM\\CertificationServerRedirection\\" : "SOFTWARE\\Microsoft\\ExchangeServer\\V15\\IRM\\LicenseServerRedirection\\";
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name))
			{
				if (registryKey != null)
				{
					string[] valueNames = registryKey.GetValueNames();
					if (valueNames == null || valueNames.Length == 0)
					{
						return originalUrl;
					}
					foreach (string text2 in valueNames)
					{
						if (!string.IsNullOrWhiteSpace(text2) && registryKey.GetValueKind(text2) == RegistryValueKind.String && text.ToLower().Contains(text2.ToLower()))
						{
							text = text.ToLower().Replace(text2.ToLower(), ((string)registryKey.GetValue(text2)).ToLower());
							try
							{
								uri = new Uri(text);
								break;
							}
							catch (UriFormatException ex)
							{
								RmsClientManagerLog.LogUrlMalFormatException(ex, text2, originalUrl.AbsoluteUri);
								RmsoProxyUtil.Tracer.TraceError(0L, ex.Message);
								throw ex;
							}
						}
					}
				}
			}
			return uri;
		}

		private const string CertficationServerRedirectionPath = "SOFTWARE\\Microsoft\\ExchangeServer\\V15\\IRM\\CertificationServerRedirection\\";

		private const string LicenseServerRedirectionPath = "SOFTWARE\\Microsoft\\ExchangeServer\\V15\\IRM\\LicenseServerRedirection\\";

		private const int DefaultUrlCacheTimeSpanInSeconds = 300;

		private static Uri originalCertServerUrl;

		private static Uri checkedCertServerUrl;

		private static DateTime certServerUrlExpirationTimeUTC;

		private static Uri originalLicenseServerUrl;

		private static Uri checkedLicenseServerUrl;

		private static DateTime licenseUrlExpirationTimeUTC;

		private static readonly Trace Tracer = ExTraceGlobals.RightsManagementTracer;
	}
}
