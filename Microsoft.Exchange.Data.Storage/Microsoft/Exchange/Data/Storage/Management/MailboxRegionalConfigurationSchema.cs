using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxRegionalConfigurationSchema : UserConfigurationObjectSchema
	{
		public static readonly SimplePropertyDefinition RawDateFormat = new SimplePropertyDefinition("rawdateformat", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition RawTimeFormat = new SimplePropertyDefinition("rawtimeformat", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition DefaultFolderNameMatchingUserLanguage = new SimplePropertyDefinition("defaultFolderNameMatchingUserLanguage", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition TimeZone = new SimplePropertyDefinition("timezone", ExchangeObjectVersion.Exchange2010, typeof(ExTimeZoneValue), PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition Language = new SimplePropertyDefinition("language", ExchangeObjectVersion.Exchange2010, typeof(CultureInfo), PropertyDefinitionFlags.None, null, null, new PropertyDefinitionConstraint[]
		{
			new DelegateConstraint(new ValidationDelegate(ConstraintDelegates.ValidateNonNeutralCulture))
		}, new PropertyDefinitionConstraint[]
		{
			new DelegateConstraint(new ValidationDelegate(ConstraintDelegates.ValidateNonNeutralCulture))
		});

		public static readonly SimplePropertyDefinition DateFormat = new SimplePropertyDefinition("dateformat", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.Calculated, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new SimplePropertyDefinition[]
		{
			MailboxRegionalConfigurationSchema.RawDateFormat,
			MailboxRegionalConfigurationSchema.Language
		}, null, new GetterDelegate(MailboxRegionalConfiguration.DateFormatGetter), new SetterDelegate(MailboxRegionalConfiguration.DateFormatSetter));

		public static readonly SimplePropertyDefinition TimeFormat = new SimplePropertyDefinition("timeformat", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.Calculated, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new SimplePropertyDefinition[]
		{
			MailboxRegionalConfigurationSchema.RawTimeFormat,
			MailboxRegionalConfigurationSchema.Language
		}, null, new GetterDelegate(MailboxRegionalConfiguration.TimeFormatGetter), new SetterDelegate(MailboxRegionalConfiguration.TimeFormatSetter));
	}
}
