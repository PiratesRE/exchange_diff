using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BufferTooSmallResult : Result
	{
		internal BufferTooSmallResult(ushort sizeNeeded, ArraySegment<byte> unexecutedBuffer, Encoding string8Encoding) : base(RopId.BufferTooSmall)
		{
			this.SizeNeeded = sizeNeeded;
			this.UnexecutedBuffer = unexecutedBuffer;
			base.String8Encoding = string8Encoding;
		}

		internal static int HeaderSize
		{
			get
			{
				return 3;
			}
		}

		internal ushort SizeNeeded { get; private set; }

		internal ArraySegment<byte> UnexecutedBuffer { get; private set; }

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt16(this.SizeNeeded);
			writer.WriteBytesSegment(this.UnexecutedBuffer);
		}
	}
}
