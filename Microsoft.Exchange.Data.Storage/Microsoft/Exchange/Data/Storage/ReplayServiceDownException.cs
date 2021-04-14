using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceDownException : TaskServerException
	{
		public ReplayServiceDownException(string serverName, string rpcErrorMessage) : base(ServerStrings.ReplayServiceDown(serverName, rpcErrorMessage))
		{
			this.serverName = serverName;
			this.rpcErrorMessage = rpcErrorMessage;
		}

		public ReplayServiceDownException(string serverName, string rpcErrorMessage, Exception innerException) : base(ServerStrings.ReplayServiceDown(serverName, rpcErrorMessage), innerException)
		{
			this.serverName = serverName;
			this.rpcErrorMessage = rpcErrorMessage;
		}

		protected ReplayServiceDownException(SerializationInfo info, StreamingContext context) : base(info, context)
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
