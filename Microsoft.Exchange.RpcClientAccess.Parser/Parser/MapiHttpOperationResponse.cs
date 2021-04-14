using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class MapiHttpOperationResponse : MapiHttpResponse
	{
		protected MapiHttpOperationResponse(uint returnCode, ArraySegment<byte> auxiliaryBuffer) : base(0U, auxiliaryBuffer)
		{
			this.returnCode = returnCode;
		}

		protected MapiHttpOperationResponse(Reader reader) : base(reader)
		{
			this.returnCode = reader.ReadUInt32();
			if (base.StatusCode != 0U)
			{
				throw new InvalidStatusCodeException("Attempted to parse a successful response with a nonzero StatusCode", base.StatusCode);
			}
		}

		public uint ReturnCode
		{
			get
			{
				return this.returnCode;
			}
		}

		public override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32(this.returnCode);
		}

		public override void AppendLogString(StringBuilder stringBuilder)
		{
			base.AppendLogString(stringBuilder);
			if (this.returnCode == 0U)
			{
				stringBuilder.Append(";RC:0");
				return;
			}
			stringBuilder.Append(";RC:");
			stringBuilder.Append(this.returnCode);
		}

		private readonly uint returnCode;
	}
}
