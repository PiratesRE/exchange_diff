using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulSeekRowBookmarkResult : RopResult
	{
		internal bool PositionChanged
		{
			get
			{
				return this.positionChanged;
			}
		}

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

		internal SuccessfulSeekRowBookmarkResult(bool positionChanged, bool soughtLessThanRequested, int rowsSought) : base(RopId.SeekRowBookmark, ErrorCode.None, null)
		{
			this.positionChanged = positionChanged;
			this.soughtLessThanRequested = soughtLessThanRequested;
			this.rowsSought = rowsSought;
		}

		internal SuccessfulSeekRowBookmarkResult(Reader reader) : base(reader)
		{
			this.positionChanged = reader.ReadBool();
			this.soughtLessThanRequested = reader.ReadBool();
			this.rowsSought = reader.ReadInt32();
		}

		public override string ToString()
		{
			return string.Format("SuccessfulSeekRowBookmarkResult: [PositionChanged: {0}] [SoughtLess: {1}] [Rows: {2}]", this.positionChanged, this.soughtLessThanRequested, this.rowsSought);
		}

		internal static SuccessfulSeekRowBookmarkResult Parse(Reader reader)
		{
			return new SuccessfulSeekRowBookmarkResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteBool(this.positionChanged, 1);
			writer.WriteBool(this.soughtLessThanRequested, 1);
			writer.WriteInt32(this.rowsSought);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" PositionChanged=").Append(this.positionChanged);
			stringBuilder.Append(" SoughtLessThanRequested=").Append(this.soughtLessThanRequested);
			stringBuilder.Append(" RowsSought=").Append(this.rowsSought);
		}

		private readonly bool positionChanged;

		private readonly bool soughtLessThanRequested;

		private readonly int rowsSought;
	}
}
