using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class DuplicateLegacyDistinguishedNameException : ServicePermanentException
	{
		public DuplicateLegacyDistinguishedNameException(string duplicateLegacyDN, Exception innerException) : base((CoreResources.IDs)3584287689U, CoreResources.ErrorDuplicateLegacyDistinguishedNameFound(duplicateLegacyDN), innerException)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2012;
			}
		}
	}
}
