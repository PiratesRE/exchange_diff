using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CrossSiteLogonTransientException : MailboxReplicationTransientException
	{
		public CrossSiteLogonTransientException(Guid mdbGuid, Guid serverGuid, string serverSite, string localSite) : base(MrsStrings.CrossSiteError(mdbGuid, serverGuid, serverSite, localSite))
		{
			this.mdbGuid = mdbGuid;
			this.serverGuid = serverGuid;
			this.serverSite = serverSite;
			this.localSite = localSite;
		}

		public CrossSiteLogonTransientException(Guid mdbGuid, Guid serverGuid, string serverSite, string localSite, Exception innerException) : base(MrsStrings.CrossSiteError(mdbGuid, serverGuid, serverSite, localSite), innerException)
		{
			this.mdbGuid = mdbGuid;
			this.serverGuid = serverGuid;
			this.serverSite = serverSite;
			this.localSite = localSite;
		}

		protected CrossSiteLogonTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mdbGuid = (Guid)info.GetValue("mdbGuid", typeof(Guid));
			this.serverGuid = (Guid)info.GetValue("serverGuid", typeof(Guid));
			this.serverSite = (string)info.GetValue("serverSite", typeof(string));
			this.localSite = (string)info.GetValue("localSite", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mdbGuid", this.mdbGuid);
			info.AddValue("serverGuid", this.serverGuid);
			info.AddValue("serverSite", this.serverSite);
			info.AddValue("localSite", this.localSite);
		}

		public Guid MdbGuid
		{
			get
			{
				return this.mdbGuid;
			}
		}

		public Guid ServerGuid
		{
			get
			{
				return this.serverGuid;
			}
		}

		public string ServerSite
		{
			get
			{
				return this.serverSite;
			}
		}

		public string LocalSite
		{
			get
			{
				return this.localSite;
			}
		}

		private readonly Guid mdbGuid;

		private readonly Guid serverGuid;

		private readonly string serverSite;

		private readonly string localSite;
	}
}
