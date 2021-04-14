using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class UnableToRemoveImContactFromGroupException : ServicePermanentException
	{
		public UnableToRemoveImContactFromGroupException() : base((CoreResources.IDs)3162641137U)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2012;
			}
		}
	}
}
