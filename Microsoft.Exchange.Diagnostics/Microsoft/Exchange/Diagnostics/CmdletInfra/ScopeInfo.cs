using System;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal class ScopeInfo
	{
		public ScopeInfo(string groupName, string funcName)
		{
			this.GroupName = groupName;
			this.FuncName = funcName;
		}

		public string GroupName { get; private set; }

		public string FuncName { get; private set; }
	}
}
