using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public interface ITaskIOPipeline
	{
		bool WriteVerbose(LocalizedString input, out LocalizedString output);

		bool WriteDebug(LocalizedString input, out LocalizedString output);

		bool WriteWarning(LocalizedString input, string helperUrl, out LocalizedString output);

		bool WriteError(TaskErrorInfo input, out TaskErrorInfo output);

		bool WriteObject(object input, out object output);

		bool WriteProgress(ExProgressRecord input, out ExProgressRecord output);

		bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll, out bool? output);

		bool ShouldProcess(string verboseDescription, string verboseWarning, string caption, out bool? output);
	}
}
