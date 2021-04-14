using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulProgressResult : RopResult
	{
		internal SuccessfulProgressResult(byte logonId, uint completedTaskCount, uint totalTaskCount) : base(RopId.Progress, ErrorCode.None, null)
		{
			this.logonId = logonId;
			this.completedTaskCount = completedTaskCount;
			this.totalTaskCount = totalTaskCount;
		}

		internal SuccessfulProgressResult(Reader reader) : base(reader)
		{
			this.logonId = reader.ReadByte();
			this.completedTaskCount = reader.ReadUInt32();
			this.totalTaskCount = reader.ReadUInt32();
		}

		internal byte LogonId
		{
			get
			{
				return this.logonId;
			}
		}

		internal uint CompletedTaskCount
		{
			get
			{
				return this.completedTaskCount;
			}
		}

		internal uint TotalTaskCount
		{
			get
			{
				return this.totalTaskCount;
			}
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteByte(this.logonId);
			writer.WriteUInt32(this.completedTaskCount);
			writer.WriteUInt32(this.totalTaskCount);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" LogonId=").Append(this.logonId);
			stringBuilder.Append(" CompletedTaskCount=").Append(this.completedTaskCount);
			stringBuilder.Append(" TotalTaskCount=").Append(this.totalTaskCount);
		}

		private readonly byte logonId;

		private readonly uint completedTaskCount;

		private readonly uint totalTaskCount;
	}
}
