using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnsupportedImapMessageEntryIdVersionException : MailboxReplicationPermanentException
	{
		public UnsupportedImapMessageEntryIdVersionException(string version) : base(MrsStrings.UnsupportedImapMessageEntryIdVersion(version))
		{
			this.version = version;
		}

		public UnsupportedImapMessageEntryIdVersionException(string version, Exception innerException) : base(MrsStrings.UnsupportedImapMessageEntryIdVersion(version), innerException)
		{
			this.version = version;
		}

		protected UnsupportedImapMessageEntryIdVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.version = (string)info.GetValue("version", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("version", this.version);
		}

		public string Version
		{
			get
			{
				return this.version;
			}
		}

		private readonly string version;
	}
}
