using System;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClusterIPNotFoundPermanentException : MailboxReplicationPermanentException
	{
		public ClusterIPNotFoundPermanentException(IPAddress clusterIp) : base(MrsStrings.ClusterIPNotFound(clusterIp))
		{
			this.clusterIp = clusterIp;
		}

		public ClusterIPNotFoundPermanentException(IPAddress clusterIp, Exception innerException) : base(MrsStrings.ClusterIPNotFound(clusterIp), innerException)
		{
			this.clusterIp = clusterIp;
		}

		protected ClusterIPNotFoundPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.clusterIp = (IPAddress)info.GetValue("clusterIp", typeof(IPAddress));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("clusterIp", this.clusterIp);
		}

		public IPAddress ClusterIp
		{
			get
			{
				return this.clusterIp;
			}
		}

		private readonly IPAddress clusterIp;
	}
}
