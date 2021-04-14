using System;
using System.Net;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface IInboundProxyLayer
	{
		ulong SessionId { get; }

		string NextHopFqdn { get; }

		IInboundProxyDestinationTracker InboundProxyDestinationTracker { get; }

		IPEndPoint ClientEndPoint { get; }

		string ClientHelloDomain { get; }

		IEhloOptions SmtpInEhloOptions { get; }

		long BytesRead { get; }

		long BytesWritten { get; }

		bool IsBdat { get; }

		long OutboundChunkSize { get; }

		bool IsLastChunk { get; }

		uint XProxyFromSeqNum { get; }

		Permission Permissions { get; }

		AuthenticationSource AuthenticationSource { get; }

		void AckMessage(AckStatus status, SmtpResponse response, string source, SessionSetupFailureReason failureReason);

		void AckMessage(AckStatus status, SmtpResponse response, bool replaceFailureResponse, string source, SessionSetupFailureReason failureReason);

		void AckConnection(AckStatus status, SmtpResponse response, SessionSetupFailureReason failureReason);

		void AckCommandSuccessful();

		void BeginReadData(InboundProxyLayer.ReadCompletionCallback readCompleteCallback);

		void WaitForNewCommand(InboundBdatProxyLayer.CommandReceivedCallback commandReceivedCallback);

		void Shutdown();

		void ReleaseMailItem();

		void ReturnBuffer(BufferCacheEntry bufferHolder);
	}
}
