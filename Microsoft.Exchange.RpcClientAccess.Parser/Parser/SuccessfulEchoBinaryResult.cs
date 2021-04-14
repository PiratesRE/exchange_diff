using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulEchoBinaryResult : RopResult
	{
		internal SuccessfulEchoBinaryResult(int returnValue, byte[] outParameter) : base(RopId.EchoBinary, ErrorCode.None, null)
		{
			this.returnValue = returnValue;
			this.outParameter = outParameter;
		}

		internal SuccessfulEchoBinaryResult(Reader reader) : base(reader)
		{
			this.returnValue = reader.ReadInt32();
			this.outParameter = reader.ReadSizeAndByteArray();
		}

		internal static RopResult Parse(Reader reader)
		{
			return new SuccessfulEchoBinaryResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteInt32(this.returnValue);
			writer.WriteSizedBytes(this.outParameter);
		}

		private readonly int returnValue;

		private readonly byte[] outParameter;
	}
}
