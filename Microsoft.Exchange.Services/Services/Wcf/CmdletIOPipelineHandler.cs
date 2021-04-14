using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class CmdletIOPipelineHandler : TaskIOPipelineBase
	{
		public string UserPrompt { get; private set; }

		public override bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll, out bool? output)
		{
			output = new bool?(yesToAll);
			if (!yesToAll && !noToAll)
			{
				this.UserPrompt = query;
			}
			return false;
		}
	}
}
