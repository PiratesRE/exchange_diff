using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxDatabaseNotUniquePermanentException : MailboxReplicationPermanentException
	{
		public MailboxDatabaseNotUniquePermanentException(string mdbIdentity) : base(MrsStrings.MailboxDatabaseNotUnique(mdbIdentity))
		{
			this.mdbIdentity = mdbIdentity;
		}

		public MailboxDatabaseNotUniquePermanentException(string mdbIdentity, Exception innerException) : base(MrsStrings.MailboxDatabaseNotUnique(mdbIdentity), innerException)
		{
			this.mdbIdentity = mdbIdentity;
		}

		protected MailboxDatabaseNotUniquePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mdbIdentity = (string)info.GetValue("mdbIdentity", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mdbIdentity", this.mdbIdentity);
		}

		public string MdbIdentity
		{
			get
			{
				return this.mdbIdentity;
			}
		}

		private readonly string mdbIdentity;
	}
}
