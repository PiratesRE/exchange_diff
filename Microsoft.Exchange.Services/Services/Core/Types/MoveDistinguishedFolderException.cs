using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class MoveDistinguishedFolderException : ServicePermanentException
	{
		public MoveDistinguishedFolderException() : base((CoreResources.IDs)3771523283U)
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
