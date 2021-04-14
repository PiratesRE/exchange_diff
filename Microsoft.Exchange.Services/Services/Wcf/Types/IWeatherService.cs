using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	internal interface IWeatherService
	{
		string Get(Uri weatherServiceUri);

		void VerifyServiceAvailability(CallContext callContext);

		string PartnerId { get; }

		string BaseUrl { get; }
	}
}
