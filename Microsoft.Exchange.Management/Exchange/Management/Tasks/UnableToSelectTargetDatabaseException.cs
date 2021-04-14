using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToSelectTargetDatabaseException : MailboxReplicationPermanentException
	{
		public UnableToSelectTargetDatabaseException(string identity) : base(Strings.ErrorUnableToDetermineTargetDatabase(identity))
		{
			this.identity = identity;
		}

		public UnableToSelectTargetDatabaseException(string identity, Exception innerException) : base(Strings.ErrorUnableToDetermineTargetDatabase(identity), innerException)
		{
			this.identity = identity;
		}

		protected UnableToSelectTargetDatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
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
