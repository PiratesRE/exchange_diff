using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Security.OAuth
{
	[Serializable]
	public sealed class ValidationResultNodeId : ObjectId
	{
		public override byte[] GetBytes()
		{
			return null;
		}
	}
}
