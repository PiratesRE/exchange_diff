using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class MRSHealthCheckOutcomeObjectId : ObjectId
	{
		public MRSHealthCheckOutcomeObjectId(string server)
		{
			this.serverName = server;
		}

		public override byte[] GetBytes()
		{
			return CommonUtils.ByteSerialize(new LocalizedString(this.serverName));
		}

		public override string ToString()
		{
			return this.serverName;
		}

		private readonly string serverName;
	}
}
