using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotFindDagObjectForServer : TransientException
	{
		public CouldNotFindDagObjectForServer(string serverName) : base(ReplayStrings.CouldNotFindDagObjectForServer(serverName))
		{
			this.serverName = serverName;
		}

		public CouldNotFindDagObjectForServer(string serverName, Exception innerException) : base(ReplayStrings.CouldNotFindDagObjectForServer(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected CouldNotFindDagObjectForServer(SerializationInfo info, StreamingContext context) : base(info, context)
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
