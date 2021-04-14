using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class FastTransferSourceGetBufferData
	{
		internal FastTransferSourceGetBufferData(FastTransferState state, uint progress, uint steps, bool isMoveUser, ArraySegment<byte> data, bool isExtendedRop)
		{
			this.State = state;
			this.Progress = progress;
			this.Steps = steps;
			this.IsMoveUser = isMoveUser;
			this.Data = data;
			this.isExtendedRop = isExtendedRop;
		}

		internal FastTransferSourceGetBufferData(uint backOffTime, bool isExtendedRop)
		{
			this.isServerBusy = true;
			this.isExtendedRop = isExtendedRop;
			this.BackOffTime = backOffTime;
		}

		internal FastTransferSourceGetBufferData(FastTransferState state, bool isExtendedRop)
		{
			this.State = state;
			this.isExtendedRop = isExtendedRop;
		}

		internal FastTransferSourceGetBufferData(Reader reader, bool isExtendedRop, bool isServerBusy)
		{
			this.isExtendedRop = isExtendedRop;
			this.isServerBusy = isServerBusy;
			this.State = (FastTransferState)reader.ReadUInt16();
			if (this.isExtendedRop)
			{
				this.Progress = reader.ReadUInt32();
				this.Steps = reader.ReadUInt32();
			}
			else
			{
				this.Progress = (uint)reader.ReadUInt16();
				this.Steps = (uint)reader.ReadUInt16();
			}
			this.IsMoveUser = reader.ReadBool();
			ushort num = reader.ReadUInt16();
			if (this.isServerBusy)
			{
				this.BackOffTime = reader.ReadUInt32();
			}
			if (num > 0)
			{
				this.Data = reader.ReadArraySegment((uint)num);
			}
		}

		internal void Serialize(Writer writer)
		{
			writer.WriteUInt16((ushort)this.State);
			if (this.isExtendedRop)
			{
				writer.WriteUInt32(this.Progress);
				writer.WriteUInt32(this.Steps);
			}
			else
			{
				writer.WriteUInt16((ushort)this.Progress);
				writer.WriteUInt16((ushort)this.Steps);
			}
			writer.WriteBool(this.IsMoveUser);
			writer.WriteUInt16((ushort)this.Data.Count);
			if (this.isServerBusy)
			{
				writer.WriteUInt32(this.BackOffTime);
			}
			if (this.Data.Count > 0)
			{
				writer.SkipArraySegment(this.Data);
			}
		}

		public override string ToString()
		{
			return string.Format("FastTransferStream({0}, {1}b)", this.State, this.Data.Count);
		}

		internal void AppendToString(StringBuilder stringBuilder)
		{
			stringBuilder.Append(" state=").Append(this.State);
			stringBuilder.Append(" progress=").Append(this.Progress);
			stringBuilder.Append(" steps=").Append(this.Steps);
			stringBuilder.Append(" moveUser=").Append(this.IsMoveUser);
			stringBuilder.Append(" serverBusy=").Append(this.isServerBusy);
			if (this.isServerBusy)
			{
				stringBuilder.Append(" backoffTime=").Append(this.BackOffTime);
			}
			stringBuilder.Append(" data=[");
			Util.AppendToString(stringBuilder, this.Data.Array, this.Data.Offset, this.Data.Count);
			stringBuilder.Append("]");
		}

		internal readonly FastTransferState State;

		internal readonly uint Progress;

		internal readonly uint Steps;

		internal readonly bool IsMoveUser;

		internal readonly uint BackOffTime;

		internal readonly ArraySegment<byte> Data = Array<byte>.EmptySegment;

		private bool isExtendedRop;

		private bool isServerBusy;
	}
}
