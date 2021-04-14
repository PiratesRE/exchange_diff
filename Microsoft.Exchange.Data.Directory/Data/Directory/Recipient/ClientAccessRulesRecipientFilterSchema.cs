using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class ClientAccessRulesRecipientFilterSchema : ObjectSchema
	{
		public static readonly ADPropertyDefinition Sid = ADMailboxRecipientSchema.Sid;

		public static readonly ADPropertyDefinition MasterAccountSid = ADRecipientSchema.MasterAccountSid;

		public static readonly ADPropertyDefinition WindowsLiveID = ADRecipientSchema.WindowsLiveID;

		public static readonly ADPropertyDefinition City = ADUserSchema.City;

		public static readonly ADPropertyDefinition Company = ADUserSchema.Company;

		internal static readonly ADPropertyDefinition C = ADUserSchema.C;

		public static readonly ADPropertyDefinition Co = ADUserSchema.Co;

		internal static readonly ADPropertyDefinition CountryCode = ADUserSchema.CountryCode;

		public static readonly ADPropertyDefinition CountryOrRegion = ADUserSchema.CountryOrRegion;

		public static readonly ADPropertyDefinition Department = ADUserSchema.Department;

		public static readonly ADPropertyDefinition Office = ADUserSchema.Office;

		public static readonly ADPropertyDefinition PostalCode = ADUserSchema.PostalCode;

		public static readonly ADPropertyDefinition StateOrProvince = ADUserSchema.StateOrProvince;

		public static readonly ADPropertyDefinition StreetAddress = ADUserSchema.StreetAddress;

		public static readonly ADPropertyDefinition CustomAttribute1 = ADRecipientSchema.CustomAttribute1;

		public static readonly ADPropertyDefinition CustomAttribute10 = ADRecipientSchema.CustomAttribute10;

		public static readonly ADPropertyDefinition CustomAttribute11 = ADRecipientSchema.CustomAttribute11;

		public static readonly ADPropertyDefinition CustomAttribute12 = ADRecipientSchema.CustomAttribute12;

		public static readonly ADPropertyDefinition CustomAttribute13 = ADRecipientSchema.CustomAttribute13;

		public static readonly ADPropertyDefinition CustomAttribute14 = ADRecipientSchema.CustomAttribute14;

		public static readonly ADPropertyDefinition CustomAttribute15 = ADRecipientSchema.CustomAttribute15;

		public static readonly ADPropertyDefinition CustomAttribute2 = ADRecipientSchema.CustomAttribute2;

		public static readonly ADPropertyDefinition CustomAttribute3 = ADRecipientSchema.CustomAttribute3;

		public static readonly ADPropertyDefinition CustomAttribute4 = ADRecipientSchema.CustomAttribute4;

		public static readonly ADPropertyDefinition CustomAttribute5 = ADRecipientSchema.CustomAttribute5;

		public static readonly ADPropertyDefinition CustomAttribute6 = ADRecipientSchema.CustomAttribute6;

		public static readonly ADPropertyDefinition CustomAttribute7 = ADRecipientSchema.CustomAttribute7;

		public static readonly ADPropertyDefinition CustomAttribute8 = ADRecipientSchema.CustomAttribute8;

		public static readonly ADPropertyDefinition CustomAttribute9 = ADRecipientSchema.CustomAttribute9;

		public static readonly ADPropertyDefinition ExtensionCustomAttribute1 = ADRecipientSchema.ExtensionCustomAttribute1;

		public static readonly ADPropertyDefinition ExtensionCustomAttribute2 = ADRecipientSchema.ExtensionCustomAttribute2;

		public static readonly ADPropertyDefinition ExtensionCustomAttribute3 = ADRecipientSchema.ExtensionCustomAttribute3;

		public static readonly ADPropertyDefinition ExtensionCustomAttribute4 = ADRecipientSchema.ExtensionCustomAttribute4;

		public static readonly ADPropertyDefinition ExtensionCustomAttribute5 = ADRecipientSchema.ExtensionCustomAttribute5;
	}
}
