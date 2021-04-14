using System;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal interface IUnhandledExceptionHandler
	{
		void OnUnhandledException();
	}
}
