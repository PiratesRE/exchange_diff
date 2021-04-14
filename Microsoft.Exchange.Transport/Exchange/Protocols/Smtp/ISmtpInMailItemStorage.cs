using System;
using System.Threading.Tasks;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface ISmtpInMailItemStorage
	{
		IAsyncResult BeginCommitMailItem(TransportMailItem mailItem, AsyncCallback callback, object state);

		bool EndCommitMailItem(TransportMailItem mailItem, IAsyncResult asyncResult, out Exception exception);

		Task CommitMailItemAsync(TransportMailItem mailItem);
	}
}
