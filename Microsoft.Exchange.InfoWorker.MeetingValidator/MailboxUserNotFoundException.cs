using System;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	internal class MailboxUserNotFoundException : Exception
	{
		internal MailboxUserNotFoundException(string user) : base(string.Format("User not found {0}", user))
		{
		}
	}
}
