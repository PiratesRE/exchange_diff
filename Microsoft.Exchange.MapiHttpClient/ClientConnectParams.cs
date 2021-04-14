using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ClientConnectParams
	{
		public static void Serialize(BufferWriter writer, string userDn, int flags, int connectionModulus, int codePage, int localeString, int localeSort, short[] clientVersion, ArraySegment<byte> segmentExtendedAuxIn, int maxSizeExtendedAuxOut)
		{
			Util.ThrowOnNullArgument(userDn, "userDn");
			Util.ThrowOnNullArgument(clientVersion, "clientVersion");
			short[] array;
			if (clientVersion.Length == 4)
			{
				array = new short[3];
				MapiVersionConversion.Legacy(clientVersion, array, 0);
			}
			else
			{
				if (clientVersion.Length != 3)
				{
					throw new ArgumentException(string.Format("clientVersion is invalid length (must be 3 or 4); found length={0}", clientVersion.Length));
				}
				array = clientVersion;
			}
			writer.WriteAsciiString(userDn, StringFlags.IncludeNull | StringFlags.Sized16);
			writer.WriteInt32(flags);
			writer.WriteInt32(connectionModulus);
			writer.WriteInt32(codePage);
			writer.WriteInt32(localeString);
			writer.WriteInt32(localeSort);
			writer.WriteInt16(array[0]);
			writer.WriteInt16(array[1]);
			writer.WriteInt16(array[2]);
			writer.WriteInt32(maxSizeExtendedAuxOut);
			writer.WriteSizedBytesSegment(segmentExtendedAuxIn, FieldLength.DWordSize);
		}

		public static void Parse(Reader reader, out TimeSpan pollsMax, out int retryCount, out TimeSpan retryDelay, out string dnPrefix, out string displayName, out short[] serverVersion, ref ArraySegment<byte> segmentExtendedAuxOut)
		{
			pollsMax = TimeSpan.FromMilliseconds((double)reader.ReadInt32());
			retryCount = reader.ReadInt32();
			retryDelay = TimeSpan.FromMilliseconds((double)reader.ReadInt32());
			dnPrefix = reader.ReadAsciiString(StringFlags.IncludeNull | StringFlags.Sized16);
			displayName = reader.ReadAsciiString(StringFlags.IncludeNull | StringFlags.Sized16);
			short[] version = new short[]
			{
				reader.ReadInt16(),
				reader.ReadInt16(),
				reader.ReadInt16()
			};
			short[] array = new short[4];
			MapiVersionConversion.Normalize(version, array);
			serverVersion = array;
			int count = reader.ReadInt32();
			ArraySegment<byte> arraySegment = reader.ReadArraySegment((uint)count);
			Array.Copy(arraySegment.Array, arraySegment.Offset, segmentExtendedAuxOut.Array, segmentExtendedAuxOut.Offset, arraySegment.Count);
			segmentExtendedAuxOut = new ArraySegment<byte>(segmentExtendedAuxOut.Array, segmentExtendedAuxOut.Offset, arraySegment.Count);
		}
	}
}
