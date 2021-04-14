using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Protocols.Smtp;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal interface IShadowSession
	{
		IAsyncResult BeginOpen(TransportMailItem transportMailItem, AsyncCallback asyncCallback, object state);

		bool EndOpen(IAsyncResult asyncResult);

		IAsyncResult BeginWrite(byte[] buffer, int offset, int count, bool seenEod, AsyncCallback asyncCallback, object state);

		bool EndWrite(IAsyncResult asyncResult);

		IAsyncResult BeginComplete(AsyncCallback asyncCallback, object state);

		bool EndComplete(IAsyncResult asyncResult);

		bool MailItemRequiresShadowCopy(TransportMailItem mailItem);

		void NotifyProxyFailover(string shadowServer, SmtpResponse smtpResponse);

		void PrepareForNewCommand(BaseDataSmtpCommand newCommand);

		void NotifyLocalMessageDiscarded(TransportMailItem mailItem);

		void NotifyMessageRejected(TransportMailItem mailItem);

		void NotifyMessageComplete(TransportMailItem mailItem);

		void Close(AckStatus ackStatus, SmtpResponse smtpResponse);

		string ShadowServerContext { get; }
	}
}
