using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulEchoIntResult : RopResult
	{
		internal SuccessfulEchoIntResult(int returnValue, int outParameter) : base(RopId.EchoInt, ErrorCode.None, null)
		{
			this.returnValue = returnValue;
			this.outParameter = outParameter;
		}

		internal SuccessfulEchoIntResult(Reader reader) : base(reader)
		{
			this.outParameter = reader.ReadInt32();
			this.returnValue = reader.ReadInt32();
		}

		public static RopResult Parse(Reader reader)
		{
			return new SuccessfulEchoIntResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteInt32(this.outParameter);
			writer.WriteInt32(this.returnValue);
		}

		private readonly int returnValue;

		private readonly int outParameter;
	}
}
