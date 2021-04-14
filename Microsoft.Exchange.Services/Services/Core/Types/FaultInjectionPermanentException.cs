using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class FaultInjectionPermanentException : ServicePermanentException
	{
		public FaultInjectionPermanentException(ResponseCodeType responseCode, string soapAction) : base(responseCode, CoreResources.ErrorInternalServerErrorFaultInjection(responseCode.ToString(), soapAction))
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}
	}
}
