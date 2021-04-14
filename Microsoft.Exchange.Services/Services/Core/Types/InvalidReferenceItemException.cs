using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class InvalidReferenceItemException : ServicePermanentException
	{
		public InvalidReferenceItemException() : base((CoreResources.IDs)2519519915U)
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
