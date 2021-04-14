using System;
using System.Management.Automation.Runspaces;

namespace Microsoft.Exchange.Management.Common
{
	internal class ScriptParseResult
	{
		public bool IsSuccessful { get; set; }

		public string Command { get; set; }

		public CommandParameterCollection Parameters { get; set; }

		public ScriptParseResult()
		{
			this.IsSuccessful = false;
			this.Command = string.Empty;
			this.Parameters = new CommandParameterCollection();
		}
	}
}
