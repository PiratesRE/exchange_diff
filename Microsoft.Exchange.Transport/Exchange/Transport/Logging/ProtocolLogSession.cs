using System;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Transport.Logging
{
	internal class ProtocolLogSession : IProtocolLogSession
	{
		internal ProtocolLogSession(ProtocolLog protocolLog, ProtocolLogRowFormatter row, ProtocolLoggingLevel loggingLevel)
		{
			this.protocolLog = protocolLog;
			this.row = row;
			this.loggingLevel = loggingLevel;
			this.row[3] = 0;
		}

		public IPEndPoint LocalEndPoint
		{
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", "Cannot set local endpoint to null after initialization");
				}
				if (this.row[4] != null)
				{
					throw new InvalidOperationException("Cannot change local endpoint once it is set");
				}
				this.row[4] = value;
			}
		}

		public ProtocolLoggingLevel ProtocolLoggingLevel
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

		public void LogConnect()
		{
			this.LogEvent(ProtocolLoggingLevel.Verbose, ProtocolEvent.Connect, null, null);
		}

		public void LogDisconnect(DisconnectReason reason)
		{
			this.LogDisconnect(reason, null);
		}

		public void LogDisconnect(DisconnectReason reason, string remoteError)
		{
			string text = reason.ToString();
			if (!string.IsNullOrEmpty(remoteError))
			{
				text += string.Format("({0})", remoteError);
			}
			this.LogEvent(ProtocolLoggingLevel.Verbose, ProtocolEvent.Disconnect, null, text);
		}

		public void LogSend(byte[] data)
		{
			this.LogEvent(ProtocolLoggingLevel.Verbose, ProtocolEvent.Send, data, null);
		}

		public void LogReceive(byte[] data)
		{
			this.LogEvent(ProtocolLoggingLevel.Verbose, ProtocolEvent.Receive, data, null);
		}

		public void LogInformation(ProtocolLoggingLevel level, byte[] data, string formatString, params object[] parameterList)
		{
			if (this.loggingLevel < level)
			{
				return;
			}
			string context = string.Format(CultureInfo.InvariantCulture, formatString, parameterList);
			this.LogEvent(this.loggingLevel, ProtocolEvent.Information, data, context);
		}

		public void LogInformation(ProtocolLoggingLevel level, byte[] data, string context)
		{
			this.LogEvent(level, ProtocolEvent.Information, data, context);
		}

		public void LogCertificate(string type, IX509Certificate2 cert)
		{
			if (cert != null)
			{
				this.LogCertificate(type, cert.Certificate);
			}
		}

		public void LogCertificate(string type, X509Certificate2 cert)
		{
			if (cert == null)
			{
				return;
			}
			this.LogInformation(ProtocolLoggingLevel.Verbose, null, type);
			this.LogStringInformation(ProtocolLoggingLevel.Verbose, cert.Subject, "Certificate subject");
			this.LogStringInformation(ProtocolLoggingLevel.Verbose, cert.IssuerName.Name, "Certificate issuer name");
			this.LogStringInformation(ProtocolLoggingLevel.Verbose, cert.SerialNumber, "Certificate serial number");
			this.LogStringInformation(ProtocolLoggingLevel.Verbose, cert.Thumbprint, "Certificate thumbprint");
			StringBuilder stringBuilder = new StringBuilder(256);
			try
			{
				foreach (string value in TlsCertificateInfo.GetFQDNs(cert))
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(';');
					}
					stringBuilder.Append(value);
				}
			}
			catch (CryptographicException ex)
			{
				this.LogInformation(ProtocolLoggingLevel.Verbose, null, "CryptographicException was thrown while attempting to get certificate alternate names: {0}", new object[]
				{
					ex.Message
				});
			}
			this.LogStringInformation(ProtocolLoggingLevel.Verbose, stringBuilder.ToString(), "Certificate alternate names");
		}

		public void LogCertificateThumbprint(string type, IX509Certificate2 cert)
		{
			if (cert != null)
			{
				this.LogCertificateThumbprint(type, cert.Certificate);
			}
		}

		public void LogCertificateThumbprint(string type, X509Certificate2 cert)
		{
			if (cert == null)
			{
				return;
			}
			this.LogInformation(ProtocolLoggingLevel.Verbose, null, type);
			this.LogStringInformation(ProtocolLoggingLevel.Verbose, cert.Thumbprint, "Certificate thumbprint");
		}

		private static byte[] GetLine(byte[] buffer, int start)
		{
			int i = start;
			int num = -1;
			byte[] array = null;
			if (buffer == null)
			{
				return null;
			}
			while (i < buffer.Length)
			{
				i = ProtocolLogSession.IndexOf(buffer, 10, i);
				if (i == -1)
				{
					num = buffer.Length - start;
					break;
				}
				if (i > start && buffer[i - 1] == 13)
				{
					num = i - start + 1 - 2;
					break;
				}
				i++;
			}
			if (num > 0)
			{
				if (start == 0 && num == buffer.Length)
				{
					array = buffer;
				}
				else
				{
					array = new byte[num];
					Buffer.BlockCopy(buffer, start, array, 0, num);
				}
			}
			return array;
		}

		private static int IndexOf(byte[] buffer, byte val, int offset)
		{
			return ExBuffer.IndexOf(buffer, val, offset, buffer.Length - offset);
		}

		private void LogEvent(ProtocolLoggingLevel level, ProtocolEvent eventId, byte[] data, string context)
		{
			if (this.loggingLevel < level)
			{
				return;
			}
			lock (this.loggingLock)
			{
				this.row[6] = eventId;
				this.row[8] = context;
				int num = 0;
				byte[] line;
				do
				{
					line = ProtocolLogSession.GetLine(data, num);
					if (line != null)
					{
						num += line.Length + 2;
					}
					if (line != null || num == 0)
					{
						this.row[7] = line;
						this.protocolLog.Append(this.row);
						this.row[3] = (int)this.row[3] + 1;
					}
				}
				while (line != null);
				this.row[7] = null;
			}
		}

		private void LogStringInformation(ProtocolLoggingLevel level, string data, string context)
		{
			this.LogEvent(level, ProtocolEvent.Information, Encoding.UTF8.GetBytes(data), context);
		}

		private readonly ProtocolLog protocolLog;

		private readonly ProtocolLogRowFormatter row;

		private ProtocolLoggingLevel loggingLevel;

		private readonly object loggingLock = new object();
	}
}
