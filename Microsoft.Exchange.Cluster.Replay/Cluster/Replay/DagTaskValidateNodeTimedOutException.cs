using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskValidateNodeTimedOutException : DagTaskServerException
	{
		public DagTaskValidateNodeTimedOutException(string serverName) : base(ReplayStrings.DagTaskValidateNodeTimedOutException(serverName))
		{
			this.serverName = serverName;
		}

		public DagTaskValidateNodeTimedOutException(string serverName, Exception innerException) : base(ReplayStrings.DagTaskValidateNodeTimedOutException(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected DagTaskValidateNodeTimedOutException(SerializationInfo info, StreamingContext context) : base(info, context)
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
