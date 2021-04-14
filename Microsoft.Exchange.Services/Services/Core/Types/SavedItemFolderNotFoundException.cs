using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class SavedItemFolderNotFoundException : ServicePermanentException
	{
		public SavedItemFolderNotFoundException() : base((CoreResources.IDs)3610830273U)
		{
		}

		public SavedItemFolderNotFoundException(Exception innerException) : base((CoreResources.IDs)3610830273U, innerException)
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
