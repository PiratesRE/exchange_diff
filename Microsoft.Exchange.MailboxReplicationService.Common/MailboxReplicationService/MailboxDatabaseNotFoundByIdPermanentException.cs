using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxDatabaseNotFoundByIdPermanentException : MailboxReplicationPermanentException
	{
		public MailboxDatabaseNotFoundByIdPermanentException(string mdbIdentity, LocalizedString notFoundReason) : base(MrsStrings.MailboxDatabaseNotFoundById(mdbIdentity, notFoundReason))
		{
			this.mdbIdentity = mdbIdentity;
			this.notFoundReason = notFoundReason;
		}

		public MailboxDatabaseNotFoundByIdPermanentException(string mdbIdentity, LocalizedString notFoundReason, Exception innerException) : base(MrsStrings.MailboxDatabaseNotFoundById(mdbIdentity, notFoundReason), innerException)
		{
			this.mdbIdentity = mdbIdentity;
			this.notFoundReason = notFoundReason;
		}

		protected MailboxDatabaseNotFoundByIdPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mdbIdentity = (string)info.GetValue("mdbIdentity", typeof(string));
			this.notFoundReason = (LocalizedString)info.GetValue("notFoundReason", typeof(LocalizedString));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mdbIdentity", this.mdbIdentity);
			info.AddValue("notFoundReason", this.notFoundReason);
		}

		public string MdbIdentity
		{
			get
			{
				return this.mdbIdentity;
			}
		}

		public LocalizedString NotFoundReason
		{
			get
			{
				return this.notFoundReason;
			}
		}

		private readonly string mdbIdentity;

		private readonly LocalizedString notFoundReason;
	}
}
