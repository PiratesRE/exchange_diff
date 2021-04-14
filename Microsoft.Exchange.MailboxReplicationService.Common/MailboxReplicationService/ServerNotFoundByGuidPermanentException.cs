using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServerNotFoundByGuidPermanentException : MailboxReplicationPermanentException
	{
		public ServerNotFoundByGuidPermanentException(Guid serverGuid) : base(MrsStrings.ServerNotFoundByGuid(serverGuid))
		{
			this.serverGuid = serverGuid;
		}

		public ServerNotFoundByGuidPermanentException(Guid serverGuid, Exception innerException) : base(MrsStrings.ServerNotFoundByGuid(serverGuid), innerException)
		{
			this.serverGuid = serverGuid;
		}

		protected ServerNotFoundByGuidPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverGuid = (Guid)info.GetValue("serverGuid", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverGuid", this.serverGuid);
		}

		public Guid ServerGuid
		{
			get
			{
				return this.serverGuid;
			}
		}

		private readonly Guid serverGuid;
	}
}
