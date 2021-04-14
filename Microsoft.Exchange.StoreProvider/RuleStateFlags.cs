using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum RuleStateFlags
	{
		Enabled = 1,
		Error = 2,
		OnlyWhenOOF = 4,
		KeepOOFHistory = 8,
		ExitAfterExecution = 16,
		SkipIfSCLIsSafe = 32,
		RuleParseError = 64,
		LegacyOofRule = 128,
		OnlyWhenOOFEx = 256,
		TempDisabled = 1073741824,
		ClearOOFHistory = -2147483648
	}
}
