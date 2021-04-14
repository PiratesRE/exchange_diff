using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal abstract class SyncOrgPersonSchema : SyncRecipientSchema
	{
		public static SyncPropertyDefinition AssistantName = new SyncPropertyDefinition(ADRecipientSchema.AssistantName, "MSExchAssistantName", typeof(DirectoryPropertyStringSingleLength1To256), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition C = new SyncPropertyDefinition(ADOrgPersonSchema.C, "C", typeof(DirectoryPropertyStringSingleLength1To3), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition City = new SyncPropertyDefinition(ADOrgPersonSchema.City, "L", typeof(DirectoryPropertyStringSingleLength1To128), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition Co = new SyncPropertyDefinition(ADOrgPersonSchema.Co, "Co", typeof(DirectoryPropertyStringSingleLength1To128), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition Company = new SyncPropertyDefinition(ADOrgPersonSchema.Company, "Company", typeof(DirectoryPropertyStringSingleLength1To64), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition CountryCode = new SyncPropertyDefinition(ADOrgPersonSchema.CountryCode, "CountryCode", typeof(DirectoryPropertyInt32SingleMin0Max65535), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition Department = new SyncPropertyDefinition(ADOrgPersonSchema.Department, "Department", typeof(DirectoryPropertyStringSingleLength1To64), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition Fax = new SyncPropertyDefinition(ADOrgPersonSchema.Fax, "FacsimileTelephoneNumber", typeof(DirectoryPropertyStringSingleLength1To64), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition FirstName = new SyncPropertyDefinition(ADOrgPersonSchema.FirstName, "GivenName", typeof(DirectoryPropertyStringSingleLength1To64), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition HomePhone = new SyncPropertyDefinition(ADOrgPersonSchema.HomePhone, "HomePhone", typeof(DirectoryPropertyStringSingleLength1To64), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition Initials = new SyncPropertyDefinition(ADOrgPersonSchema.Initials, "Initials", typeof(DirectoryPropertyStringSingleLength1To6), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition LastName = new SyncPropertyDefinition(ADOrgPersonSchema.LastName, "Sn", typeof(DirectoryPropertyStringSingleLength1To64), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition MobilePhone = new SyncPropertyDefinition(ADOrgPersonSchema.MobilePhone, "Mobile", typeof(DirectoryPropertyStringSingleLength1To64), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition Notes = new SyncPropertyDefinition(ADRecipientSchema.Notes, "Info", typeof(DirectoryPropertyStringSingleLength1To1024), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition Office = new SyncPropertyDefinition(ADOrgPersonSchema.Office, "PhysicalDeliveryOfficeName", typeof(DirectoryPropertyStringSingleLength1To128), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition OtherFax = new SyncPropertyDefinition(ADOrgPersonSchema.OtherFax, "OtherFacsimileTelephoneNumber", typeof(DirectoryPropertyStringLength1To64), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition OtherHomePhone = new SyncPropertyDefinition(ADOrgPersonSchema.OtherHomePhone, "OtherHomePhone", typeof(DirectoryPropertyStringLength1To64), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition OtherTelephone = new SyncPropertyDefinition(ADOrgPersonSchema.OtherTelephone, "OtherTelephone", typeof(DirectoryPropertyStringLength1To64), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.BackSync | SyncPropertyDefinitionFlags.MultiValued, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition Pager = new SyncPropertyDefinition(ADOrgPersonSchema.Pager, "Pager", typeof(DirectoryPropertyStringSingleLength1To64), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition Phone = new SyncPropertyDefinition(ADOrgPersonSchema.Phone, "TelephoneNumber", typeof(DirectoryPropertyStringSingleLength1To64), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition PostalCode = new SyncPropertyDefinition(ADOrgPersonSchema.PostalCode, "PostalCode", typeof(DirectoryPropertyStringSingleLength1To40), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition StateOrProvince = new SyncPropertyDefinition(ADOrgPersonSchema.StateOrProvince, "St", typeof(DirectoryPropertyStringSingleLength1To128), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition StreetAddress = new SyncPropertyDefinition(ADOrgPersonSchema.StreetAddress, "StreetAddress", typeof(DirectoryPropertyStringSingleLength1To1024), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition TelephoneAssistant = new SyncPropertyDefinition(ADOrgPersonSchema.TelephoneAssistant, "TelephoneAssistant", typeof(DirectoryPropertyStringSingleLength1To64), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition Title = new SyncPropertyDefinition(ADOrgPersonSchema.Title, "Title", typeof(DirectoryPropertyStringSingleLength1To128), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition WebPage = new SyncPropertyDefinition(ADRecipientSchema.WebPage, "WwwHomepage", typeof(DirectoryPropertyStringSingleLength1To2048), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);
	}
}
