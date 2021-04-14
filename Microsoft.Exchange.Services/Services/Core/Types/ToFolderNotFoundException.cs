using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class ToFolderNotFoundException : ServicePermanentException
	{
		public ToFolderNotFoundException() : base(CoreResources.IDs.ErrorToFolderNotFound)
		{
		}

		public ToFolderNotFoundException(Exception innerException) : base(CoreResources.IDs.ErrorToFolderNotFound, innerException)
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
