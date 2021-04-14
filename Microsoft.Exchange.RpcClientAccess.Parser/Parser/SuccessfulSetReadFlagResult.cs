using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal class SuccessfulSetReadFlagResult : RopResult
	{
		internal SuccessfulSetReadFlagResult(bool hasChanged, byte logonIndex, StoreLongTermId longTermId) : base(RopId.SetReadFlag, ErrorCode.None, null)
		{
			this.hasChanged = hasChanged;
			this.logonIndex = logonIndex;
			this.longTermId = longTermId;
		}

		internal SuccessfulSetReadFlagResult(Reader reader) : base(reader)
		{
			this.hasChanged = reader.ReadBool();
			if (this.hasChanged)
			{
				this.logonIndex = reader.ReadByte();
				this.longTermId = StoreLongTermId.Parse(reader);
			}
		}

		internal static SuccessfulSetReadFlagResult Parse(Reader reader)
		{
			return new SuccessfulSetReadFlagResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteBool(this.hasChanged, 1);
			if (this.hasChanged)
			{
				writer.WriteByte(this.logonIndex);
				this.longTermId.Serialize(writer);
			}
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Changed=").Append(this.hasChanged);
			stringBuilder.Append(" LTID=[").Append(this.longTermId).Append("]");
		}

		private bool hasChanged;

		private byte logonIndex;

		private StoreLongTermId longTermId = StoreLongTermId.Null;
	}
}
