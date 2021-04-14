using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class SharedProperties
	{
		public static readonly PropertyDefinition ItemMovedByJunkMailRule = new SimpleProviderPropertyDefinition("ItemMovedByJunkMailRule", ExchangeObjectVersion.Exchange2012, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
