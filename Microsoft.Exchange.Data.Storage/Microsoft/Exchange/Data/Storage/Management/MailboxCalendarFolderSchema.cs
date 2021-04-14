using System;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxCalendarFolderSchema : UserConfigurationObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition MailboxFolderId = new SimpleProviderPropertyDefinition("Identity", ExchangeObjectVersion.Exchange2003, typeof(MailboxFolderId), PropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition PublishEnabled = new SimpleProviderPropertyDefinition("PublishEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition PublishDateRangeFrom = new SimpleProviderPropertyDefinition("PublishDateRangeFrom", ExchangeObjectVersion.Exchange2010, typeof(DateRangeEnumType), PropertyDefinitionFlags.None, DateRangeEnumType.ThreeMonths, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition PublishDateRangeTo = new SimpleProviderPropertyDefinition("PublishDateRangeTo", ExchangeObjectVersion.Exchange2010, typeof(DateRangeEnumType), PropertyDefinitionFlags.None, DateRangeEnumType.ThreeMonths, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DetailLevel = new SimpleProviderPropertyDefinition("DetailLevel", ExchangeObjectVersion.Exchange2010, typeof(DetailLevelEnumType), PropertyDefinitionFlags.None, DetailLevelEnumType.AvailabilityOnly, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SearchableUrlEnabled = new SimpleProviderPropertyDefinition("SearchableUrlEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition PublishedCalendarUrl = new SimpleProviderPropertyDefinition("PublishedCalendarUrl", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition PublishedICalUrl = new SimpleProviderPropertyDefinition("PublishedICalUrl", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition PublishedCalendarUrlCalculated = new SimpleProviderPropertyDefinition("PublishedCalendarUrlCalculated", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MailboxCalendarFolderSchema.PublishedCalendarUrl
		}, null, (IPropertyBag propertyBag) => MailboxCalendarFolder.PublishedUrlGetter(propertyBag, MailboxCalendarFolderSchema.PublishedCalendarUrl), delegate(object value, IPropertyBag propertyBag)
		{
			MailboxCalendarFolder.PublishedUrlSetter(value, propertyBag, MailboxCalendarFolderSchema.PublishedCalendarUrl);
		});

		public static readonly SimpleProviderPropertyDefinition PublishedICalUrlCalculated = new SimpleProviderPropertyDefinition("PublishedICalUrlCalculated", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MailboxCalendarFolderSchema.PublishedICalUrl
		}, null, (IPropertyBag propertyBag) => MailboxCalendarFolder.PublishedUrlGetter(propertyBag, MailboxCalendarFolderSchema.PublishedICalUrl), delegate(object value, IPropertyBag propertyBag)
		{
			MailboxCalendarFolder.PublishedUrlSetter(value, propertyBag, MailboxCalendarFolderSchema.PublishedICalUrl);
		});
	}
}
