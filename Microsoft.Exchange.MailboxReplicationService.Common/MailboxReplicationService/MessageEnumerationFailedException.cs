using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MessageEnumerationFailedException : MailboxReplicationTransientException
	{
		public MessageEnumerationFailedException(int exists, int messagesEnumeratedCount) : base(MrsStrings.MessageEnumerationFailed(exists, messagesEnumeratedCount))
		{
			this.exists = exists;
			this.messagesEnumeratedCount = messagesEnumeratedCount;
		}

		public MessageEnumerationFailedException(int exists, int messagesEnumeratedCount, Exception innerException) : base(MrsStrings.MessageEnumerationFailed(exists, messagesEnumeratedCount), innerException)
		{
			this.exists = exists;
			this.messagesEnumeratedCount = messagesEnumeratedCount;
		}

		protected MessageEnumerationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.exists = (int)info.GetValue("exists", typeof(int));
			this.messagesEnumeratedCount = (int)info.GetValue("messagesEnumeratedCount", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("exists", this.exists);
			info.AddValue("messagesEnumeratedCount", this.messagesEnumeratedCount);
		}

		public int Exists
		{
			get
			{
				return this.exists;
			}
		}

		public int MessagesEnumeratedCount
		{
			get
			{
				return this.messagesEnumeratedCount;
			}
		}

		private readonly int exists;

		private readonly int messagesEnumeratedCount;
	}
}
