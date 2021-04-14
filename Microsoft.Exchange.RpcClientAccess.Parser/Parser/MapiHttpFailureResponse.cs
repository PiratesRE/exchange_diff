using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class MapiHttpFailureResponse : MapiHttpResponse
	{
		public MapiHttpFailureResponse(uint statusCode, ArraySegment<byte> auxiliaryBuffer) : base(statusCode, auxiliaryBuffer)
		{
			if (base.StatusCode == 0U)
			{
				throw new ArgumentException("StatusCode must be nonzero in a failure response.");
			}
		}

		public MapiHttpFailureResponse(Reader reader) : base(reader)
		{
			if (base.StatusCode == 0U)
			{
				throw new BufferParseException("StatusCode must be nonzero in a failure response.");
			}
			base.ParseAuxiliaryBuffer(reader);
		}

		public override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private const string StatusCodeExceptionMessage = "StatusCode must be nonzero in a failure response.";
	}
}
