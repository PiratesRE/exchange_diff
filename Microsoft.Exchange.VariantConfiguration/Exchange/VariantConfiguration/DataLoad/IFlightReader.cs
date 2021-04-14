using System;

namespace Microsoft.Exchange.VariantConfiguration.DataLoad
{
	internal interface IFlightReader
	{
		string GetFlightContent(string flightName);
	}
}
