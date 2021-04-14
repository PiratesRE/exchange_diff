using System;
using Microsoft.Exchange.Nspi.Rfri;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RfriGetMailboxUrlRequest : MapiHttpRequest
	{
		public RfriGetMailboxUrlRequest(RfriGetMailboxUrlFlags flags, string serverDn, ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
			this.flags = flags;
			this.serverDn = serverDn;
		}

		public RfriGetMailboxUrlRequest(Reader reader) : base(reader)
		{
			this.flags = (RfriGetMailboxUrlFlags)reader.ReadUInt32();
			this.serverDn = reader.ReadUnicodeString(StringFlags.IncludeNull);
			base.ParseAuxiliaryBuffer(reader);
		}

		public RfriGetMailboxUrlFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		public string ServerDn
		{
			get
			{
				return this.serverDn;
			}
		}

		public override void Serialize(Writer writer)
		{
			writer.WriteUInt32((uint)this.flags);
			writer.WriteUnicodeString(this.serverDn, StringFlags.IncludeNull);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly RfriGetMailboxUrlFlags flags;

		private readonly string serverDn;
	}
}
