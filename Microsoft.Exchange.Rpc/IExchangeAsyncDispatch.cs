using System;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Rpc
{
	internal interface IExchangeAsyncDispatch
	{
		ICancelableAsyncResult BeginConnect(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, string userDn, int flags, int conMod, int cpid, int lcidString, int lcidSort, short[] clientVersion, ArraySegment<byte> segmentExtendedAuxIn, ArraySegment<byte> segmentExtendedAuxOut, CancelableAsyncCallback asyncCallback, object asyncState);

		int EndConnect(ICancelableAsyncResult result, out IntPtr contextHandle, out TimeSpan pollsMax, out int retryCount, out TimeSpan retryDelay, out string dnPrefix, out string displayName, out short[] serverVersion, out ArraySegment<byte> segmentExtendedAuxOut);

		ICancelableAsyncResult BeginDisconnect(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, CancelableAsyncCallback asyncCallback, object asyncState);

		int EndDisconnect(ICancelableAsyncResult result, out IntPtr contextHandle);

		ICancelableAsyncResult BeginExecute(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, int flags, ArraySegment<byte> segmentExtendedRopIn, ArraySegment<byte> segmentExtendedRopOut, ArraySegment<byte> segmentExtendedAuxIn, ArraySegment<byte> segmentExtendedAuxOut, CancelableAsyncCallback asyncCallback, object asyncState);

		int EndExecute(ICancelableAsyncResult result, out IntPtr contextHandle, out ArraySegment<byte> segmentExtendedRopOut, out ArraySegment<byte> segmentExtendedAuxOut);

		ICancelableAsyncResult BeginNotificationConnect(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, CancelableAsyncCallback asyncCallback, object asyncState);

		int EndNotificationConnect(ICancelableAsyncResult result, out IntPtr notificationContextHandle);

		ICancelableAsyncResult BeginNotificationWait(ProtocolRequestInfo protocolRequestInfo, IntPtr notificationContextHandle, int flagsIn, CancelableAsyncCallback asyncCallback, object asyncState);

		int EndNotificationWait(ICancelableAsyncResult result, out int flagsOut);

		ICancelableAsyncResult BeginRegisterPushNotification(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, ArraySegment<byte> segmentContext, int adviseBits, ArraySegment<byte> segmentClientBlob, CancelableAsyncCallback asyncCallback, object asyncState);

		int EndRegisterPushNotification(ICancelableAsyncResult asyncResult, out IntPtr contextHandle, out int notificationHandle);

		ICancelableAsyncResult BeginUnregisterPushNotification(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, int notificationHandle, CancelableAsyncCallback asyncCallback, object asyncState);

		int EndUnregisterPushNotification(ICancelableAsyncResult asyncResult, out IntPtr contextHandle);

		ICancelableAsyncResult BeginDummy(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, CancelableAsyncCallback asyncCallback, object asyncState);

		int EndDummy(ICancelableAsyncResult result);

		void ContextHandleRundown(IntPtr contextHandle);

		void NotificationContextHandleRundown(IntPtr notificationContextHandle);

		void DroppedConnection(IntPtr notificationContextHandle);
	}
}
