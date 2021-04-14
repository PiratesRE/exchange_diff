using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DumpsterCouldNotFindHubServerException : DumpsterRedeliveryException
	{
		public DumpsterCouldNotFindHubServerException(string hubServerName) : base(ReplayStrings.DumpsterCouldNotFindHubServerException(hubServerName))
		{
			this.hubServerName = hubServerName;
		}

		public DumpsterCouldNotFindHubServerException(string hubServerName, Exception innerException) : base(ReplayStrings.DumpsterCouldNotFindHubServerException(hubServerName), innerException)
		{
			this.hubServerName = hubServerName;
		}

		protected DumpsterCouldNotFindHubServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.hubServerName = (string)info.GetValue("hubServerName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("hubServerName", this.hubServerName);
		}

		public string HubServerName
		{
			get
			{
				return this.hubServerName;
			}
		}

		private readonly string hubServerName;
	}
}
