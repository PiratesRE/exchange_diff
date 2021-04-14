using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class MessageDispositionRequiredException : ServicePermanentException
	{
		public MessageDispositionRequiredException() : base(CoreResources.IDs.ErrorMessageDispositionRequired)
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
