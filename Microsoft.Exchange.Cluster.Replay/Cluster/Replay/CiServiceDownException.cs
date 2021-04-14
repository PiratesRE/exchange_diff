using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CiServiceDownException : SeederServerException
	{
		public CiServiceDownException(string serverName, string rpcErrorMessage) : base(ReplayStrings.CiServiceDownException(serverName, rpcErrorMessage))
		{
			this.serverName = serverName;
			this.rpcErrorMessage = rpcErrorMessage;
		}

		public CiServiceDownException(string serverName, string rpcErrorMessage, Exception innerException) : base(ReplayStrings.CiServiceDownException(serverName, rpcErrorMessage), innerException)
		{
			this.serverName = serverName;
			this.rpcErrorMessage = rpcErrorMessage;
		}

		protected CiServiceDownException(SerializationInfo info, StreamingContext context) : base(info, context)
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
