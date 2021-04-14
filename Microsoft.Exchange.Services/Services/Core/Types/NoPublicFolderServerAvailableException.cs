using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class NoPublicFolderServerAvailableException : ServicePermanentException
	{
		public NoPublicFolderServerAvailableException() : base(ResponseCodeType.ErrorNoPublicFolderReplicaAvailable, (CoreResources.IDs)2356362688U)
		{
		}

		public NoPublicFolderServerAvailableException(Exception innerException) : base(ResponseCodeType.ErrorNoPublicFolderReplicaAvailable, (CoreResources.IDs)2356362688U, innerException)
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
