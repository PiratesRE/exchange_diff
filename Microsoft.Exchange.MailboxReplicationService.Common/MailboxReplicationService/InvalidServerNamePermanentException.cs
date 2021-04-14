using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidServerNamePermanentException : MailboxReplicationPermanentException
	{
		public InvalidServerNamePermanentException(string serverName) : base(MrsStrings.InvalidServerName(serverName))
		{
			this.serverName = serverName;
		}

		public InvalidServerNamePermanentException(string serverName, Exception innerException) : base(MrsStrings.InvalidServerName(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected InvalidServerNamePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		private readonly string serverName;
	}
}
