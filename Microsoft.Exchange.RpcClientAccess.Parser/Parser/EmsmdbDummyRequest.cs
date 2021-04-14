using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class EmsmdbDummyRequest : MapiHttpRequest
	{
		public EmsmdbDummyRequest() : base(Array<byte>.EmptySegment)
		{
		}

		public EmsmdbDummyRequest(Reader reader) : base(reader)
		{
		}

		public override void Serialize(Writer writer)
		{
		}
	}
}
