using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxTransport.Shared.SubmissionItem
{
	internal class QuarantineMailboxConfigurationLoadException : LocalizedException
	{
		internal QuarantineMailboxConfigurationLoadException(string errorString) : base(new LocalizedString(errorString))
		{
		}
	}
}
