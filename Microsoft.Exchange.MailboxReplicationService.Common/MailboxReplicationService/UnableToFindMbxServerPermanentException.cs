using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToFindMbxServerPermanentException : MailboxReplicationPermanentException
	{
		public UnableToFindMbxServerPermanentException(string server) : base(MrsStrings.UnableToFindMbxServer(server))
		{
			this.server = server;
		}

		public UnableToFindMbxServerPermanentException(string server, Exception innerException) : base(MrsStrings.UnableToFindMbxServer(server), innerException)
		{
			this.server = server;
		}

		protected UnableToFindMbxServerPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.server = (string)info.GetValue("server", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("server", this.server);
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		private readonly string server;
	}
}
