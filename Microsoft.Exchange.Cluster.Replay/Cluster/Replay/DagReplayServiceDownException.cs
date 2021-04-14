using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagReplayServiceDownException : DagTaskServerException
	{
		public DagReplayServiceDownException(string serverName, string rpcErrorMessage) : base(ReplayStrings.DagReplayServiceDownException(serverName, rpcErrorMessage))
		{
			this.serverName = serverName;
			this.rpcErrorMessage = rpcErrorMessage;
		}

		public DagReplayServiceDownException(string serverName, string rpcErrorMessage, Exception innerException) : base(ReplayStrings.DagReplayServiceDownException(serverName, rpcErrorMessage), innerException)
		{
			this.serverName = serverName;
			this.rpcErrorMessage = rpcErrorMessage;
		}

		protected DagReplayServiceDownException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
			this.rpcErrorMessage = (string)info.GetValue("rpcErrorMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
			info.AddValue("rpcErrorMessage", this.rpcErrorMessage);
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public string RpcErrorMessage
		{
			get
			{
				return this.rpcErrorMessage;
			}
		}

		private readonly string serverName;

		private readonly string rpcErrorMessage;
	}
}
