using System;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiQueryColumnsRequest : MapiHttpRequest
	{
		public NspiQueryColumnsRequest(NspiQueryColumnsFlags flags, NspiQueryColumnsMapiFlags mapiFlags, ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
			this.flags = flags;
			this.mapiFlags = mapiFlags;
		}

		public NspiQueryColumnsRequest(Reader reader) : base(reader)
		{
			this.flags = (NspiQueryColumnsFlags)reader.ReadUInt32();
			this.mapiFlags = (NspiQueryColumnsMapiFlags)reader.ReadUInt32();
			base.ParseAuxiliaryBuffer(reader);
		}

		public NspiQueryColumnsFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		public NspiQueryColumnsMapiFlags MapiFlags
		{
			get
			{
				return this.mapiFlags;
			}
		}

		public override void Serialize(Writer writer)
		{
			writer.WriteUInt32((uint)this.flags);
			writer.WriteUInt32((uint)this.mapiFlags);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly NspiQueryColumnsFlags flags;

		private readonly NspiQueryColumnsMapiFlags mapiFlags;
	}
}
