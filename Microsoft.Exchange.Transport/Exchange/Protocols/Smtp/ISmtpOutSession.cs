using System;
using System.Net;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface ISmtpOutSession
	{
		IProtocolLogSession LogSession { get; }

		IPEndPoint LocalEndPoint { get; }

		AckDetails AckDetails { get; }

		void StartUsingConnection();

		void FailoverConnection(SmtpResponse smtpResponse, SessionSetupFailureReason failoverReason);

		void ConnectionCompleted(NetworkConnection networkConnection);

		void ShutdownConnection();

		string GetConnectionInfo();

		void PrepareForNextMessageOnCachedSession();
	}
}
