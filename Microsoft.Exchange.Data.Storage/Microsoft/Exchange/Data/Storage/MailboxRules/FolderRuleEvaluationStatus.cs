using System;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	internal enum FolderRuleEvaluationStatus
	{
		Continue,
		InterruptWithException,
		InterruptSilently
	}
}
