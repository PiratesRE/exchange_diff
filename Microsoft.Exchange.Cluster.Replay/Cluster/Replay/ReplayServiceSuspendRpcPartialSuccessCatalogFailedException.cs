using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceSuspendRpcPartialSuccessCatalogFailedException : TaskServerException
	{
		public ReplayServiceSuspendRpcPartialSuccessCatalogFailedException(string errMsg) : base(ReplayStrings.ReplayServiceSuspendRpcPartialSuccessCatalogFailedException(errMsg))
		{
			this.errMsg = errMsg;
		}

		public ReplayServiceSuspendRpcPartialSuccessCatalogFailedException(string errMsg, Exception innerException) : base(ReplayStrings.ReplayServiceSuspendRpcPartialSuccessCatalogFailedException(errMsg), innerException)
		{
			this.errMsg = errMsg;
		}

		protected ReplayServiceSuspendRpcPartialSuccessCatalogFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errMsg = (string)info.GetValue("errMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errMsg", this.errMsg);
		}

		public string ErrMsg
		{
			get
			{
				return this.errMsg;
			}
		}

		private readonly string errMsg;
	}
}
