using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class InvalidDelegateUserIdException : ServicePermanentException
	{
		public InvalidDelegateUserIdException() : base(ResponseCodeType.ErrorInvalidDelegateUserId, CoreResources.ErrorInvalidDelegateUserId(string.Empty))
		{
		}

		public InvalidDelegateUserIdException(string message) : base(ResponseCodeType.ErrorInvalidDelegateUserId, CoreResources.ErrorInvalidDelegateUserId(message))
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
