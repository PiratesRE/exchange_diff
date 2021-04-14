using System;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Notifications.Broker;

namespace Microsoft.Exchange.Rpc
{
	internal interface INotificationsBrokerAsyncDispatch
	{
		ICancelableAsyncResult BeginSubscribe(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, string subscription, CancelableAsyncCallback asyncCallback, object asyncState);

		BrokerStatus EndSubscribe(ICancelableAsyncResult asyncResult);

		ICancelableAsyncResult BeginUnsubscribe(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, string subscription, CancelableAsyncCallback asyncCallback, object asyncState);

		BrokerStatus EndUnsubscribe(ICancelableAsyncResult asyncResult);

		ICancelableAsyncResult BeginGetNextNotification(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, int consumerId, Guid ackNotificationId, CancelableAsyncCallback asyncCallback, object asyncState);

		BrokerStatus EndGetNextNotification(ICancelableAsyncResult asyncResult, out string notification);
	}
}
