using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.ABProviderFramework
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ABObjectSchema : ObjectSchema
	{
		public static readonly ABPropertyDefinition Id = new ABPropertyDefinition("Id", typeof(ABObjectId), PropertyDefinitionFlags.ReadOnly, null);

		public static readonly ABPropertyDefinition CanEmail = new ABPropertyDefinition("CanEmail", typeof(bool), PropertyDefinitionFlags.ReadOnly, false);

		public static readonly ABPropertyDefinition LegacyExchangeDN = new ABPropertyDefinition("LegacyExchangeDN", typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty);

		public static readonly ABPropertyDefinition DisplayName = new ABPropertyDefinition("DisplayName", typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty);

		public static readonly ABPropertyDefinition Alias = new ABPropertyDefinition("Alias", typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty);

		public static readonly ABPropertyDefinition EmailAddress = new ABPropertyDefinition("EmailAddress", typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty);
	}
}
