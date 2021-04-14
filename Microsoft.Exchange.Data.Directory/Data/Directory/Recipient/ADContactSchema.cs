using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class ADContactSchema : ADRecipientSchema
	{
		public static readonly ADPropertyDefinition DeliverToMailboxAndForward = IADMailStorageSchema.DeliverToMailboxAndForward;

		public static readonly ADPropertyDefinition CatchAllRecipientBL = ADUserSchema.CatchAllRecipientBL;

		public static readonly ADPropertyDefinition Company = ADOrgPersonSchema.Company;

		public static readonly ADPropertyDefinition Co = ADOrgPersonSchema.Co;

		internal static readonly ADPropertyDefinition C = ADOrgPersonSchema.C;

		internal static readonly ADPropertyDefinition CountryCode = ADOrgPersonSchema.CountryCode;

		public static readonly ADPropertyDefinition Department = ADOrgPersonSchema.Department;

		public static readonly ADPropertyDefinition DirectReports = ADOrgPersonSchema.DirectReports;

		public static readonly ADPropertyDefinition Fax = ADOrgPersonSchema.Fax;

		public static readonly ADPropertyDefinition FirstName = ADOrgPersonSchema.FirstName;

		public static readonly ADPropertyDefinition HomePhone = ADOrgPersonSchema.HomePhone;

		public static readonly ADPropertyDefinition Initials = ADOrgPersonSchema.Initials;

		public static readonly ADPropertyDefinition LanguagesRaw = ADOrgPersonSchema.LanguagesRaw;

		public static readonly ADPropertyDefinition LastName = ADOrgPersonSchema.LastName;

		public static readonly ADPropertyDefinition City = ADOrgPersonSchema.City;

		public static readonly ADPropertyDefinition Manager = ADOrgPersonSchema.Manager;

		public static readonly ADPropertyDefinition MobilePhone = ADOrgPersonSchema.MobilePhone;

		public static readonly ADPropertyDefinition Office = ADOrgPersonSchema.Office;

		public static readonly ADPropertyDefinition OtherFax = ADOrgPersonSchema.OtherFax;

		public static readonly ADPropertyDefinition OtherHomePhone = ADOrgPersonSchema.OtherHomePhone;

		public static readonly ADPropertyDefinition OtherTelephone = ADOrgPersonSchema.OtherTelephone;

		public static readonly ADPropertyDefinition OtherMobile = ADOrgPersonSchema.OtherMobile;

		public static readonly ADPropertyDefinition Pager = ADOrgPersonSchema.Pager;

		public static readonly ADPropertyDefinition Phone = ADOrgPersonSchema.Phone;

		public static readonly ADPropertyDefinition PostalCode = ADOrgPersonSchema.PostalCode;

		public static readonly ADPropertyDefinition PostOfficeBox = ADOrgPersonSchema.PostOfficeBox;

		public static readonly ADPropertyDefinition StateOrProvince = ADOrgPersonSchema.StateOrProvince;

		public static readonly ADPropertyDefinition StreetAddress = ADOrgPersonSchema.StreetAddress;

		public static readonly ADPropertyDefinition TelephoneAssistant = ADOrgPersonSchema.TelephoneAssistant;

		public static readonly ADPropertyDefinition Title = ADOrgPersonSchema.Title;

		public static readonly ADPropertyDefinition ViewDepth = ADOrgPersonSchema.ViewDepth;

		public static readonly ADPropertyDefinition RtcSipLine = ADOrgPersonSchema.RtcSipLine;

		public static readonly ADPropertyDefinition UMCallingLineIds = ADOrgPersonSchema.UMCallingLineIds;

		public static readonly ADPropertyDefinition VoiceMailSettings = ADOrgPersonSchema.VoiceMailSettings;

		public static readonly ADPropertyDefinition CountryOrRegion = ADOrgPersonSchema.CountryOrRegion;

		public static readonly ADPropertyDefinition Languages = ADOrgPersonSchema.Languages;

		public static readonly ADPropertyDefinition SanitizedPhoneNumbers = ADOrgPersonSchema.SanitizedPhoneNumbers;
	}
}
