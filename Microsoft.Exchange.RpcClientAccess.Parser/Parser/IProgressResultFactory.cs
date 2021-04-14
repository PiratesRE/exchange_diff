using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IProgressResultFactory
	{
		RopResult CreateProgressResult(uint completedTaskCount, uint totalTaskCount);
	}
}
