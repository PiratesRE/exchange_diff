using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulSeekRowResult : RopResult
	{
		internal bool SoughtLessThanRequested
		{
			get
			{
				return this.soughtLessThanRequested;
			}
		}

		internal int RowsSought
		{
			get
			{
				return this.rowsSought;
			}
		}

		internal SuccessfulSeekRowResult(bool soughtLessThanRequested, int rowsSought) : base(RopId.SeekRow, ErrorCode.None, null)
		{
			this.soughtLessThanRequested = soughtLessThanRequested;
			this.rowsSought = rowsSought;
		}

		internal SuccessfulSeekRowResult(Reader reader) : base(reader)
		{
			this.soughtLessThanRequested = (reader.ReadByte() != 0);
			this.rowsSought = reader.ReadInt32();
		}

		internal static SuccessfulSeekRowResult Parse(Reader reader)
		{
			return new SuccessfulSeekRowResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteBool(this.soughtLessThanRequested, 1);
			writer.WriteInt32(this.rowsSought);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" SoughtLess=").Append(this.soughtLessThanRequested);
			stringBuilder.Append(" RowsSought=").Append(this.rowsSought);
		}

		private readonly bool soughtLessThanRequested;

		private readonly int rowsSought;
	}
}
