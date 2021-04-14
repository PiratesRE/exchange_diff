using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetReceiveFolderTableResultFactory : StandardResultFactory
	{
		internal GetReceiveFolderTableResultFactory() : base(RopId.GetReceiveFolderTable)
		{
		}

		public RopResult CreateSuccessfulResult(PropertyValue[][] rowValues)
		{
			return new SuccessfulGetReceiveFolderTableResult(rowValues);
		}
	}
}
