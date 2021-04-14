using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal abstract class IOriginatingTimestampSchema
	{
		public static readonly ADPropertyDefinition LastExchangeChangedTime = new ADPropertyDefinition("LastExchangeChangedTime", ExchangeObjectVersion.Exchange2003, typeof(DateTime?), "msExchLastExchangeChangedTime", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.LastExchangeChangedTime);
	}
}
