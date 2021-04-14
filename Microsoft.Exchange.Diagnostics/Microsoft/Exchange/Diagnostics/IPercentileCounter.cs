using System;

namespace Microsoft.Exchange.Diagnostics
{
	public interface IPercentileCounter
	{
		void AddValue(long value);

		long PercentileQuery(double percentage);

		long PercentileQuery(double percentage, out long samples);
	}
}
