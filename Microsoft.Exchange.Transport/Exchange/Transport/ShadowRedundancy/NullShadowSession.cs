using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Protocols.Smtp;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal class NullShadowSession : IShadowSession
	{
		public IAsyncResult BeginOpen(TransportMailItem transportMailItem, AsyncCallback asyncCallback, object state)
		{
			if (asyncCallback == null)
			{
				throw new ArgumentNullException("asyncCallback");
			}
			return new AsyncResult(asyncCallback, state, true);
		}

		public bool EndOpen(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			return true;
		}

		public virtual IAsyncResult BeginWrite(byte[] buffer, int offset, int count, bool seenEod, AsyncCallback asyncCallback, object state)
		{
			if (asyncCallback == null)
			{
				throw new ArgumentNullException("asyncCallback");
			}
			return new AsyncResult(asyncCallback, state, true);
		}

		public bool EndWrite(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			return true;
		}

		public IAsyncResult BeginComplete(AsyncCallback asyncCallback, object state)
		{
			if (asyncCallback == null)
			{
				throw new ArgumentNullException("asyncCallback");
			}
			return new AsyncResult(asyncCallback, state, true);
		}

		public bool EndComplete(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			return true;
		}

		public void NotifyProxyFailover(string shadowServer, SmtpResponse smtpResponse)
		{
		}

		public void PrepareForNewCommand(BaseDataSmtpCommand newCommand)
		{
		}

		public bool MailItemRequiresShadowCopy(TransportMailItem mailItem)
		{
			return false;
		}

		public void NotifyLocalMessageDiscarded(TransportMailItem mailItem)
		{
		}

		public void NotifyMessageRejected(TransportMailItem mailItem)
		{
		}

		public void NotifyMessageComplete(TransportMailItem mailItem)
		{
		}

		public void Close(AckStatus ackStatus, SmtpResponse smtpResponse)
		{
		}

		public string ShadowServerContext
		{
			get
			{
				return null;
			}
		}
	}
}
