using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal struct BackoffRopData
	{
		internal BackoffRopData(RopId ropId, uint duration)
		{
			this.ropId = ropId;
			this.duration = duration;
		}

		internal BackoffRopData(Reader reader)
		{
			this.ropId = (RopId)reader.ReadByte();
			this.duration = reader.ReadUInt32();
		}

		public RopId RopId
		{
			get
			{
				return this.ropId;
			}
		}

		public uint Duration
		{
			get
			{
				return this.duration;
			}
		}

		internal void Serialize(Writer writer)
		{
			writer.WriteByte((byte)this.RopId);
			writer.WriteUInt32(this.Duration);
		}

		private readonly RopId ropId;

		private readonly uint duration;
	}
}
