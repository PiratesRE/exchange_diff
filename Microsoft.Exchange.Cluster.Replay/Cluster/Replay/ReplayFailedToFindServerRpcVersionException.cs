using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayFailedToFindServerRpcVersionException : TaskServerTransientException
	{
		public ReplayFailedToFindServerRpcVersionException(string serverName) : base(ReplayStrings.ReplayFailedToFindServerRpcVersionException(serverName))
		{
			this.serverName = serverName;
		}

		public ReplayFailedToFindServerRpcVersionException(string serverName, Exception innerException) : base(ReplayStrings.ReplayFailedToFindServerRpcVersionException(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected ReplayFailedToFindServerRpcVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
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
