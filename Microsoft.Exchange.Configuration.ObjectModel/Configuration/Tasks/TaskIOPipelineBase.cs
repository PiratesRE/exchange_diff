using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public class TaskIOPipelineBase : ITaskIOPipeline
	{
		public virtual bool WriteVerbose(LocalizedString input, out LocalizedString output)
		{
			output = input;
			return true;
		}

		public virtual bool WriteDebug(LocalizedString input, out LocalizedString output)
		{
			output = input;
			return true;
		}

		public virtual bool WriteWarning(LocalizedString input, string helperUrl, out LocalizedString output)
		{
			output = input;
			return true;
		}

		public virtual bool WriteError(TaskErrorInfo input, out TaskErrorInfo output)
		{
			output = input;
			return true;
		}

		public virtual bool WriteObject(object input, out object output)
		{
			output = input;
			return true;
		}

		public virtual bool WriteProgress(ExProgressRecord input, out ExProgressRecord output)
		{
			output = input;
			return true;
		}

		public virtual bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll, out bool? output)
		{
			output = null;
			return true;
		}

		public virtual bool ShouldProcess(string verboseDescription, string verboseWarning, string caption, out bool? output)
		{
			output = null;
			return true;
		}
	}
}
