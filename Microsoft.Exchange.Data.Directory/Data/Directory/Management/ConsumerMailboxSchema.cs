using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class ConsumerMailboxSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADUserSchema>();
		}

		public static readonly ADPropertyDefinition Alias = ADRecipientSchema.Alias;

		public static readonly ADPropertyDefinition Database = ADMailboxRecipientSchema.Database;

		public static readonly ADPropertyDefinition Description = ADRecipientSchema.Description;

		public static readonly ADPropertyDefinition DisplayName = ADRecipientSchema.DisplayName;

		public static readonly ADPropertyDefinition EmailAddresses = ADRecipientSchema.EmailAddresses;

		public static readonly ADPropertyDefinition ExchangeGuid = ADMailboxRecipientSchema.ExchangeGuid;

		public static readonly ADPropertyDefinition LegacyExchangeDN = ADRecipientSchema.LegacyExchangeDN;

		public static readonly ADPropertyDefinition NetID = ADUserSchema.NetID;

		public static readonly ADPropertyDefinition PrimarySmtpAddress = ADRecipientSchema.PrimarySmtpAddress;

		public static readonly ADPropertyDefinition ServerName = ADMailboxRecipientSchema.ServerName;

		public static readonly ADPropertyDefinition WindowsLiveID = ADRecipientSchema.WindowsLiveID;

		public static readonly ADPropertyDefinition PrimaryMailboxSource = ADUserSchema.PrimaryMailboxSource;

		public static readonly ADPropertyDefinition SatchmoClusterIp = ADUserSchema.SatchmoClusterIp;

		public static readonly ADPropertyDefinition SatchmoDGroup = ADUserSchema.SatchmoDGroup;

		public static readonly ADPropertyDefinition FblEnabled = ADUserSchema.FblEnabled;

		public static readonly ADPropertyDefinition Gender = ADUserSchema.Gender;

		public static readonly ADPropertyDefinition Occupation = ADUserSchema.Occupation;

		public static readonly ADPropertyDefinition Region = ADUserSchema.Region;

		public static readonly ADPropertyDefinition Timezone = ADUserSchema.Timezone;

		public static readonly ADPropertyDefinition Birthdate = ADUserSchema.Birthdate;

		public static readonly ADPropertyDefinition BirthdayPrecision = ADUserSchema.BirthdayPrecision;

		public static readonly ADPropertyDefinition NameVersion = ADUserSchema.NameVersion;

		public static readonly ADPropertyDefinition AlternateSupportEmailAddresses = ADUserSchema.AlternateSupportEmailAddresses;

		public static readonly ADPropertyDefinition PostalCode = ADUserSchema.PostalCode;

		public static readonly ADPropertyDefinition OptInUser = ADUserSchema.OptInUser;

		public static readonly ADPropertyDefinition MigrationDryRun = ADUserSchema.MigrationDryRun;

		public static readonly ADPropertyDefinition FirstName = ADUserSchema.FirstName;

		public static readonly ADPropertyDefinition LastName = ADUserSchema.LastName;

		public static readonly ADPropertyDefinition UsageLocation = ADRecipientSchema.UsageLocation;

		public static readonly ADPropertyDefinition LocaleID = ADUserSchema.LocaleID;

		public static readonly ADPropertyDefinition IsPremiumConsumerMailbox = ADUserSchema.IsPremiumConsumerMailbox;

		public static readonly ADPropertyDefinition IsMigratedConsumerMailbox = ADUserSchema.IsMigratedConsumerMailbox;
	}
}
