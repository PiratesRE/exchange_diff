using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NullPeopleIKnowPublisher : IPeopleIKnowPublisher
	{
		private NullPeopleIKnowPublisher()
		{
		}

		public void Publish(IMailboxSession mailbox)
		{
		}

		public static readonly NullPeopleIKnowPublisher Instance = new NullPeopleIKnowPublisher();
	}
}
