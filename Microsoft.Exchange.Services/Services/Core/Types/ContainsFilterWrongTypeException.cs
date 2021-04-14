using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class ContainsFilterWrongTypeException : ServicePermanentException
	{
		public ContainsFilterWrongTypeException() : base((CoreResources.IDs)3836413508U)
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
