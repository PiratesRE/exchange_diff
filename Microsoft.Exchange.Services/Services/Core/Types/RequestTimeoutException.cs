using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class RequestTimeoutException : ServicePermanentException
	{
		public RequestTimeoutException() : base(ResponseCodeType.ErrorTimeoutExpired, (CoreResources.IDs)3285224352U)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}

		internal override bool StopsBatchProcessing
		{
			get
			{
				return true;
			}
		}
	}
}
