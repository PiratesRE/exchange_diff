using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ILocatableMailbox
	{
		IMailboxLocator Locator { get; }
	}
}
