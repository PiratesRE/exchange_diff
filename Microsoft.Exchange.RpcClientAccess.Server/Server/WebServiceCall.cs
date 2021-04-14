using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal class WebServiceCall : EasyAsyncResult
	{
		public WebServiceCall(AsyncCallback asyncCallback, object asyncState) : base(asyncCallback, asyncState)
		{
		}

		internal static byte[] GetBuffer(int size)
		{
			return AsyncBufferPools.GetBuffer(size);
		}

		internal static void ReleaseBuffer(byte[] buffer)
		{
			AsyncBufferPools.ReleaseBuffer(buffer);
		}

		internal static ArraySegment<byte> GetResponseAuxSegment(int size, out byte[] buffer)
		{
			int num = Math.Min(size, EmsmdbConstants.MaxExtendedAuxBufferSize);
			buffer = WebServiceCall.GetBuffer(num);
			return new ArraySegment<byte>(buffer, 0, num);
		}

		internal static ArraySegment<byte> GetResponseRopSegment(int size, out byte[] buffer)
		{
			int num = Math.Min(size, EmsmdbConstants.MaxChainedExtendedRopBufferSize);
			buffer = WebServiceCall.GetBuffer(num);
			return new ArraySegment<byte>(buffer, 0, num);
		}

		internal static ArraySegment<byte> BuildRequestSegment(byte[] buffer)
		{
			if (buffer != null && buffer.Length > 0)
			{
				return new ArraySegment<byte>(buffer);
			}
			return Array<byte>.EmptySegment;
		}

		internal static byte[] BuildResponseArray(ArraySegment<byte> segment)
		{
			byte[] array = new byte[segment.Count];
			Array.Copy(segment.Array, segment.Offset, array, 0, segment.Count);
			return array;
		}
	}
}
