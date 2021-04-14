using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AggregatedContactSchema : Schema
	{
		public new static AggregatedContactSchema Instance
		{
			get
			{
				return AggregatedContactSchema.instance;
			}
		}

		public static readonly PropertyDefinition CompanyName = new ReadonlySmartProperty(InternalSchema.InternalPersonCompanyName);

		public static readonly PropertyDefinition DisplayName = new ReadonlySmartProperty(InternalSchema.InternalPersonDisplayName);

		public static readonly PropertyDefinition GivenName = new ReadonlySmartProperty(InternalSchema.InternalPersonGivenName);

		public static readonly PropertyDefinition Surname = new ReadonlySmartProperty(InternalSchema.InternalPersonSurname);

		public static readonly PropertyDefinition FileAs = new FileAsStringProperty(InternalSchema.InternalPersonFileAs);

		public static readonly PropertyDefinition HomeCity = new ReadonlySmartProperty(InternalSchema.InternalPersonHomeCity);

		public static readonly PropertyDefinition CreationTime = new ReadonlySmartProperty(InternalSchema.InternalPersonCreationTime);

		public static readonly PropertyDefinition WorkCity = new ReadonlySmartProperty(InternalSchema.InternalPersonWorkCity);

		public static readonly PropertyDefinition DisplayNameFirstLast = new ReadonlySmartProperty(InternalSchema.InternalPersonDisplayNameFirstLast);

		public static readonly PropertyDefinition DisplayNameLastFirst = new ReadonlySmartProperty(InternalSchema.InternalPersonDisplayNameLastFirst);

		public static readonly PropertyDefinition RelevanceScore = new ReadonlySmartProperty(InternalSchema.InternalPersonRelevanceScore);

		public static readonly PropertyDefinition MessageClass = new ReadonlySmartProperty(InternalSchema.InternalConversationMessageClasses);

		private static readonly AggregatedContactSchema instance = new AggregatedContactSchema();
	}
}
