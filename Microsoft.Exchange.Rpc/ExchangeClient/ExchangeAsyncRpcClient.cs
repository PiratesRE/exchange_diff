using System;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Rpc.ExchangeClient
{
	internal class ExchangeAsyncRpcClient : RpcClientBase, IExchangeAsyncDispatch
	{
		public ExchangeAsyncRpcClient(RpcBindingInfo bindingInfo) : base(bindingInfo.UseKerberosSpn("exchangeMDB", null))
		{
		}

		public virtual ICancelableAsyncResult BeginConnect(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, string userDn, int flags, int conMod, int cpid, int lcidString, int lcidSort, short[] clientVersion, ArraySegment<byte> segmentExtendedAuxIn, ArraySegment<byte> segmentExtendedAuxOut, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			ClientAsyncCallState_Connect clientAsyncCallState_Connect = null;
			bool flag = false;
			ICancelableAsyncResult result;
			try
			{
				IntPtr pRpcBindingHandle = (IntPtr)base.BindingHandle;
				clientAsyncCallState_Connect = new ClientAsyncCallState_Connect(asyncCallback, asyncState, pRpcBindingHandle, userDn, flags, conMod, cpid, lcidString, lcidSort, clientVersion, segmentExtendedAuxIn, segmentExtendedAuxOut);
				clientAsyncCallState_Connect.Begin();
				flag = true;
				result = clientAsyncCallState_Connect;
			}
			finally
			{
				if (!flag && clientAsyncCallState_Connect != null)
				{
					((IDisposable)clientAsyncCallState_Connect).Dispose();
				}
			}
			return result;
		}

		public virtual int EndConnect(ICancelableAsyncResult result, out IntPtr contextHandle, out TimeSpan pollsMax, out int retryCount, out TimeSpan retryDelay, out string dnPrefix, out string displayName, out short[] serverVersion, out ArraySegment<byte> segmentExtendedAuxOut)
		{
			int result2;
			using (ClientAsyncCallState_Connect clientAsyncCallState_Connect = (ClientAsyncCallState_Connect)result)
			{
				result2 = clientAsyncCallState_Connect.End(out contextHandle, out pollsMax, out retryCount, out retryDelay, out dnPrefix, out displayName, out serverVersion, out segmentExtendedAuxOut);
			}
			return result2;
		}

		public virtual ICancelableAsyncResult BeginDisconnect(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			ClientAsyncCallState_Disconnect clientAsyncCallState_Disconnect = null;
			bool flag = false;
			ICancelableAsyncResult result;
			try
			{
				clientAsyncCallState_Disconnect = new ClientAsyncCallState_Disconnect(asyncCallback, asyncState, contextHandle);
				clientAsyncCallState_Disconnect.Begin();
				flag = true;
				result = clientAsyncCallState_Disconnect;
			}
			finally
			{
				if (!flag && clientAsyncCallState_Disconnect != null)
				{
					((IDisposable)clientAsyncCallState_Disconnect).Dispose();
				}
			}
			return result;
		}

		public virtual int EndDisconnect(ICancelableAsyncResult result, out IntPtr contextHandle)
		{
			int result2;
			using (ClientAsyncCallState_Disconnect clientAsyncCallState_Disconnect = (ClientAsyncCallState_Disconnect)result)
			{
				result2 = clientAsyncCallState_Disconnect.End(out contextHandle);
			}
			return result2;
		}

		public virtual ICancelableAsyncResult BeginExecute(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, int flags, ArraySegment<byte> segmentExtendedRopIn, ArraySegment<byte> segmentExtendedRopOut, ArraySegment<byte> segmentExtendedAuxIn, ArraySegment<byte> segmentExtendedAuxOut, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			ClientAsyncCallState_Execute clientAsyncCallState_Execute = null;
			bool flag = false;
			ICancelableAsyncResult result;
			try
			{
				clientAsyncCallState_Execute = new ClientAsyncCallState_Execute(asyncCallback, asyncState, contextHandle, flags, segmentExtendedRopIn, segmentExtendedRopOut, segmentExtendedAuxIn, segmentExtendedAuxOut);
				clientAsyncCallState_Execute.Begin();
				flag = true;
				result = clientAsyncCallState_Execute;
			}
			finally
			{
				if (!flag && clientAsyncCallState_Execute != null)
				{
					((IDisposable)clientAsyncCallState_Execute).Dispose();
				}
			}
			return result;
		}

		public virtual int EndExecute(ICancelableAsyncResult result, out IntPtr contextHandle, out ArraySegment<byte> segmentExtendedRopOut, out ArraySegment<byte> segmentExtendedAuxOut)
		{
			int result2;
			using (ClientAsyncCallState_Execute clientAsyncCallState_Execute = (ClientAsyncCallState_Execute)result)
			{
				result2 = clientAsyncCallState_Execute.End(out contextHandle, out segmentExtendedRopOut, out segmentExtendedAuxOut);
			}
			return result2;
		}

		public virtual ICancelableAsyncResult BeginDummy(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			ClientAsyncCallState_Dummy clientAsyncCallState_Dummy = null;
			bool flag = false;
			ICancelableAsyncResult result;
			try
			{
				IntPtr pRpcBindingHandle = (IntPtr)base.BindingHandle;
				clientAsyncCallState_Dummy = new ClientAsyncCallState_Dummy(asyncCallback, asyncState, pRpcBindingHandle);
				clientAsyncCallState_Dummy.Begin();
				flag = true;
				result = clientAsyncCallState_Dummy;
			}
			finally
			{
				if (!flag && clientAsyncCallState_Dummy != null)
				{
					((IDisposable)clientAsyncCallState_Dummy).Dispose();
				}
			}
			return result;
		}

		public virtual int EndDummy(ICancelableAsyncResult result)
		{
			int result2;
			using (ClientAsyncCallState_Dummy clientAsyncCallState_Dummy = (ClientAsyncCallState_Dummy)result)
			{
				result2 = clientAsyncCallState_Dummy.CheckCompletion();
			}
			return result2;
		}

		public virtual ICancelableAsyncResult BeginNotificationConnect(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new Exception("Not Implemented");
		}

		public virtual int EndNotificationConnect(ICancelableAsyncResult result, out IntPtr notificationContextHandle)
		{
			throw new Exception("Not Implemented");
		}

		public virtual ICancelableAsyncResult BeginNotificationWait(ProtocolRequestInfo protocolRequestInfo, IntPtr notificationContextHandle, int flagsIn, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new Exception("Not Implemented");
		}

		public virtual int EndNotificationWait(ICancelableAsyncResult result, out int flagsOut)
		{
			throw new Exception("Not Implemented");
		}

		public virtual ICancelableAsyncResult BeginRegisterPushNotification(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, ArraySegment<byte> segmentContext, int adviseBits, ArraySegment<byte> segmentClientBlob, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new Exception("Not Implemented");
		}

		public virtual int EndRegisterPushNotification(ICancelableAsyncResult asyncResult, out IntPtr contextHandle, out int notificationHandle)
		{
			throw new Exception("Not Implemented");
		}

		public virtual ICancelableAsyncResult BeginUnregisterPushNotification(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, int notificationHandle, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new Exception("Not Implemented");
		}

		public virtual int EndUnregisterPushNotification(ICancelableAsyncResult asyncResult, out IntPtr contextHandle)
		{
			throw new Exception("Not Implemented");
		}

		public virtual void ContextHandleRundown(IntPtr contextHandle)
		{
			throw new Exception("Not Implemented");
		}

		public virtual void NotificationContextHandleRundown(IntPtr notificationContextHandle)
		{
			throw new Exception("Not Implemented");
		}

		public virtual void DroppedConnection(IntPtr notificationContextHandle)
		{
			throw new Exception("Not Implemented");
		}
	}
}
