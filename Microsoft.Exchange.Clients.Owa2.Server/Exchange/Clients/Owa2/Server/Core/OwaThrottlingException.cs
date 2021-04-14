using System;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	internal sealed class OwaThrottlingException : ServicePermanentException
	{
		public OwaThrottlingException() : base(ResponseCodeType.ErrorServerBusy, (CoreResources.IDs)3655513582U)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2013;
			}
		}
	}
}
