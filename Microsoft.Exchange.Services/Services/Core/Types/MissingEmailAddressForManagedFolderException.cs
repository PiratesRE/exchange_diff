using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class MissingEmailAddressForManagedFolderException : ServicePermanentException
	{
		public MissingEmailAddressForManagedFolderException() : base(CoreResources.IDs.ErrorMissingEmailAddressForManagedFolder)
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
