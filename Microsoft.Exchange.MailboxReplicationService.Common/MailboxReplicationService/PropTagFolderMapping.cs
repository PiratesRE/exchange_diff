using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class PropTagFolderMapping : WellKnownFolderMapping
	{
		public PropTag Ptag { get; protected set; }
	}
}
