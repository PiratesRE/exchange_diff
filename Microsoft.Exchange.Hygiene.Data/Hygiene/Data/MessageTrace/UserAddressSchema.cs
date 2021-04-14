using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class UserAddressSchema
	{
		internal static readonly HygienePropertyDefinition UserAddressIdProperty = new HygienePropertyDefinition("UserAddressId", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition EmailDomainProperty = new HygienePropertyDefinition("EmailDomain", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition EmailPrefixProperty = new HygienePropertyDefinition("EmailPrefix", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition DigestFrequencyProperty = new HygienePropertyDefinition("DigestFreq", typeof(int?));

		internal static readonly HygienePropertyDefinition LastNotifiedProperty = new HygienePropertyDefinition("LastNotifiedDateTime", typeof(DateTime?));

		internal static readonly HygienePropertyDefinition BlockEsnProperty = new HygienePropertyDefinition("BlockEsn", typeof(bool?));
	}
}
