using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServerNotFoundException : TransientException
	{
		public ServerNotFoundException(string serverName) : base(ReplayStrings.ServerNotFoundException(serverName))
		{
			this.serverName = serverName;
		}

		public ServerNotFoundException(string serverName, Exception innerException) : base(ReplayStrings.ServerNotFoundException(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected ServerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
