using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmRpcOperationNotImplemented : AmServerTransientException
	{
		public AmRpcOperationNotImplemented(string operationHint, string serverName) : base(ServerStrings.AmRpcOperationNotImplemented(operationHint, serverName))
		{
			this.operationHint = operationHint;
			this.serverName = serverName;
		}

		public AmRpcOperationNotImplemented(string operationHint, string serverName, Exception innerException) : base(ServerStrings.AmRpcOperationNotImplemented(operationHint, serverName), innerException)
		{
			this.operationHint = operationHint;
			this.serverName = serverName;
		}

		protected AmRpcOperationNotImplemented(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.operationHint = (string)info.GetValue("operationHint", typeof(string));
			this.serverName = (string)info.GetValue("serverName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("operationHint", this.operationHint);
			info.AddValue("serverName", this.serverName);
		}

		public string OperationHint
		{
			get
			{
				return this.operationHint;
			}
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		private readonly string operationHint;

		private readonly string serverName;
	}
}
