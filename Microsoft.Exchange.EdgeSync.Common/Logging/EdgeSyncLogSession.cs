using System;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.EdgeSync.Logging
{
	internal class EdgeSyncLogSession
	{
		internal EdgeSyncLogSession(EdgeSyncLog edgeSyncLog, LogRowFormatter row, EdgeSyncLoggingLevel loggingLevel)
		{
			this.edgeSyncLog = edgeSyncLog;
			this.row = row;
			this.loggingLevel = loggingLevel;
			this.row[2] = 0;
		}

		public EdgeSyncLoggingLevel LoggingLevel
		{
			get
			{
				return this.loggingLevel;
			}
			set
			{
				this.loggingLevel = value;
			}
		}

		public void LogService(string message)
		{
			this.LogEvent(EdgeSyncLoggingLevel.None, EdgeSyncEvent.Service, null, message);
		}

		public void LogConfiguration(string message)
		{
			this.LogEvent(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.Configuration, null, message);
		}

		public void LogLease(string message, string host)
		{
			this.LogEvent(EdgeSyncLoggingLevel.High, EdgeSyncEvent.SyncEngine, host, message);
		}

		public void LogLeaseTakeover(string oldLease, string newLease)
		{
			this.LogEvent(EdgeSyncLoggingLevel.Medium, EdgeSyncEvent.SyncEngine, string.Format(CultureInfo.InvariantCulture, "Old:{0} New:{1}", new object[]
			{
				oldLease,
				newLease
			}), "LeaseTakeover");
		}

		public void LogLeaseRefresh(string oldLease, string newLease)
		{
			this.LogEvent(EdgeSyncLoggingLevel.High, EdgeSyncEvent.SyncEngine, string.Format(CultureInfo.InvariantCulture, "Old:{0} New:{1}", new object[]
			{
				oldLease,
				newLease
			}), "LeaseRefresh");
		}

		public void LogLeaseHeld(string leaseHolder)
		{
			this.LogEvent(EdgeSyncLoggingLevel.High, EdgeSyncEvent.SyncEngine, "Can't take over because was held by " + leaseHolder, "LeaseHeld");
		}

		public void LogConnectFailure(string message, string username, string host, string hash)
		{
			this.LogEvent(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, host, message);
			this.LogEvent(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, username, "Username");
			this.LogEvent(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, hash, "Password Hash");
		}

		public void LogRenewal(string message, string username, string host, DateTime startUtcDate, string hash)
		{
			string data = string.Format(CultureInfo.InvariantCulture, "Host:{0}, UserName:{1}, StartUtcDate:{2}, PasswordHash:{3}", new object[]
			{
				host,
				username,
				startUtcDate.ToString(CultureInfo.InvariantCulture),
				hash
			});
			this.LogEvent(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.Credential, data, message);
		}

		public void LogFailedDirectTrust(string host, string message, X509Certificate2 cert)
		{
			this.LogEvent(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, cert.Subject, message);
			this.LogCertificate(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, cert);
		}

		public void LogCredentialDetail(string userName, DateTime startUtcDate)
		{
			string data = string.Format(CultureInfo.InvariantCulture, "User:{0}, StartUtcDate:{1}", new object[]
			{
				userName,
				startUtcDate.ToString(CultureInfo.InvariantCulture)
			});
			this.LogEvent(EdgeSyncLoggingLevel.High, EdgeSyncEvent.TargetConnection, data, "EdgeSync Credential used for connection");
		}

		public void LogTopology(string host, string message)
		{
			this.LogEvent(EdgeSyncLoggingLevel.Medium, EdgeSyncEvent.Topology, host, message);
		}

		public void LogRenewalException(string message, Exception exception, string host)
		{
			string data = string.Format(CultureInfo.InvariantCulture, "Host:{0}, Exception:{1}", new object[]
			{
				host,
				(exception != null) ? exception.Message : string.Empty
			});
			this.LogEvent(EdgeSyncLoggingLevel.None, EdgeSyncEvent.Credential, data, message);
		}

		public void LogSyncNow(string message)
		{
			this.LogEvent(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.Service, null, message);
		}

		public void LogCredential(string host, string message)
		{
			this.LogEvent(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, host, message);
		}

		public void LogProbe(string message, string username, string host, DateTime startUtcDate, string hash)
		{
			string data = string.Format(CultureInfo.InvariantCulture, "Host:{0}, UserName:{1}, StartUtcDate:{2}, PasswordHash:{3}", new object[]
			{
				host,
				username,
				startUtcDate.ToString(CultureInfo.InvariantCulture),
				hash
			});
			this.LogEvent(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.Credential, data, message);
		}

		public void LogException(EdgeSyncLoggingLevel level, EdgeSyncEvent edgeEvent, Exception exception, string context)
		{
			if (!this.CanLogEvent(level))
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(exception.Message.Length + exception.GetType().Name.Length + 3);
			stringBuilder.AppendFormat("{0} [{1}]", exception.Message, exception.GetType().Name);
			if (exception.InnerException != null)
			{
				stringBuilder.AppendFormat("; Inner Exception: {0} [{1}]", exception.InnerException.Message, exception.InnerException.GetType().Name);
			}
			this.LogEvent(level, edgeEvent, stringBuilder.ToString(), context);
		}

		public void LogCertificate(EdgeSyncLoggingLevel level, EdgeSyncEvent logEvent, X509Certificate2 cert)
		{
			if (cert == null)
			{
				return;
			}
			if (!this.CanLogEvent(level))
			{
				return;
			}
			this.LogEvent(level, logEvent, cert.Subject, "Certificate subject");
			this.LogEvent(level, logEvent, cert.IssuerName.Name, "Certificate issuer name");
			this.LogEvent(level, logEvent, cert.SerialNumber, "Certificate serial number");
			this.LogEvent(level, logEvent, cert.Thumbprint, "Certificate thumbprint");
			StringBuilder stringBuilder = new StringBuilder(256);
			foreach (string value in TlsCertificateInfo.GetFQDNs(cert))
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append(';');
				}
				stringBuilder.Append(value);
			}
			this.LogEvent(level, logEvent, stringBuilder.ToString(), "Certificate alternate names");
		}

		public void LogEvent(EdgeSyncLoggingLevel loggingLevel, EdgeSyncEvent eventId, string data, string context)
		{
			if (!this.CanLogEvent(loggingLevel))
			{
				return;
			}
			this.row[6] = eventId;
			this.row[9] = context;
			this.row[8] = data;
			this.row[7] = loggingLevel;
			this.edgeSyncLog.Append(this.row);
			this.row[2] = (int)this.row[2] + 1;
		}

		private bool CanLogEvent(EdgeSyncLoggingLevel loggingLevel)
		{
			return this.edgeSyncLog.Enabled && this.loggingLevel >= loggingLevel;
		}

		private EdgeSyncLog edgeSyncLog;

		private LogRowFormatter row;

		private EdgeSyncLoggingLevel loggingLevel;
	}
}
