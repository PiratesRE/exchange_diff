using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class SentTaskRequestUpdateException : ServicePermanentException
	{
		public SentTaskRequestUpdateException() : base(CoreResources.IDs.ErrorSentTaskRequestUpdate)
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
