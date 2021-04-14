using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IResultFactory
	{
		RopResult CreateStandardFailedResult(ErrorCode errorCode);

		long SuccessfulResultMinimalSize { get; }
	}
}
