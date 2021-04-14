using System;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	public enum OptionsActionError
	{
		Unexpected,
		None,
		CmdletError,
		PermissionDenied,
		PromptUser
	}
}
