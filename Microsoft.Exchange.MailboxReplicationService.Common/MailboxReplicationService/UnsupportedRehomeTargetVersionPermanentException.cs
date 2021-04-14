using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnsupportedRehomeTargetVersionPermanentException : MailboxReplicationPermanentException
	{
		public UnsupportedRehomeTargetVersionPermanentException(string mdbID, string serverVersion) : base(MrsStrings.MustRehomeRequestToSupportedVersion(mdbID, serverVersion))
		{
			this.mdbID = mdbID;
			this.serverVersion = serverVersion;
		}

		public UnsupportedRehomeTargetVersionPermanentException(string mdbID, string serverVersion, Exception innerException) : base(MrsStrings.MustRehomeRequestToSupportedVersion(mdbID, serverVersion), innerException)
		{
			this.mdbID = mdbID;
			this.serverVersion = serverVersion;
		}

		protected UnsupportedRehomeTargetVersionPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mdbID = (string)info.GetValue("mdbID", typeof(string));
			this.serverVersion = (string)info.GetValue("serverVersion", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mdbID", this.mdbID);
			info.AddValue("serverVersion", this.serverVersion);
		}

		public string MdbID
		{
			get
			{
				return this.mdbID;
			}
		}

		public string ServerVersion
		{
			get
			{
				return this.serverVersion;
			}
		}

		private readonly string mdbID;

		private readonly string serverVersion;
	}
}
