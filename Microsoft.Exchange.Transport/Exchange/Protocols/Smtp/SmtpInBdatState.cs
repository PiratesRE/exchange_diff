using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpInBdatState
	{
		internal long TotalChunkSize;

		internal bool DiscardingMessage;

		internal bool SeenEoh;

		internal string MessageId;

		internal long OriginalMessageSize;

		internal long MessageSizeLimit;

		internal SmtpInBdatProxyParser ProxyParser;

		internal InboundBdatProxyLayer ProxyLayer;
	}
}
