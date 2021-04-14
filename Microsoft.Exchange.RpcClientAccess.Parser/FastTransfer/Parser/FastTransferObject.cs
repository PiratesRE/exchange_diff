using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class FastTransferObject : BaseObject
	{
		protected FastTransferObject(bool isTopLevel)
		{
			this.isTopLevel = isTopLevel;
		}

		protected bool IsTopLevel
		{
			get
			{
				return this.isTopLevel;
			}
		}

		private readonly bool isTopLevel;
	}
}
