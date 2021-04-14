using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagNetworkRpcServerException : TaskServerException
	{
		public DagNetworkRpcServerException(string rpcName, string errMsg) : base(ReplayStrings.DagNetworkRpcServerError(rpcName, errMsg))
		{
			this.rpcName = rpcName;
			this.errMsg = errMsg;
		}

		public DagNetworkRpcServerException(string rpcName, string errMsg, Exception innerException) : base(ReplayStrings.DagNetworkRpcServerError(rpcName, errMsg), innerException)
		{
			this.rpcName = rpcName;
			this.errMsg = errMsg;
		}

		protected DagNetworkRpcServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.rpcName = (string)info.GetValue("rpcName", typeof(string));
			this.errMsg = (string)info.GetValue("errMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("rpcName", this.rpcName);
			info.AddValue("errMsg", this.errMsg);
		}

		public string RpcName
		{
			get
			{
				return this.rpcName;
			}
		}

		public string ErrMsg
		{
			get
			{
				return this.errMsg;
			}
		}

		private readonly string rpcName;

		private readonly string errMsg;
	}
}
