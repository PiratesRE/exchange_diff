using System;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiGetSpecialTableRequest : MapiHttpRequest
	{
		public NspiGetSpecialTableRequest(NspiGetHierarchyInfoFlags flags, NspiState state, uint? version, ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
			this.flags = flags;
			this.state = state;
			this.version = version;
		}

		public NspiGetSpecialTableRequest(Reader reader) : base(reader)
		{
			this.flags = (NspiGetHierarchyInfoFlags)reader.ReadUInt32();
			this.state = reader.ReadNspiState();
			this.version = reader.ReadNullableUInt32();
			base.ParseAuxiliaryBuffer(reader);
		}

		public NspiGetHierarchyInfoFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		public NspiState State
		{
			get
			{
				return this.state;
			}
		}

		public uint? Version
		{
			get
			{
				return this.version;
			}
		}

		public override void Serialize(Writer writer)
		{
			writer.WriteUInt32((uint)this.flags);
			writer.WriteNspiState(this.state);
			writer.WriteNullableUInt32(this.version);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly NspiGetHierarchyInfoFlags flags;

		private readonly NspiState state;

		private readonly uint? version;
	}
}
