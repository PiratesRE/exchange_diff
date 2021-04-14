using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	public interface ITracer
	{
		void TraceError(string format, params object[] args);

		void TraceWarning(string format, params object[] args);

		void TraceInformation(string format, params object[] args);
	}
}
