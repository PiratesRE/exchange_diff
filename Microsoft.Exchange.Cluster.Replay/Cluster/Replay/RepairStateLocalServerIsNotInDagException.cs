using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RepairStateLocalServerIsNotInDagException : RepairStateException
	{
		public RepairStateLocalServerIsNotInDagException(string serverName) : base(ReplayStrings.RepairStateLocalServerIsNotInDag(serverName))
		{
			this.serverName = serverName;
		}

		public RepairStateLocalServerIsNotInDagException(string serverName, Exception innerException) : base(ReplayStrings.RepairStateLocalServerIsNotInDag(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected RepairStateLocalServerIsNotInDagException(SerializationInfo info, StreamingContext context) : base(info, context)
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
