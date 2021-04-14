using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal interface IRpcService : IDisposable
	{
		string Name { get; }

		bool IsEnabled();

		void OnStartBegin();

		void OnStartEnd();

		void OnStopBegin();

		void OnStopEnd();

		void HandleUnexpectedExceptionOnStart(Exception ex);

		void HandleUnexpectedExceptionOnStop(Exception ex);
	}
}
