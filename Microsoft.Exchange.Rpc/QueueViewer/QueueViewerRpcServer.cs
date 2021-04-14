using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.QueueViewer
{
	internal abstract class QueueViewerRpcServer : RpcServerBase
	{
		public abstract byte[] GetQueueViewerObjectPage(QVObjectType objectType, byte[] queryFilterBytes, byte[] sortOrderBytes, [MarshalAs(UnmanagedType.U1)] bool searchForward, int pageSize, byte[] bookmarkObjectBytes, int bookmarkIndex, [MarshalAs(UnmanagedType.U1)] bool includeBookmark, [MarshalAs(UnmanagedType.U1)] bool includeDetails, byte[] propertyBagBytes, ref int totalCount, ref int pageOffset);

		public abstract int ReadMessageBody(byte[] mailItemIdBytes, byte[] buffer, int position, int count);

		public abstract void FreezeMessage(byte[] mailItemIdBytes, byte[] queryFilterBytes);

		public abstract void UnfreezeMessage(byte[] mailItemIdBytes, byte[] queryFilterBytes);

		public abstract void DeleteMessage(byte[] mailItemIdBytes, byte[] queryFilterBytes, [MarshalAs(UnmanagedType.U1)] bool withNDR);

		public abstract void RedirectMessage(byte[] targetServersBytes);

		public abstract void FreezeQueue(byte[] queueIdentityBytes, byte[] queryFilterBytes);

		public abstract void UnfreezeQueue(byte[] queueIdentityBytes, byte[] queryFilterBytes);

		public abstract void RetryQueue(byte[] queueIdentityBytes, byte[] queryFilterBytes, [MarshalAs(UnmanagedType.U1)] bool resubmit);

		public abstract byte[] GetPropertyBagBasedQueueViewerObjectPage(QVObjectType objectType, byte[] inputObjectBytes, ref int totalCount, ref int pageOffset);

		public abstract byte[] GetPropertyBagBasedQueueViewerObjectPageCustomSerialization(QVObjectType objectType, byte[] inputObjectBytes, ref int totalCount, ref int pageOffset);

		public abstract void SetMessage(byte[] mailItemIdBytes, byte[] queryFilterBytes, byte[] inputPropertiesBytes, [MarshalAs(UnmanagedType.U1)] bool resubmit);

		public QueueViewerRpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.IQueueViewer_v1_0_s_ifspec;
	}
}
