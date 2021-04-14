using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public interface IDisposableImpl : IDisposable
	{
		DisposeTracker InternalGetDisposeTracker();

		void InternalDispose(bool calledFromDispose);
	}
}
