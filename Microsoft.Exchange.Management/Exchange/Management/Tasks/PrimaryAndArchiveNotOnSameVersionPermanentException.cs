using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PrimaryAndArchiveNotOnSameVersionPermanentException : MailboxReplicationPermanentException
	{
		public PrimaryAndArchiveNotOnSameVersionPermanentException(string primaryVersion, string archiveVersion) : base(Strings.PrimaryAndArchiveNotOnSameVersionPostMove(primaryVersion, archiveVersion))
		{
			this.primaryVersion = primaryVersion;
			this.archiveVersion = archiveVersion;
		}

		public PrimaryAndArchiveNotOnSameVersionPermanentException(string primaryVersion, string archiveVersion, Exception innerException) : base(Strings.PrimaryAndArchiveNotOnSameVersionPostMove(primaryVersion, archiveVersion), innerException)
		{
			this.primaryVersion = primaryVersion;
			this.archiveVersion = archiveVersion;
		}

		protected PrimaryAndArchiveNotOnSameVersionPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.primaryVersion = (string)info.GetValue("primaryVersion", typeof(string));
			this.archiveVersion = (string)info.GetValue("archiveVersion", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("primaryVersion", this.primaryVersion);
			info.AddValue("archiveVersion", this.archiveVersion);
		}

		public string PrimaryVersion
		{
			get
			{
				return this.primaryVersion;
			}
		}

		public string ArchiveVersion
		{
			get
			{
				return this.archiveVersion;
			}
		}

		private readonly string primaryVersion;

		private readonly string archiveVersion;
	}
}
