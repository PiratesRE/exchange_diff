using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.JobQueues;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class EnqueueRequestRpcOutParameters : RpcParameters
	{
		public EnqueueResult Result { get; private set; }

		public EnqueueRequestRpcOutParameters(byte[] data) : base(data)
		{
			this.Result = (EnqueueResult)base.GetParameterValue("EnqueueResult");
		}

		public EnqueueRequestRpcOutParameters(EnqueueResult enqueueResult)
		{
			this.Result = enqueueResult;
			base.SetParameterValue("EnqueueResult", this.Result);
		}

		private const string EnqueueResultParameterName = "EnqueueResult";
	}
}
