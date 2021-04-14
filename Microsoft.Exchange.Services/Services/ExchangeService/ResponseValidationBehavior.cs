using System;

namespace Microsoft.Exchange.Services.ExchangeService
{
	public enum ResponseValidationBehavior
	{
		ThrowOnAnyResponseError,
		ThrowOnSingleResponseError,
		DoNotValidate
	}
}
