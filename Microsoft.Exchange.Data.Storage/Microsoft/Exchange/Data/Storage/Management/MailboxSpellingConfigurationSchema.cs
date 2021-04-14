using System;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxSpellingConfigurationSchema : UserConfigurationObjectSchema
	{
		public static readonly SimplePropertyDefinition CheckBeforeSend = new SimplePropertyDefinition("spellingcheckbeforesend", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, false, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition DictionaryLanguage = new SimplePropertyDefinition("spellingdictionarylanguage", ExchangeObjectVersion.Exchange2007, typeof(SpellcheckerSupportedLanguage), PropertyDefinitionFlags.None, SpellcheckerSupportedLanguage.EnglishUnitedStates, SpellcheckerSupportedLanguage.EnglishUnitedStates, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition IgnoreUppercase = new SimplePropertyDefinition("spellingignoreuppercase", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, false, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition IgnoreMixedDigits = new SimplePropertyDefinition("spellingignoremixeddigits", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
