using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceResumeRpcPartialSuccessCatalogFailedException : TaskServerException
	{
		public ReplayServiceResumeRpcPartialSuccessCatalogFailedException(string errMsg) : base(ReplayStrings.ReplayServiceResumeRpcPartialSuccessCatalogFailedException(errMsg))
		{
			this.errMsg = errMsg;
		}

		public ReplayServiceResumeRpcPartialSuccessCatalogFailedException(string errMsg, Exception innerException) : base(ReplayStrings.ReplayServiceResumeRpcPartialSuccessCatalogFailedException(errMsg), innerException)
		{
			this.errMsg = errMsg;
		}

		protected ReplayServiceResumeRpcPartialSuccessCatalogFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
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
