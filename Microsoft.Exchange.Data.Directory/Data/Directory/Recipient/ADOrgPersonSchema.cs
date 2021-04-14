using System;
using System.Globalization;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class ADOrgPersonSchema
	{
		public static readonly ADPropertyDefinition Company = new ADPropertyDefinition("Company", ExchangeObjectVersion.Exchange2003, typeof(string), "company", "msExchShadowCompany", ADPropertyDefinitionFlags.None, string.Empty, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 256)
		}, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition Co = new ADPropertyDefinition("Co", ExchangeObjectVersion.Exchange2003, typeof(string), "co", "msExchShadowCo", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		internal static readonly ADPropertyDefinition C = new ADPropertyDefinition("C", ExchangeObjectVersion.Exchange2003, typeof(string), "c", "msExchShadowC", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.C);

		internal static readonly ADPropertyDefinition CountryCode = new ADPropertyDefinition("CountryCode", ExchangeObjectVersion.Exchange2003, typeof(int), "CountryCode", "msExchShadowCountryCode", ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 32767)
		}, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.CountryCode);

		public static readonly ADPropertyDefinition Department = new ADPropertyDefinition("Department", ExchangeObjectVersion.Exchange2003, typeof(string), "department", "msExchShadowDepartment", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 128)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition DirectReports = new ADPropertyDefinition("DirectReports", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "directReports", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition Fax = new ADPropertyDefinition("Fax", ExchangeObjectVersion.Exchange2003, typeof(string), "facsimileTelephoneNumber", "msExchShadowFacsimileTelephoneNumber", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 64)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition FirstName = new ADPropertyDefinition("FirstName", ExchangeObjectVersion.Exchange2003, typeof(string), "givenName", "msExchShadowGivenName", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 64)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.FirstName);

		public static readonly ADPropertyDefinition HomePhone = new ADPropertyDefinition("HomePhone", ExchangeObjectVersion.Exchange2003, typeof(string), "homePhone", "msExchShadowHomePhone", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 64)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.HomePhone);

		public static readonly ADPropertyDefinition Initials = new ADPropertyDefinition("Initials", ExchangeObjectVersion.Exchange2003, typeof(string), "initials", "msExchShadowInitials", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 6)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.Initials);

		public static readonly ADPropertyDefinition LanguagesRaw = new ADPropertyDefinition("LanguagesRaw", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUserCulture", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition LastName = new ADPropertyDefinition("LastName", ExchangeObjectVersion.Exchange2003, typeof(string), "sn", "msExchShadowSn", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 64)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.LastName);

		public static readonly ADPropertyDefinition City = new ADPropertyDefinition("City", ExchangeObjectVersion.Exchange2003, typeof(string), "l", "msExchShadowL", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 128)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.City);

		public static readonly ADPropertyDefinition Manager = new ADPropertyDefinition("Manager", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "manager", "msExchShadowManagerLink", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition MobilePhone = new ADPropertyDefinition("MobilePhone", ExchangeObjectVersion.Exchange2003, typeof(string), "mobile", "msExchShadowMobile", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 64)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.MobilePhone);

		public static readonly ADPropertyDefinition Office = new ADPropertyDefinition("Office", ExchangeObjectVersion.Exchange2003, typeof(string), "physicalDeliveryOfficeName", "msExchShadowPhysicalDeliveryOfficeName", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 128)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition OtherFax = new ADPropertyDefinition("OtherFax", ExchangeObjectVersion.Exchange2003, typeof(string), "otherFacsimileTelephoneNumber", "msExchShadowOtherFacsimileTelephone", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition OtherHomePhone = new ADPropertyDefinition("OtherHomePhone", ExchangeObjectVersion.Exchange2003, typeof(string), "otherHomePhone", "msExchShadowOtherHomePhone", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.OtherHomePhone);

		public static readonly ADPropertyDefinition OtherTelephone = new ADPropertyDefinition("OtherTelephone", ExchangeObjectVersion.Exchange2003, typeof(string), "otherTelephone", "msExchShadowOtherTelephone", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.OtherTelephone);

		public static readonly ADPropertyDefinition OtherMobile = new ADPropertyDefinition("OtherMobile", ExchangeObjectVersion.Exchange2003, typeof(string), "otherMobile", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.OtherMobile);

		public static readonly ADPropertyDefinition Pager = new ADPropertyDefinition("Pager", ExchangeObjectVersion.Exchange2003, typeof(string), "pager", "msExchShadowPager", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 64)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.Pager);

		public static readonly ADPropertyDefinition Phone = new ADPropertyDefinition("Phone", ExchangeObjectVersion.Exchange2003, typeof(string), "telephoneNumber", "msExchShadowTelephoneNumber", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 64)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.Phone);

		public static readonly ADPropertyDefinition PostalCode = new ADPropertyDefinition("PostalCode", ExchangeObjectVersion.Exchange2003, typeof(string), "postalCode", "msExchShadowPostalCode", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 40)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.PostalCode);

		public static readonly ADPropertyDefinition PostOfficeBox = new ADPropertyDefinition("PostOfficeBox", ExchangeObjectVersion.Exchange2003, typeof(string), "postOfficeBox", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.PostOfficeBox);

		public static readonly ADPropertyDefinition StateOrProvince = new ADPropertyDefinition("StateOrProvince", ExchangeObjectVersion.Exchange2003, typeof(string), "st", "msExchShadowSt", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 128)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.StateOrProvince);

		public static readonly ADPropertyDefinition StreetAddress = new ADPropertyDefinition("StreetAddress", ExchangeObjectVersion.Exchange2003, typeof(string), "streetAddress", "msExchShadowStreetAddress", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 1024)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.StreetAddress);

		public static readonly ADPropertyDefinition TelephoneAssistant = new ADPropertyDefinition("TelephoneAssistant", ExchangeObjectVersion.Exchange2003, typeof(string), "telephoneAssistant", "msExchShadowTelephoneAssistant", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition Title = new ADPropertyDefinition("Title", ExchangeObjectVersion.Exchange2003, typeof(string), "title", "msExchShadowTitle", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 128)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition ViewDepth = new ADPropertyDefinition("ViewDepth", ExchangeObjectVersion.Exchange2003, typeof(int?), null, ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RtcSipLine = new ADPropertyDefinition("RtcSipLine", ExchangeObjectVersion.Exchange2003, typeof(string), "msRTCSIP-Line", ADPropertyDefinitionFlags.ReadOnly, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition UMCallingLineIds = new ADPropertyDefinition("UMCallingLineIds", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchUMCallingLineIds", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RegexConstraint("^\\+?\\d+$", DataStrings.CLIDPatternDescription)
		}, null, null);

		public static readonly ADPropertyDefinition VoiceMailSettings = new ADPropertyDefinition("VoiceMailSettings", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchUCVoiceMailSettings", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition CountryOrRegion = new ADPropertyDefinition("CountryOrRegion", ExchangeObjectVersion.Exchange2003, typeof(CountryInfo), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOrgPersonSchema.C,
			ADOrgPersonSchema.Co,
			ADOrgPersonSchema.CountryCode
		}, new CustomFilterBuilderDelegate(ADOrgPerson.CountryOrRegionFilterBuilder), new GetterDelegate(ADOrgPerson.CountryOrRegionGetter), new SetterDelegate(ADOrgPerson.CountryOrRegionSetter), null, MbxRecipientSchema.CountryOrRegion);

		public static readonly ADPropertyDefinition Languages = new ADPropertyDefinition("Languages", ExchangeObjectVersion.Exchange2007, typeof(CultureInfo), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new DelegateConstraint(new ValidationDelegate(ConstraintDelegates.ValidateNonNeutralCulture))
		}, new ProviderPropertyDefinition[]
		{
			ADOrgPersonSchema.LanguagesRaw
		}, null, new GetterDelegate(ADOrgPerson.LanguagesGetter), new SetterDelegate(ADOrgPerson.LanguagesSetter), null, MbxRecipientSchema.Languages);

		public static readonly ADPropertyDefinition SanitizedPhoneNumbers = new ADPropertyDefinition("SanitizedPhoneNumbers", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADOrgPersonSchema.Phone,
			ADOrgPersonSchema.HomePhone,
			ADOrgPersonSchema.MobilePhone,
			ADOrgPersonSchema.Fax,
			ADOrgPersonSchema.OtherTelephone,
			ADOrgPersonSchema.OtherHomePhone,
			ADOrgPersonSchema.OtherFax,
			ADOrgPersonSchema.OtherMobile
		}, null, new GetterDelegate(ADOrgPerson.SanitizedPhoneNumbersGetter), null, null, null);
	}
}
