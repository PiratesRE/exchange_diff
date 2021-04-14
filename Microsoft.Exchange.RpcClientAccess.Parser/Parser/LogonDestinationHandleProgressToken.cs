using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LogonDestinationHandleProgressToken : LogonProgressToken
	{
		internal LogonDestinationHandleProgressToken(RopId ropId, uint destinationObjectHandleIndex, byte logonId) : base(ropId, logonId)
		{
			this.destinationObjectHandleIndex = destinationObjectHandleIndex;
		}

		internal uint DestinationObjectHandleIndex
		{
			get
			{
				return this.destinationObjectHandleIndex;
			}
		}

		private readonly uint destinationObjectHandleIndex;
	}
}
