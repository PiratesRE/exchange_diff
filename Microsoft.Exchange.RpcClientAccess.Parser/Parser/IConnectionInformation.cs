using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IConnectionInformation
	{
		ushort SessionId { get; }

		bool ClientSupportsBackoffResult { get; }

		bool ClientSupportsBufferTooSmallBreakup { get; }

		Encoding String8Encoding { get; }
	}
}
