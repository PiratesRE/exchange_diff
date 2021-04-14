using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class WellKnownFolderMapping
	{
		public WellKnownFolderType WKFType { get; protected set; }
	}
}
