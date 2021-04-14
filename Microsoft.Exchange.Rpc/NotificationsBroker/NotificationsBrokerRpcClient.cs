using System;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Notifications.Broker;

namespace Microsoft.Exchange.Rpc.NotificationsBroker
{
	internal class NotificationsBrokerRpcClient : RpcClientBase, INotificationsBrokerAsyncDispatch
	{
		public NotificationsBrokerRpcClient(RpcBindingInfo bindingInfo) : base(bindingInfo.UseKerberosSpn("exchangeNotificationsBroker", null))
		{
		}

		public virtual ICancelableAsyncResult BeginSubscribe(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, string subscription, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			ClientAsyncCallState_Subscribe clientAsyncCallState_Subscribe = null;
			bool flag = false;
			ICancelableAsyncResult result;
			try
			{
				IntPtr bindingHandle = (IntPtr)base.BindingHandle;
				clientAsyncCallState_Subscribe = new ClientAsyncCallState_Subscribe(asyncCallback, asyncState, bindingHandle, subscription);
				clientAsyncCallState_Subscribe.Begin();
				flag = true;
				result = clientAsyncCallState_Subscribe;
			}
			finally
			{
				if (!flag && clientAsyncCallState_Subscribe != null)
				{
					((IDisposable)clientAsyncCallState_Subscribe).Dispose();
				}
			}
			return result;
		}

		public virtual BrokerStatus EndSubscribe(ICancelableAsyncResult asyncResult)
		{
			BrokerStatus result;
			using (ClientAsyncCallState_Subscribe clientAsyncCallState_Subscribe = (ClientAsyncCallState_Subscribe)asyncResult)
			{
				result = clientAsyncCallState_Subscribe.End();
			}
			return result;
		}

		public virtual ICancelableAsyncResult BeginUnsubscribe(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, string subscription, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			ClientAsyncCallState_Unsubscribe clientAsyncCallState_Unsubscribe = null;
			bool flag = false;
			ICancelableAsyncResult result;
			try
			{
				IntPtr bindingHandle = (IntPtr)base.BindingHandle;
				clientAsyncCallState_Unsubscribe = new ClientAsyncCallState_Unsubscribe(asyncCallback, asyncState, bindingHandle, subscription);
				clientAsyncCallState_Unsubscribe.Begin();
				flag = true;
				result = clientAsyncCallState_Unsubscribe;
			}
			finally
			{
				if (!flag && clientAsyncCallState_Unsubscribe != null)
				{
					((IDisposable)clientAsyncCallState_Unsubscribe).Dispose();
				}
			}
			return result;
		}

		public virtual BrokerStatus EndUnsubscribe(ICancelableAsyncResult asyncResult)
		{
			BrokerStatus result;
			using (ClientAsyncCallState_Unsubscribe clientAsyncCallState_Unsubscribe = (ClientAsyncCallState_Unsubscribe)asyncResult)
			{
				result = clientAsyncCallState_Unsubscribe.End();
			}
			return result;
		}

		public virtual ICancelableAsyncResult BeginGetNextNotification(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, int consumerId, Guid ackNotificationId, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			ClientAsyncCallState_GetNextNotification clientAsyncCallState_GetNextNotification = null;
			bool flag = false;
			ICancelableAsyncResult result;
			try
			{
				IntPtr bindingHandle = (IntPtr)base.BindingHandle;
				clientAsyncCallState_GetNextNotification = new ClientAsyncCallState_GetNextNotification(asyncCallback, asyncState, bindingHandle, consumerId, ackNotificationId);
				clientAsyncCallState_GetNextNotification.Begin();
				flag = true;
				result = clientAsyncCallState_GetNextNotification;
			}
			finally
			{
				if (!flag && clientAsyncCallState_GetNextNotification != null)
				{
					((IDisposable)clientAsyncCallState_GetNextNotification).Dispose();
				}
			}
			return result;
		}

		public virtual BrokerStatus EndGetNextNotification(ICancelableAsyncResult asyncResult, out string notification)
		{
			BrokerStatus result;
			using (ClientAsyncCallState_GetNextNotification clientAsyncCallState_GetNextNotification = (ClientAsyncCallState_GetNextNotification)asyncResult)
			{
				result = clientAsyncCallState_GetNextNotification.End(out notification);
			}
			return result;
		}
	}
}
