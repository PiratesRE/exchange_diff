using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DumpsterSafetyNetRpcFailedException : DumpsterRedeliveryException
	{
		public DumpsterSafetyNetRpcFailedException(string hubServerName, string rpcStatus) : base(ReplayStrings.DumpsterSafetyNetRpcFailedException(hubServerName, rpcStatus))
		{
			this.hubServerName = hubServerName;
			this.rpcStatus = rpcStatus;
		}

		public DumpsterSafetyNetRpcFailedException(string hubServerName, string rpcStatus, Exception innerException) : base(ReplayStrings.DumpsterSafetyNetRpcFailedException(hubServerName, rpcStatus), innerException)
		{
			this.hubServerName = hubServerName;
			this.rpcStatus = rpcStatus;
		}

		protected DumpsterSafetyNetRpcFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.hubServerName = (string)info.GetValue("hubServerName", typeof(string));
			this.rpcStatus = (string)info.GetValue("rpcStatus", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("hubServerName", this.hubServerName);
			info.AddValue("rpcStatus", this.rpcStatus);
		}

		public string HubServerName
		{
			get
			{
				return this.hubServerName;
			}
		}

		public string RpcStatus
		{
			get
			{
				return this.rpcStatus;
			}
		}

		private readonly string hubServerName;

		private readonly string rpcStatus;
	}
}
