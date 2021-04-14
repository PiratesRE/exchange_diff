using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Net.Logging
{
	internal class ProtocolLogSession
	{
		internal ProtocolLogSession(ProtocolLog protocolLog, LogRowFormatter row)
		{
			this.protocolLog = protocolLog;
			this.row = row;
		}

		public void LogConnect(string context)
		{
			this.LogEvent(ProtocolLoggingLevel.All, ProtocolEvent.Connect, null, context);
		}

		public void LogDisconnect(string reason)
		{
			this.LogEvent(ProtocolLoggingLevel.All, ProtocolEvent.Disconnect, null, reason);
		}

		public void LogSend(byte[] data)
		{
			this.LogEvent(ProtocolLoggingLevel.All, ProtocolEvent.Send, data, null);
		}

		public void LogReceive(byte[] data)
		{
			this.LogEvent(ProtocolLoggingLevel.All, ProtocolEvent.Receive, data, null);
		}

		public void LogInformation(ProtocolLoggingLevel loggingLevel, byte[] data, string context)
		{
			this.LogEvent(loggingLevel, ProtocolEvent.Information, data, context);
		}

		public void LogStringInformation(ProtocolLoggingLevel loggingLevel, string data, string context)
		{
			this.LogEvent(loggingLevel, ProtocolEvent.Information, Encoding.UTF8.GetBytes(data), context);
		}

		public void LogCertificate(string type, X509Certificate2 cert)
		{
			if (cert == null)
			{
				return;
			}
			this.LogInformation(ProtocolLoggingLevel.All, null, type);
			this.LogStringInformation(ProtocolLoggingLevel.All, cert.Subject, "Certificate subject");
			this.LogStringInformation(ProtocolLoggingLevel.All, cert.IssuerName.Name, "Certificate issuer name");
			this.LogStringInformation(ProtocolLoggingLevel.All, cert.SerialNumber, "Certificate serial number");
			this.LogStringInformation(ProtocolLoggingLevel.All, cert.Thumbprint, "Certificate thumbprint");
			StringBuilder stringBuilder = new StringBuilder(256);
			foreach (string value in TlsCertificateInfo.GetFQDNs(cert))
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append(';');
				}
				stringBuilder.Append(value);
			}
			this.LogStringInformation(ProtocolLoggingLevel.All, stringBuilder.ToString(), "Certificate alternate names");
		}

		public void LogCertificateThumbprint(string type, X509Certificate2 cert)
		{
			if (cert == null)
			{
				return;
			}
			this.LogInformation(ProtocolLoggingLevel.All, null, type);
			this.LogStringInformation(ProtocolLoggingLevel.All, cert.Thumbprint, "Certificate thumbprint");
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

		private void LogEvent(ProtocolLoggingLevel loggingLevel, ProtocolEvent eventId, byte[] data, string context)
		{
			if (this.protocolLog.LoggingLevel < loggingLevel)
			{
				return;
			}
			this.row[4] = eventId;
			this.row[6] = context;
			int num = 0;
			byte[] line;
			do
			{
				line = ProtocolLogSession.GetLine(data, num);
				if (line != null)
				{
					num += line.Length + 2;
				}
				if (line != null || (line == null && num == 0))
				{
					this.row[5] = line;
					this.protocolLog.Append(this.row);
				}
			}
			while (line != null);
			this.row[5] = null;
		}

		private ProtocolLog protocolLog;

		private LogRowFormatter row;
	}
}
