using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidGetSharingFolderRequestException : ServicePermanentException
	{
		public InvalidGetSharingFolderRequestException() : base(CoreResources.IDs.ErrorInvalidGetSharingFolderRequest)
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
