using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum RuleStateFlags
	{
		Enabled = 1,
		ProcessingError = 2,
		OnlyEnabledWhenOOF = 4,
		KeepOOFHistoryList = 8,
		TerminateAfterExecution = 16,
		SkipRuleIfNotSpam = 32,
		ParsingError = 64
	}
}
