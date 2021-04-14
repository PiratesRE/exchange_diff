using System;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	internal enum PswsErrorCode
	{
		AuthZUserError = 830001,
		MemberShipIdError,
		OverBudgetException,
		GetISSError,
		ClientAccessRuleBlock,
		CmdletExecutionFailure = 840001,
		CmdletShouldContinue,
		RetriableTransientException = 841101,
		ParameterValidationFailed = 842001,
		DuplicateObjectCreation,
		ScopePermissionDenied = 843001,
		UnkownFailure = 849999
	}
}
