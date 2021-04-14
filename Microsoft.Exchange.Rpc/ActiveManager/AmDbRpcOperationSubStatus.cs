using System;
using System.Text;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Rpc.ActiveManager
{
	[Serializable]
	internal sealed class AmDbRpcOperationSubStatus
	{
		private void BuildDebugString()
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			stringBuilder.Append("AmDbRpcOperationSubStatus: [ ");
			stringBuilder.AppendFormat("ServerAttempted='{0}', ", this.m_serverFqdnAttempted);
			AmAcllReturnStatus acllStatus = this.m_acllStatus;
			if (acllStatus != null)
			{
				stringBuilder.AppendFormat("AmAcllReturnStatus='{0}', ", acllStatus.ToString());
			}
			RpcErrorExceptionInfo errorInfo = this.m_errorInfo;
			if (errorInfo != null)
			{
				stringBuilder.AppendFormat("RpcErrorExceptionInfo='{0}' ", errorInfo.ToString());
			}
			stringBuilder.Append("]");
			this.m_debugString = stringBuilder.ToString();
		}

		public AmDbRpcOperationSubStatus(string serverFqdnAttempted, AmAcllReturnStatus acllStatus, RpcErrorExceptionInfo errorInfo)
		{
			this.BuildDebugString();
		}

		public sealed override string ToString()
		{
			return this.m_debugString;
		}

		public string ServerAttemptedFqdn
		{
			get
			{
				return this.m_serverFqdnAttempted;
			}
		}

		public AmAcllReturnStatus AcllStatus
		{
			get
			{
				return this.m_acllStatus;
			}
		}

		public RpcErrorExceptionInfo ErrorInfo
		{
			get
			{
				return this.m_errorInfo;
			}
		}

		private readonly string m_serverFqdnAttempted = serverFqdnAttempted;

		private readonly AmAcllReturnStatus m_acllStatus = acllStatus;

		private readonly RpcErrorExceptionInfo m_errorInfo = errorInfo;

		private string m_debugString;
	}
}
