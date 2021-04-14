using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal abstract class OrgPersonPresentationObjectSchema : ADPresentationSchema
	{
		public static readonly ADPropertyDefinition GeoCoordinates = ADRecipientSchema.GeoCoordinates;

		public static readonly ADPropertyDefinition AssistantName = ADRecipientSchema.AssistantName;

		public static readonly ADPropertyDefinition City = ADOrgPersonSchema.City;

		public static readonly ADPropertyDefinition Company = ADOrgPersonSchema.Company;

		public static readonly ADPropertyDefinition CountryOrRegion = ADOrgPersonSchema.CountryOrRegion;

		public static readonly ADPropertyDefinition Department = ADOrgPersonSchema.Department;

		public static readonly ADPropertyDefinition DirectReports = ADOrgPersonSchema.DirectReports;

		public static readonly ADPropertyDefinition DisplayName = ADRecipientSchema.DisplayName;

		public static readonly ADPropertyDefinition Fax = ADOrgPersonSchema.Fax;

		public static readonly ADPropertyDefinition FirstName = ADOrgPersonSchema.FirstName;

		public static readonly ADPropertyDefinition HomePhone = ADOrgPersonSchema.HomePhone;

		public static readonly ADPropertyDefinition Initials = ADOrgPersonSchema.Initials;

		public static readonly ADPropertyDefinition LastName = ADOrgPersonSchema.LastName;

		public static readonly ADPropertyDefinition Manager = ADOrgPersonSchema.Manager;

		public static readonly ADPropertyDefinition MobilePhone = ADOrgPersonSchema.MobilePhone;

		public static readonly ADPropertyDefinition Notes = ADRecipientSchema.Notes;

		public static readonly ADPropertyDefinition Office = ADOrgPersonSchema.Office;

		public static readonly ADPropertyDefinition OtherFax = ADOrgPersonSchema.OtherFax;

		public static readonly ADPropertyDefinition OtherHomePhone = ADOrgPersonSchema.OtherHomePhone;

		public static readonly ADPropertyDefinition OtherTelephone = ADOrgPersonSchema.OtherTelephone;

		public static readonly ADPropertyDefinition Pager = ADOrgPersonSchema.Pager;

		public static readonly ADPropertyDefinition Phone = ADOrgPersonSchema.Phone;

		public static readonly ADPropertyDefinition PhoneticDisplayName = ADRecipientSchema.PhoneticDisplayName;

		public static readonly ADPropertyDefinition PostalCode = ADOrgPersonSchema.PostalCode;

		public static readonly ADPropertyDefinition PostOfficeBox = ADOrgPersonSchema.PostOfficeBox;

		public static readonly ADPropertyDefinition RecipientType = ADRecipientSchema.RecipientType;

		public static readonly ADPropertyDefinition RecipientTypeDetails = ADRecipientSchema.RecipientTypeDetails;

		public static readonly ADPropertyDefinition SimpleDisplayName = ADRecipientSchema.SimpleDisplayName;

		public static readonly ADPropertyDefinition StateOrProvince = ADOrgPersonSchema.StateOrProvince;

		public static readonly ADPropertyDefinition StreetAddress = ADOrgPersonSchema.StreetAddress;

		public static readonly ADPropertyDefinition Title = ADOrgPersonSchema.Title;

		public static readonly ADPropertyDefinition UMRecipientDialPlanId = ADRecipientSchema.UMRecipientDialPlanId;

		public static readonly ADPropertyDefinition UMDtmfMap = ADRecipientSchema.UMDtmfMap;

		public static readonly ADPropertyDefinition AllowUMCallsFromNonUsers = ADRecipientSchema.AllowUMCallsFromNonUsers;

		public static readonly ADPropertyDefinition WebPage = ADRecipientSchema.WebPage;

		public static readonly ADPropertyDefinition TelephoneAssistant = ADOrgPersonSchema.TelephoneAssistant;

		public static readonly ADPropertyDefinition MemberOfGroup = ADRecipientSchema.MemberOfGroup;

		public static readonly ADPropertyDefinition WindowsEmailAddress = ADRecipientSchema.WindowsEmailAddress;

		public static readonly ADPropertyDefinition UMCallingLineIds = ADOrgPersonSchema.UMCallingLineIds;

		public static readonly ADPropertyDefinition SeniorityIndex = ADRecipientSchema.HABSeniorityIndex;

		public static readonly ADPropertyDefinition VoiceMailSettings = ADOrgPersonSchema.VoiceMailSettings;
	}
}
