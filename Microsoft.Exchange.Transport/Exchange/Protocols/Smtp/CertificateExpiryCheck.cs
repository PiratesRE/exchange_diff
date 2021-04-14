using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class CertificateExpiryCheck
	{
		public static bool HasCertificateExpired(IX509Certificate2 certificate, DateTime utcNow)
		{
			ArgumentValidator.ThrowIfNull("certificate", certificate);
			return DateTime.Compare(utcNow, certificate.NotAfter) > 0;
		}

		public static bool IsCertificateExpiringSoon(IX509Certificate2 certificate, DateTime utcNow, TimeSpan? howSoon = null)
		{
			ArgumentValidator.ThrowIfNull("certificate", certificate);
			return DateTime.Compare(utcNow.Add((howSoon != null) ? howSoon.Value : CertificateExpiryCheck.ExpiryWarningTimeSpan), certificate.NotAfter) > 0;
		}

		public static void LogExpiredCertificate(IX509Certificate2 certificate, SmtpSessionCertificateUse certificateUse, string certificateFqdn, IExEventLog eventLog, DateTime utcNow)
		{
			ArgumentValidator.ThrowIfNull("certificate", certificate);
			ArgumentValidator.ThrowIfNull("eventLog", eventLog);
			switch (certificateUse)
			{
			case SmtpSessionCertificateUse.DirectTrust:
				eventLog.LogEvent(TransportEventLogConstants.Tuple_InternalTransportCertificateExpired, certificate.Thumbprint, new object[]
				{
					certificate.Thumbprint
				});
				return;
			case SmtpSessionCertificateUse.STARTTLS:
				eventLog.LogEvent(TransportEventLogConstants.Tuple_STARTTLSCertificateExpired, certificate.Thumbprint, new object[]
				{
					certificateFqdn,
					certificate.Thumbprint
				});
				return;
			case SmtpSessionCertificateUse.RemoteDirectTrust:
				eventLog.LogEvent(TransportEventLogConstants.Tuple_RemoteInternalTransportCertificateExpired, certificateFqdn, new object[]
				{
					certificateFqdn
				});
				return;
			case SmtpSessionCertificateUse.RemoteSTARTTLS:
				eventLog.LogEvent(TransportEventLogConstants.Tuple_RemoteSTARTTLSCertificateExpired, certificateFqdn, new object[]
				{
					certificateFqdn
				});
				return;
			default:
				return;
			}
		}

		public static void PublishLamNotificationForExpiredCertificate(IX509Certificate2 certificate, SmtpSessionCertificateUse certificateUse, string certificateFqdn)
		{
			ArgumentValidator.ThrowIfNull("certificate", certificate);
			if (certificateUse != SmtpSessionCertificateUse.STARTTLS)
			{
				return;
			}
			EventNotificationItem.PublishPeriodic(ExchangeComponent.Transport.Name, "STARTTLSCertficateExpired", null, string.Format("There is no valid SMTP Transport Layer Security (TLS) certificate for the FQDN of '{0}'.", certificateFqdn ?? string.Empty), "STARTTLSCertficateExpired", TimeSpan.FromMinutes(5.0), ResultSeverityLevel.Error, false);
		}

		public static void LogCertificateExpiringSoon(IX509Certificate2 certificate, SmtpSessionCertificateUse certificateUse, string certificateFqdn, IExEventLog eventLog, DateTime utcNow)
		{
			ArgumentValidator.ThrowIfNull("certificate", certificate);
			ArgumentValidator.ThrowIfNull("eventLog", eventLog);
			switch (certificateUse)
			{
			case SmtpSessionCertificateUse.DirectTrust:
				eventLog.LogEvent(TransportEventLogConstants.Tuple_InternalTransportCertificateExpiresSoon, certificate.Thumbprint, new object[]
				{
					certificate.Thumbprint,
					Util.CalculateHoursBetween(utcNow, certificate.NotAfter)
				});
				return;
			case SmtpSessionCertificateUse.STARTTLS:
				eventLog.LogEvent(TransportEventLogConstants.Tuple_STARTTLSCertificateExpiresSoon, certificate.Thumbprint, new object[]
				{
					certificateFqdn,
					certificate.Thumbprint,
					Util.CalculateHoursBetween(utcNow, certificate.NotAfter)
				});
				return;
			default:
				return;
			}
		}

		public static void PublishLamNotificationForCertificateExpiringSoon(IX509Certificate2 certificate, SmtpSessionCertificateUse certificateUse, string certificateFqdn, DateTime utcNow)
		{
			ArgumentValidator.ThrowIfNull("certificate", certificate);
			if (certificateUse != SmtpSessionCertificateUse.STARTTLS)
			{
				return;
			}
			EventNotificationItem.PublishPeriodic(ExchangeComponent.Transport.Name, "STARTTLSCertificateExpiresSoon", null, string.Format("The STARTTLS certificate will expire soon: subject: {0}, thumbprint: {1}, hours remaining: {2}.", certificateFqdn ?? string.Empty, certificate.Thumbprint, Util.CalculateHoursBetween(utcNow, certificate.NotAfter)), "STARTTLSCertificateExpiresSoon", TimeSpan.FromMinutes(5.0), ResultSeverityLevel.Error, false);
		}

		public static bool CheckCertificateExpiry(X509Certificate2 certificate, ExEventLog eventLogger, SmtpSessionCertificateUse use, string fqdn)
		{
			ArgumentValidator.ThrowIfNull("certificate", certificate);
			ArgumentValidator.ThrowIfNull("eventLogger", eventLogger);
			return CertificateExpiryCheck.CheckCertificateExpiry(new X509Certificate2Wrapper(certificate), new ExEventLogWrapper(eventLogger), use, fqdn, DateTime.UtcNow);
		}

		public static bool CheckCertificateExpiry(IX509Certificate2 certificate, IExEventLog eventLog, SmtpSessionCertificateUse use, string fqdn, DateTime utcNow)
		{
			ArgumentValidator.ThrowIfNull("certificate", certificate);
			ArgumentValidator.ThrowIfNull("eventLog", eventLog);
			if (CertificateExpiryCheck.HasCertificateExpired(certificate, utcNow))
			{
				CertificateExpiryCheck.LogExpiredCertificate(certificate, use, fqdn, eventLog, utcNow);
				CertificateExpiryCheck.PublishLamNotificationForExpiredCertificate(certificate, use, fqdn);
				return true;
			}
			if (CertificateExpiryCheck.IsCertificateExpiringSoon(certificate, utcNow, null))
			{
				CertificateExpiryCheck.LogCertificateExpiringSoon(certificate, use, fqdn, eventLog, utcNow);
				CertificateExpiryCheck.PublishLamNotificationForCertificateExpiringSoon(certificate, use, fqdn, utcNow);
			}
			return false;
		}

		private const string componentKeyStartTlsCertExpired = "STARTTLSCertficateExpired";

		private const string componentKeyStartTlsExpiresSoon = "STARTTLSCertificateExpiresSoon";

		public static readonly TimeSpan ExpiryWarningTimeSpan = TimeSpan.FromDays(90.0);
	}
}
