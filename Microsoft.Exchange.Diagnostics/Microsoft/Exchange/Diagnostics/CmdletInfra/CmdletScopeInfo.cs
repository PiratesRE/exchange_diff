using System;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal class CmdletScopeInfo : ScopeInfo
	{
		public CmdletScopeInfo(Guid cmdletUniqueId, string groupName, string funcName) : base(groupName, funcName)
		{
			this.CmdletUniqueId = cmdletUniqueId;
		}

		public Guid CmdletUniqueId { get; private set; }
	}
}
