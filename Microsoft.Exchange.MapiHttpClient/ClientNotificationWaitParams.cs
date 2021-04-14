using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ClientNotificationWaitParams
	{
		public static void Serialize(BufferWriter writer, int flags)
		{
			writer.WriteInt32(flags);
		}

		public static void Parse(Reader reader, out int flags)
		{
			flags = reader.ReadInt32();
		}
	}
}
