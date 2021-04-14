using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class DuplicateSOAPHeaderException : ServicePermanentException
	{
		public DuplicateSOAPHeaderException() : base((CoreResources.IDs)4197444273U)
		{
		}

		public DuplicateSOAPHeaderException(Exception innerException) : base((CoreResources.IDs)4197444273U, innerException)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2010;
			}
		}
	}
}
