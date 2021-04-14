using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class MissingEmailAddressForDistinguishedFolderException : ServicePermanentException
	{
		public MissingEmailAddressForDistinguishedFolderException() : base(CoreResources.IDs.ErrorMissingEmailAddress)
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
