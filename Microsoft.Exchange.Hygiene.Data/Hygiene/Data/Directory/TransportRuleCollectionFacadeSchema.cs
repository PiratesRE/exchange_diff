using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Transport;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class TransportRuleCollectionFacadeSchema : ADObjectSchema
	{
		public static readonly HygienePropertyDefinition FileData = new HygienePropertyDefinition("FileData", typeof(byte[]));

		public static readonly HygienePropertyDefinition MigrationSource = new HygienePropertyDefinition("MigrationSource", typeof(MigrationSourceType));
	}
}
