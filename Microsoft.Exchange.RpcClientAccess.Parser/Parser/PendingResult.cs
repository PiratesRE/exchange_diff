using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PendingResult : Result
	{
		internal PendingResult(ushort sessionId, Encoding string8Encoding) : base(RopId.Pending)
		{
			this.SessionId = sessionId;
			base.String8Encoding = string8Encoding;
		}

		internal PendingResult(Reader reader) : base(reader)
		{
			this.SessionId = reader.ReadUInt16();
		}

		internal static int Size
		{
			get
			{
				return 3;
			}
		}

		internal ushort SessionId { get; private set; }

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt16(this.SessionId);
		}
	}
}
