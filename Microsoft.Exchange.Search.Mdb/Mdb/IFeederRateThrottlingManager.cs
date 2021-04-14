using System;

namespace Microsoft.Exchange.Search.Mdb
{
	internal interface IFeederRateThrottlingManager
	{
		double ThrottlingRateContinue(double currentRate);

		double ThrottlingRateStart();
	}
}
