using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class InvalidPropertyUpdateSentMessageException : ServicePermanentExceptionWithPropertyPath
	{
		public InvalidPropertyUpdateSentMessageException(PropertyPath propertyPath) : base(CoreResources.IDs.ErrorInvalidPropertyUpdateSentMessage, propertyPath)
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
