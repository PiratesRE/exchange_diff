using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AggregatedMailboxNotFoundPermanentException : MailboxReplicationPermanentException
	{
		public AggregatedMailboxNotFoundPermanentException(Guid aggregatedMailboxGuid, string identity) : base(Strings.ErrorAggregatedMailboxNotFound(aggregatedMailboxGuid, identity))
		{
			this.aggregatedMailboxGuid = aggregatedMailboxGuid;
			this.identity = identity;
		}

		public AggregatedMailboxNotFoundPermanentException(Guid aggregatedMailboxGuid, string identity, Exception innerException) : base(Strings.ErrorAggregatedMailboxNotFound(aggregatedMailboxGuid, identity), innerException)
		{
			this.aggregatedMailboxGuid = aggregatedMailboxGuid;
			this.identity = identity;
		}

		protected AggregatedMailboxNotFoundPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.aggregatedMailboxGuid = (Guid)info.GetValue("aggregatedMailboxGuid", typeof(Guid));
			this.identity = (string)info.GetValue("identity", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("aggregatedMailboxGuid", this.aggregatedMailboxGuid);
			info.AddValue("identity", this.identity);
		}

		public Guid AggregatedMailboxGuid
		{
			get
			{
				return this.aggregatedMailboxGuid;
			}
		}

		public string Identity
		{
			get
			{
				return this.identity;
			}
		}

		private readonly Guid aggregatedMailboxGuid;

		private readonly string identity;
	}
}
