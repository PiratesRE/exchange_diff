using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class CannotCreatePostItemInNonMailFolderException : ServicePermanentException
	{
		public CannotCreatePostItemInNonMailFolderException() : base((CoreResources.IDs)3792171687U)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007SP1;
			}
		}
	}
}
