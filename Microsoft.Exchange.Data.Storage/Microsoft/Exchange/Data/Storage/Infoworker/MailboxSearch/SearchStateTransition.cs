using System;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	internal enum SearchStateTransition
	{
		StartSearch,
		DeleteSearch,
		StopSearch,
		ResetSearch,
		MoveToNextState,
		MoveToNextPartialSuccessState,
		Fail
	}
}
