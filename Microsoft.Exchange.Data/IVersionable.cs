using System;

namespace Microsoft.Exchange.Data
{
	internal interface IVersionable
	{
		ObjectSchema ObjectSchema { get; }

		ExchangeObjectVersion ExchangeVersion { get; }

		ExchangeObjectVersion MaximumSupportedExchangeObjectVersion { get; }

		bool IsReadOnly { get; }

		bool ExchangeVersionUpgradeSupported { get; }

		bool IsPropertyAccessible(PropertyDefinition propertyDefinition);
	}
}
