using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ClientExecuteParams
	{
		public static void Serialize(BufferWriter writer, int flags, ArraySegment<byte> segmentExtendedRopIn, int maxSizeExtendedRopOut, ArraySegment<byte> segmentExtendedAuxIn, int maxSizeExtendedAuxOut)
		{
			writer.WriteInt32(flags);
			writer.WriteInt32(maxSizeExtendedRopOut);
			writer.WriteSizedBytesSegment(segmentExtendedRopIn, FieldLength.DWordSize);
			writer.WriteInt32(maxSizeExtendedAuxOut);
			writer.WriteSizedBytesSegment(segmentExtendedAuxIn, FieldLength.DWordSize);
		}

		public static void Parse(Reader reader, out int flags, out TimeSpan transTime, ref ArraySegment<byte> segmentExtendedRopOut, ref ArraySegment<byte> segmentExtendedAuxOut)
		{
			flags = reader.ReadInt32();
			transTime = TimeSpan.FromMilliseconds(reader.ReadUInt32());
			int count = reader.ReadInt32();
			ArraySegment<byte> arraySegment = reader.ReadArraySegment((uint)count);
			Array.Copy(arraySegment.Array, arraySegment.Offset, segmentExtendedRopOut.Array, segmentExtendedRopOut.Offset, arraySegment.Count);
			segmentExtendedRopOut = new ArraySegment<byte>(segmentExtendedRopOut.Array, segmentExtendedRopOut.Offset, arraySegment.Count);
			int count2 = reader.ReadInt32();
			ArraySegment<byte> arraySegment2 = reader.ReadArraySegment((uint)count2);
			Array.Copy(arraySegment2.Array, arraySegment2.Offset, segmentExtendedAuxOut.Array, segmentExtendedAuxOut.Offset, arraySegment2.Count);
			segmentExtendedAuxOut = new ArraySegment<byte>(segmentExtendedAuxOut.Array, segmentExtendedAuxOut.Offset, arraySegment2.Count);
		}
	}
}
