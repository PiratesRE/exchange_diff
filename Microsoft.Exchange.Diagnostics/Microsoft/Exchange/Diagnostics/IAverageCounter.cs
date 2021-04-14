using System;

namespace Microsoft.Exchange.Diagnostics
{
	public interface IAverageCounter
	{
		void AddSample(long sample);
	}
}
