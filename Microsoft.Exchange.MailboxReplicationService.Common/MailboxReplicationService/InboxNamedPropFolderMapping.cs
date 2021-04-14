using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class InboxNamedPropFolderMapping : InboxFolderMapping
	{
		public InboxNamedPropFolderMapping(WellKnownFolderType wkft, NamedPropData namedPropData) : base(wkft, PropTag.Null)
		{
			this.NamedPropData = namedPropData;
		}

		public NamedPropData NamedPropData { get; protected set; }
	}
}
