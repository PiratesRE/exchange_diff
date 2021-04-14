using System;

namespace Microsoft.Exchange.Data.Metering
{
	internal interface ITrendline
	{
		bool WasAbove(long high);

		bool WasBelow(long low);

		bool HasCrossedBelowAfterLastCrossingAbove(long high, long low);

		bool HasCrossedAboveAfterLastCrossingBelow(long low, long high);

		bool StillAboveLowAfterCrossingHigh(long high, long low);

		bool StillBelowHighAfterCrossingLow(long low, long high);

		long GetMax();

		long GetMin();

		long GetAverage();
	}
}
