using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LogonProgressToken
	{
		internal LogonProgressToken(RopId ropId, byte logonId)
		{
			this.ropId = ropId;
			this.logonId = logonId;
		}

		internal RopId RopId
		{
			get
			{
				return this.ropId;
			}
		}

		internal byte LogonId
		{
			get
			{
				return this.logonId;
			}
		}

		private readonly RopId ropId;

		private readonly byte logonId;
	}
}
