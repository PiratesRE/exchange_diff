using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class DeleteDistinguishedFolderException : ServicePermanentException
	{
		public DeleteDistinguishedFolderException(Exception innerException) : base((CoreResources.IDs)3448951775U, innerException)
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
