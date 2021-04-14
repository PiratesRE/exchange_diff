using System;

namespace Microsoft.Exchange.Data.Storage.Cluster
{
	public interface IHaRpcServerBaseException
	{
		string ErrorMessage { get; }

		string OriginatingServer { get; }

		string OriginatingStackTrace { get; }

		string StackTrace { get; }

		string DatabaseName { get; }

		Exception InnerException { get; }
	}
}
