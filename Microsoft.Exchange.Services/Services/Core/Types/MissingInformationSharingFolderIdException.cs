using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class MissingInformationSharingFolderIdException : ServicePermanentException
	{
		public MissingInformationSharingFolderIdException() : base((CoreResources.IDs)2938284467U)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2010;
			}
		}
	}
}
