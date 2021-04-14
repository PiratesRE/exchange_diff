using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class NamedPropertyData
	{
		public NamedPropertyData(Guid npGuid, string npName, PropType npType)
		{
			this.NPGuid = npGuid;
			this.NPName = npName;
			this.NPType = npType;
		}

		public Guid NPGuid { get; private set; }

		public string NPName { get; private set; }

		public PropType NPType { get; private set; }
	}
}
