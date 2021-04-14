using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulQueryPositionResult : RopResult
	{
		internal SuccessfulQueryPositionResult(uint numerator, uint denominator) : base(RopId.QueryPosition, ErrorCode.None, null)
		{
			this.numerator = numerator;
			this.denominator = denominator;
		}

		internal SuccessfulQueryPositionResult(Reader reader) : base(reader)
		{
			this.numerator = reader.ReadUInt32();
			this.denominator = reader.ReadUInt32();
		}

		internal uint Numerator
		{
			get
			{
				return this.numerator;
			}
		}

		internal uint Denominator
		{
			get
			{
				return this.denominator;
			}
		}

		internal static SuccessfulQueryPositionResult Parse(Reader reader)
		{
			return new SuccessfulQueryPositionResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32(this.numerator);
			writer.WriteUInt32(this.denominator);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Numerator=").Append(this.numerator);
			stringBuilder.Append(" Denominator=").Append(this.denominator);
		}

		private readonly uint numerator;

		private readonly uint denominator;
	}
}
