using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class ReadReceiptNotPendingException : ServicePermanentException
	{
		public ReadReceiptNotPendingException() : base((CoreResources.IDs)2875907804U)
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
