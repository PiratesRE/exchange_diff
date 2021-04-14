using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class FastTransferDestinationPutBufferExtendedResult : RopResult
	{
		internal uint Progress
		{
			get
			{
				return this.progress;
			}
		}

		internal uint Steps
		{
			get
			{
				return this.steps;
			}
		}

		internal bool IsMoveUser
		{
			get
			{
				return this.isMoveUser;
			}
		}

		internal ushort UsedBufferSize
		{
			get
			{
				return this.usedBufferSize;
			}
		}

		internal FastTransferDestinationPutBufferExtendedResult(ErrorCode errorCode, uint progress, uint steps, bool isMoveUser, ushort usedBufferSize) : base(RopId.FastTransferDestinationPutBufferExtended, errorCode, null)
		{
			this.progress = progress;
			this.steps = steps;
			this.isMoveUser = isMoveUser;
			this.usedBufferSize = usedBufferSize;
		}

		internal FastTransferDestinationPutBufferExtendedResult(Reader reader) : base(reader)
		{
			reader.Position += 2L;
			this.progress = reader.ReadUInt32();
			this.steps = reader.ReadUInt32();
			this.isMoveUser = reader.ReadBool();
			this.usedBufferSize = reader.ReadUInt16();
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt16(0);
			writer.WriteUInt32(this.progress);
			writer.WriteUInt32(this.steps);
			writer.WriteBool(this.isMoveUser);
			writer.WriteUInt16(this.usedBufferSize);
		}

		private readonly uint progress;

		private readonly uint steps;

		private readonly bool isMoveUser;

		private readonly ushort usedBufferSize;
	}
}
