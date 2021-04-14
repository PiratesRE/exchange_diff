using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal class IncorrectUpdatePropertyCountException : ServicePermanentException
	{
		public IncorrectUpdatePropertyCountException() : base(CoreResources.IDs.ErrorIncorrectUpdatePropertyCount)
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
