using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InvalidRuleException : StoragePermanentException
	{
		public InvalidRuleException(string message) : base(new LocalizedString(message))
		{
		}

		public InvalidRuleException(string message, Exception innerException) : base(new LocalizedString(message), innerException)
		{
		}
	}
}
