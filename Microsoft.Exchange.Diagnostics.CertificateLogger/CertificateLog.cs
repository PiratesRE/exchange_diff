using System;
using Microsoft.Exchange.LogAnalyzer.Extensions.CertificateLog;

namespace Microsoft.Exchange.Diagnostics.CertificateLogger
{
	public class CertificateLog : IDisposable
	{
		public CertificateLog(string logDirectory, long maxDirectorySize, long maxFileSize, int maxBufferSize, TimeSpan flushInterval)
		{
			this.logSchema = new LogSchema("Microsoft Exchange Server", "15.0.0.0", "Certificate Log", CertificateInformation.GetColumnHeaders());
			LogHeaderFormatter headerFormatter = new LogHeaderFormatter(this.logSchema, LogHeaderCsvOption.CsvStrict);
			this.exlog = new Log(CertificateLog.LogFilePrefix, headerFormatter, "Certificate Log");
			this.exlog.Configure(logDirectory, TimeSpan.Zero, maxDirectorySize, maxFileSize, maxBufferSize, flushInterval, true);
		}

		public void Dispose()
		{
			if (this.exlog != null)
			{
				this.exlog.Close();
			}
		}

		public void LogCertificateInformation(CertificateInformation info)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			string value = (info.RootCertificate == null) ? string.Empty : info.RootCertificate.Format(false);
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.logSchema);
			logRowFormatter[1] = Environment.MachineName;
			logRowFormatter[2] = info.StoreName;
			logRowFormatter[3] = info.Version;
			logRowFormatter[4] = info.Subject.Format(false);
			logRowFormatter[5] = info.Issuer.Format(false);
			logRowFormatter[6] = info.ValidFrom;
			logRowFormatter[7] = info.ValidTo;
			logRowFormatter[8] = info.Thumbprint;
			logRowFormatter[9] = info.SignatureAlgorithm;
			logRowFormatter[10] = info.PublicKeySize;
			logRowFormatter[11] = info.SerialNumber;
			logRowFormatter[12] = value;
			logRowFormatter[13] = info.SubjectKeyIdentifier;
			logRowFormatter[14] = info.BasicConstraints;
			logRowFormatter[15] = info.KeyUsage;
			logRowFormatter[16] = info.EnhancedKeyUsage;
			logRowFormatter[17] = info.ComponentOwner;
			this.exlog.Append(logRowFormatter, 0);
		}

		private const string ComponentName = "Certificate Log";

		private static readonly string LogFilePrefix = string.Format("{0}_{1}_", Environment.MachineName, "CertificateLog");

		private readonly Log exlog;

		private readonly LogSchema logSchema;
	}
}
