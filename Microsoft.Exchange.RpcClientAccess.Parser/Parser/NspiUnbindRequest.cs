using System;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiUnbindRequest : MapiHttpRequest
	{
		public NspiUnbindRequest(NspiUnbindFlags flags, ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
			this.flags = flags;
		}

		public NspiUnbindRequest(Reader reader) : base(reader)
		{
			this.flags = (NspiUnbindFlags)reader.ReadUInt32();
			base.ParseAuxiliaryBuffer(reader);
		}

		public NspiUnbindFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		public override void Serialize(Writer writer)
		{
			writer.WriteUInt32((uint)this.flags);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly NspiUnbindFlags flags;
	}
}
