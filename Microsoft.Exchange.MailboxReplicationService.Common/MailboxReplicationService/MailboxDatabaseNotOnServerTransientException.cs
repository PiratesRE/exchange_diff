using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxDatabaseNotOnServerTransientException : MailboxReplicationTransientException
	{
		public MailboxDatabaseNotOnServerTransientException(string mdbName, Guid mdbId, string mdbServerFqdn, string currentServerFqdn) : base(MrsStrings.MdbNotOnServer(mdbName, mdbId, mdbServerFqdn, currentServerFqdn))
		{
			this.mdbName = mdbName;
			this.mdbId = mdbId;
			this.mdbServerFqdn = mdbServerFqdn;
			this.currentServerFqdn = currentServerFqdn;
		}

		public MailboxDatabaseNotOnServerTransientException(string mdbName, Guid mdbId, string mdbServerFqdn, string currentServerFqdn, Exception innerException) : base(MrsStrings.MdbNotOnServer(mdbName, mdbId, mdbServerFqdn, currentServerFqdn), innerException)
		{
			this.mdbName = mdbName;
			this.mdbId = mdbId;
			this.mdbServerFqdn = mdbServerFqdn;
			this.currentServerFqdn = currentServerFqdn;
		}

		protected MailboxDatabaseNotOnServerTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mdbName = (string)info.GetValue("mdbName", typeof(string));
			this.mdbId = (Guid)info.GetValue("mdbId", typeof(Guid));
			this.mdbServerFqdn = (string)info.GetValue("mdbServerFqdn", typeof(string));
			this.currentServerFqdn = (string)info.GetValue("currentServerFqdn", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mdbName", this.mdbName);
			info.AddValue("mdbId", this.mdbId);
			info.AddValue("mdbServerFqdn", this.mdbServerFqdn);
			info.AddValue("currentServerFqdn", this.currentServerFqdn);
		}

		public string MdbName
		{
			get
			{
				return this.mdbName;
			}
		}

		public Guid MdbId
		{
			get
			{
				return this.mdbId;
			}
		}

		public string MdbServerFqdn
		{
			get
			{
				return this.mdbServerFqdn;
			}
		}

		public string CurrentServerFqdn
		{
			get
			{
				return this.currentServerFqdn;
			}
		}

		private readonly string mdbName;

		private readonly Guid mdbId;

		private readonly string mdbServerFqdn;

		private readonly string currentServerFqdn;
	}
}
