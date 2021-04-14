using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	internal class RulePresentationObjectBaseSchema : ObjectSchema
	{
		public static readonly ADPropertyDefinition Name = ADObjectSchema.Name;

		public static readonly ADPropertyDefinition Guid = ADObjectSchema.Guid;

		public static readonly ADPropertyDefinition DistinguishedName = ADObjectSchema.DistinguishedName;

		public static readonly ADPropertyDefinition ImmutableId = TransportRuleSchema.ImmutableId;
	}
}
