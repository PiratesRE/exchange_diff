using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class MissingInformationReferenceItemIdException : ServicePermanentException
	{
		public MissingInformationReferenceItemIdException() : base(CoreResources.IDs.ErrorMissingInformationReferenceItemId)
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
