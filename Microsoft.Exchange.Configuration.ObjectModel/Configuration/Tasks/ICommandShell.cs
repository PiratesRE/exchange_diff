using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal interface ICommandShell
	{
		void WriteObject(object sendToPipeline);

		void WriteError(LocalizedException exception, ExchangeErrorCategory category, object target);

		void WriteError(LocalizedException exception, ExchangeErrorCategory category, object target, bool reThrow);

		void ThrowTerminatingError(LocalizedException exception, ExchangeErrorCategory category, object target);

		void WriteVerbose(LocalizedString message);

		void WriteDebug(LocalizedString message);

		void WriteWarning(LocalizedString message);

		void WriteProgress(ExProgressRecord record);

		bool ShouldContinue(LocalizedString promptMessage);

		bool ShouldProcess(LocalizedString promptMessage);

		void SetShouldExit(int exitCode);

		bool TryGetVariableValue<T>(string name, out T value);

		void PrependTaskIOPipelineHandler(ITaskIOPipeline pipeline);
	}
}
