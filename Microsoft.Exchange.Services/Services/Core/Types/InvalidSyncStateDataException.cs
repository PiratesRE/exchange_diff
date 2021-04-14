using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidSyncStateDataException : ServicePermanentException
	{
		public InvalidSyncStateDataException() : base(CoreResources.IDs.ErrorInvalidSyncStateData)
		{
		}

		public InvalidSyncStateDataException(Exception innerException) : base(CoreResources.IDs.ErrorInvalidSyncStateData, innerException)
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
