using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal interface IExchangeDispatch
	{
		int Connect(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, out IntPtr contextHandle, string userDn, int flags, int connectionModulus, int codePage, int stringLocale, int sortLocale, out TimeSpan pollsMax, out int retryCount, out TimeSpan retryDelay, out string dnPrefix, out string displayName, short[] clientVersion, out short[] serverVersion, ArraySegment<byte> segmentExtendedAuxIn, ArraySegment<byte> segmentExtendedAuxOut, out ArraySegment<byte> auxOutData, IStandardBudget budget);

		int Disconnect(ProtocolRequestInfo protocolRequestInfo, ref IntPtr contextHandle, bool rundown);

		int Execute(ProtocolRequestInfo protocolRequestInfo, ref IntPtr contextHandle, int flags, ArraySegment<byte> segmentExtendedRopIn, ArraySegment<byte> segmentExtendedRopOut, out ArraySegment<byte> ropOutData, ArraySegment<byte> segmentExtendedAuxIn, ArraySegment<byte> segmentExtendedAuxOut, out ArraySegment<byte> auxOutData);

		int NotificationConnect(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, out IntPtr asynchronousContextHandle);

		void NotificationWait(ProtocolRequestInfo protocolRequestInfo, IntPtr asynchronousContextHandle, int flags, Action<bool, int> completion);

		int RegisterPushNotification(ProtocolRequestInfo protocolRequestInfo, ref IntPtr contextHandle, ArraySegment<byte> segmentContext, int adviseBits, ArraySegment<byte> segmentClientBlob, out int notificationHandle);

		int UnregisterPushNotification(ProtocolRequestInfo protocolRequestInfo, ref IntPtr contextHandle, int notificationHandle);

		int Dummy(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding);

		void DroppedConnection(IntPtr asynchronousContextHandle);
	}
}
