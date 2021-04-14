using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public class FailureEvent : IFailureObjectLoggable
	{
		public FailureEvent(Guid objectGuid, string objectType, int flags, string failureContext)
		{
			this.ObjectGuid = objectGuid;
			this.ObjectType = objectType;
			this.Flags = flags;
			this.FailureContext = failureContext;
		}

		public Guid ObjectGuid { get; set; }

		public string ObjectType { get; set; }

		public int Flags { get; set; }

		public string FailureContext { get; set; }
	}
}
