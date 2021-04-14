using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class FastTransferDestinationPutBufferResult : RopResult
	{
		internal ushort Progress
		{
			get
			{
				return this.progress;
			}
		}

		internal ushort Steps
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

		internal FastTransferDestinationPutBufferResult(ErrorCode errorCode, ushort progress, ushort steps, bool isMoveUser, ushort usedBufferSize) : base(RopId.FastTransferDestinationPutBuffer, errorCode, null)
		{
			this.progress = progress;
			this.steps = steps;
			this.isMoveUser = isMoveUser;
			this.usedBufferSize = usedBufferSize;
		}

		internal FastTransferDestinationPutBufferResult(Reader reader) : base(reader)
		{
			reader.Position += 2L;
			this.progress = reader.ReadUInt16();
			this.steps = reader.ReadUInt16();
			this.isMoveUser = reader.ReadBool();
			this.usedBufferSize = reader.ReadUInt16();
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt16(0);
			writer.WriteUInt16(this.progress);
			writer.WriteUInt16(this.steps);
			writer.WriteBool(this.isMoveUser);
			writer.WriteUInt16(this.usedBufferSize);
		}

		private readonly ushort progress;

		private readonly ushort steps;

		private readonly bool isMoveUser;

		private readonly ushort usedBufferSize;
	}
}
