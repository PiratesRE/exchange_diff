using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RequestJobAlreadyOnProperQueuePermanentException : MailboxReplicationPermanentException
	{
		public RequestJobAlreadyOnProperQueuePermanentException(string identity, string queue) : base(Strings.ErrorRequestJobAlreadyOnProperQueue(identity, queue))
		{
			this.identity = identity;
			this.queue = queue;
		}

		public RequestJobAlreadyOnProperQueuePermanentException(string identity, string queue, Exception innerException) : base(Strings.ErrorRequestJobAlreadyOnProperQueue(identity, queue), innerException)
		{
			this.identity = identity;
			this.queue = queue;
		}

		protected RequestJobAlreadyOnProperQueuePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.identity = (string)info.GetValue("identity", typeof(string));
			this.queue = (string)info.GetValue("queue", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identity", this.identity);
			info.AddValue("queue", this.queue);
		}

		public string Identity
		{
			get
			{
				return this.identity;
			}
		}

		public string Queue
		{
			get
			{
				return this.queue;
			}
		}

		private readonly string identity;

		private readonly string queue;
	}
}
