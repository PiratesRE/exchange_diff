using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SearchObjectBaseSchema : ObjectSchema
	{
		internal static readonly char[] NameReservedChars = new char[]
		{
			'*',
			'?',
			'\\',
			'/'
		};

		public static readonly ADPropertyDefinition Id = new ADPropertyDefinition("Id", ExchangeObjectVersion.Exchange2007, typeof(SearchObjectId), "distinguishedName", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExchangeVersion = new ADPropertyDefinition("ExchangeVersion", ExchangeObjectVersion.Exchange2007, typeof(ExchangeObjectVersion), "exchangeVersion", ADPropertyDefinitionFlags.DoNotProvisionalClone, ExchangeObjectVersion.Exchange2007, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ObjectState = new ADPropertyDefinition("ObjectState", ExchangeObjectVersion.Exchange2007, typeof(ObjectState), "objectState", ADPropertyDefinitionFlags.ReadOnly, Microsoft.Exchange.Data.ObjectState.New, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Name = new ADPropertyDefinition("Name", ExchangeObjectVersion.Exchange2007, typeof(string), "name", ADPropertyDefinitionFlags.Mandatory, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new NotNullOrEmptyConstraint(),
			new NoSurroundingWhiteSpaceConstraint(),
			new CharacterConstraint(SearchObjectBaseSchema.NameReservedChars, false),
			new StringLengthConstraint(1, 64)
		}, null, null);
	}
}
