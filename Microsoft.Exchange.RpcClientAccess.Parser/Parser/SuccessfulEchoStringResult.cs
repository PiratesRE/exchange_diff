using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulEchoStringResult : RopResult
	{
		internal SuccessfulEchoStringResult(string returnValue, string outParameter) : base(RopId.EchoString, ErrorCode.None, null)
		{
			this.returnValue = returnValue;
			this.outParameter = outParameter;
		}

		internal SuccessfulEchoStringResult(Reader reader) : base(reader)
		{
			this.outParameter = reader.ReadAsciiString(StringFlags.Sized16);
			this.returnValue = reader.ReadAsciiString(StringFlags.Sized16);
		}

		internal static RopResult Parse(Reader reader)
		{
			return new SuccessfulEchoStringResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteAsciiString(this.outParameter, StringFlags.Sized16);
			writer.WriteAsciiString(this.returnValue, StringFlags.Sized16);
		}

		private readonly string returnValue;

		private readonly string outParameter;
	}
}
