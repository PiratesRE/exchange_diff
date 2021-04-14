using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServerNotFoundPermanentException : MailboxReplicationPermanentException
	{
		public ServerNotFoundPermanentException(string serverLegDN) : base(MrsStrings.ServerNotFound(serverLegDN))
		{
			this.serverLegDN = serverLegDN;
		}

		public ServerNotFoundPermanentException(string serverLegDN, Exception innerException) : base(MrsStrings.ServerNotFound(serverLegDN), innerException)
		{
			this.serverLegDN = serverLegDN;
		}

		protected ServerNotFoundPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverLegDN = (string)info.GetValue("serverLegDN", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverLegDN", this.serverLegDN);
		}

		public string ServerLegDN
		{
			get
			{
				return this.serverLegDN;
			}
		}

		private readonly string serverLegDN;
	}
}
