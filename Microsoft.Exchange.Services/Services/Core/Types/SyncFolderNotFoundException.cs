using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class SyncFolderNotFoundException : ServicePermanentException
	{
		public SyncFolderNotFoundException() : base(CoreResources.IDs.ErrorSyncFolderNotFound)
		{
		}

		public SyncFolderNotFoundException(Exception innerException) : base(CoreResources.IDs.ErrorSyncFolderNotFound, innerException)
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
