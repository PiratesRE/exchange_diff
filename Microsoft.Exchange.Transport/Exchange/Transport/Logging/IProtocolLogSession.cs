using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Transport.Logging
{
	internal interface IProtocolLogSession
	{
		void LogConnect();

		void LogDisconnect(DisconnectReason reason);

		void LogDisconnect(DisconnectReason reason, string remoteError);

		void LogSend(byte[] data);

		void LogReceive(byte[] data);

		void LogInformation(ProtocolLoggingLevel loggingLevel, byte[] data, string formatString, params object[] parameterList);

		void LogInformation(ProtocolLoggingLevel loggingLevel, byte[] data, string context);

		void LogCertificate(string type, IX509Certificate2 cert);

		void LogCertificate(string type, X509Certificate2 cert);

		void LogCertificateThumbprint(string type, IX509Certificate2 cert);

		void LogCertificateThumbprint(string type, X509Certificate2 cert);

		ProtocolLoggingLevel ProtocolLoggingLevel { get; set; }

		IPEndPoint LocalEndPoint { set; }
	}
}
