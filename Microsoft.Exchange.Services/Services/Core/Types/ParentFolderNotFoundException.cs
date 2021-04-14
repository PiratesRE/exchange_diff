using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class ParentFolderNotFoundException : ServicePermanentException
	{
		public ParentFolderNotFoundException() : base((CoreResources.IDs)4217637937U)
		{
		}

		public ParentFolderNotFoundException(Exception innerException) : base((CoreResources.IDs)4217637937U, innerException)
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
