using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class FolderSaveException : ServicePermanentException
	{
		public FolderSaveException() : base(CoreResources.IDs.ErrorFolderSaveFailed)
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
