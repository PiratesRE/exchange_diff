using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public class RpcClientException : Exception
	{
		private RpcClientException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected RpcClientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		internal static Exception TranslateRpcException(Exception exception)
		{
			RpcException ex = exception as RpcException;
			if (ex == null)
			{
				return exception;
			}
			int errorCode = ex.ErrorCode;
			if (errorCode == 1753)
			{
				return new RpcClientException(Strings.ServerNotFound((ex != null) ? ex.Source : string.Empty), ex);
			}
			return new Win32Exception(ex.ErrorCode);
		}
	}
}
