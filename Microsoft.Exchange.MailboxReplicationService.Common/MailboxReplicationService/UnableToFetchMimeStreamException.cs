using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToFetchMimeStreamException : MailboxReplicationTransientException
	{
		public UnableToFetchMimeStreamException(string identity) : base(MrsStrings.UnableToFetchMimeStream(identity))
		{
			this.identity = identity;
		}

		public UnableToFetchMimeStreamException(string identity, Exception innerException) : base(MrsStrings.UnableToFetchMimeStream(identity), innerException)
		{
			this.identity = identity;
		}

		protected UnableToFetchMimeStreamException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.identity = (string)info.GetValue("identity", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identity", this.identity);
		}

		public string Identity
		{
			get
			{
				return this.identity;
			}
		}

		private readonly string identity;
	}
}
