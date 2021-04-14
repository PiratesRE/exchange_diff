using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DistributionListSchema : ContactBaseSchema
	{
		public new static DistributionListSchema Instance
		{
			get
			{
				if (DistributionListSchema.instance == null)
				{
					DistributionListSchema.instance = new DistributionListSchema();
				}
				return DistributionListSchema.instance;
			}
		}

		protected override ICollection<PropertyRule> PropertyRules
		{
			get
			{
				return DistributionListSchema.DistributionListSchemaPropertyRules;
			}
		}

		[Autoload]
		internal static readonly StorePropertyDefinition DLStream = InternalSchema.DLStream;

		[Autoload]
		internal static readonly StorePropertyDefinition DLChecksum = InternalSchema.DLChecksum;

		[Autoload]
		public static readonly StorePropertyDefinition DLName = InternalSchema.DLName;

		[Autoload]
		public static readonly StorePropertyDefinition DLAlias = InternalSchema.DLAlias;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition Members = InternalSchema.Members;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition OneOffMembers = InternalSchema.OneOffMembers;

		[Autoload]
		public static readonly StorePropertyDefinition Email1EmailAddress = InternalSchema.Email1EmailAddress;

		[Autoload]
		public static readonly StorePropertyDefinition AsParticipant = InternalSchema.DistributionListParticipant;

		[Autoload]
		internal static readonly StorePropertyDefinition MapiSubject = InternalSchema.MapiSubject;

		private static readonly List<PropertyRule> DistributionListSchemaPropertyRules = new List<PropertyRule>
		{
			PropertyRuleLibrary.PersonIdRule,
			PropertyRuleLibrary.PDLDisplayNameRule,
			PropertyRuleLibrary.PDLMembershipRule
		};

		private static DistributionListSchema instance = null;
	}
}
