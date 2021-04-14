using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDbOperationSubStatus
	{
		public AmDbOperationSubStatus(AmServerName serverAttempted, AmAcllReturnStatus acllStatus, Exception exception)
		{
			this.ServerAttempted = serverAttempted;
			this.AcllReturnStatus = acllStatus;
			this.LastException = exception;
		}

		internal AmServerName ServerAttempted { get; private set; }

		internal AmAcllReturnStatus AcllReturnStatus { get; private set; }

		internal Exception LastException { get; private set; }

		public AmDbRpcOperationSubStatus ConvertToRpcSubStatus()
		{
			RpcErrorExceptionInfo errorInfo = AmRpcExceptionWrapper.Instance.ConvertExceptionToErrorExceptionInfo(this.LastException);
			return new AmDbRpcOperationSubStatus(this.ServerAttempted.Fqdn, this.AcllReturnStatus, errorInfo);
		}
	}
}
