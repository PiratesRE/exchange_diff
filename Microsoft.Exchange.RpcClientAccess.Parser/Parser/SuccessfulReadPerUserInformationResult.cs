using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulReadPerUserInformationResult : RopResult
	{
		internal SuccessfulReadPerUserInformationResult(bool hasFinished, byte[] data) : base(RopId.ReadPerUserInformation, ErrorCode.None, null)
		{
			Util.ThrowOnNullArgument(data, "data");
			this.hasFinished = hasFinished;
			this.data = data;
		}

		internal SuccessfulReadPerUserInformationResult(Reader reader) : base(reader)
		{
			this.hasFinished = reader.ReadBool();
			this.data = reader.ReadSizeAndByteArray();
		}

		internal bool HasFinished
		{
			get
			{
				return this.hasFinished;
			}
		}

		internal byte[] Data
		{
			get
			{
				return this.data;
			}
		}

		internal static SuccessfulReadPerUserInformationResult Parse(Reader reader)
		{
			return new SuccessfulReadPerUserInformationResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteBool(this.hasFinished, 1);
			writer.WriteSizedBytes(this.data);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" HasFinished=").Append(this.hasFinished);
			stringBuilder.Append(" Data=[");
			Util.AppendToString(stringBuilder, this.data);
			stringBuilder.Append("]");
		}

		private readonly bool hasFinished;

		private readonly byte[] data;
	}
}
