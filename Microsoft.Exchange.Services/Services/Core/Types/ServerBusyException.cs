using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class ServerBusyException : ServicePermanentException
	{
		public ServerBusyException(Exception innerException) : base((CoreResources.IDs)3655513582U, innerException)
		{
		}

		public ServerBusyException() : base((CoreResources.IDs)3655513582U)
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
