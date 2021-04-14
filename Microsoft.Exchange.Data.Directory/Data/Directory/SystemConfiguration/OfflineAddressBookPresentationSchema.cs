using System;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class OfflineAddressBookPresentationSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<OfflineAddressBookSchema>();
		}

		public static readonly ADPropertyDefinition Server = OfflineAddressBookSchema.Server;

		public static readonly ADPropertyDefinition AddressLists = OfflineAddressBookSchema.AddressLists;

		public static readonly ADPropertyDefinition IsDefault = OfflineAddressBookSchema.IsDefault;

		public static readonly ADPropertyDefinition PublicFolderDatabase = OfflineAddressBookSchema.PublicFolderDatabase;

		public static readonly ADPropertyDefinition VirtualDirectories = OfflineAddressBookSchema.VirtualDirectories;

		public static readonly ADPropertyDefinition MaxBinaryPropertySize = OfflineAddressBookSchema.MaxBinaryPropertySize;

		public static readonly ADPropertyDefinition MaxMultivaluedBinaryPropertySize = OfflineAddressBookSchema.MaxMultivaluedBinaryPropertySize;

		public static readonly ADPropertyDefinition MaxStringPropertySize = OfflineAddressBookSchema.MaxStringPropertySize;

		public static readonly ADPropertyDefinition MaxMultivaluedStringPropertySize = OfflineAddressBookSchema.MaxMultivaluedStringPropertySize;

		public static readonly ADPropertyDefinition LastTouchedTime = OfflineAddressBookSchema.LastTouchedTime;

		public static readonly ADPropertyDefinition LastRequestedTime = OfflineAddressBookSchema.LastRequestedTime;

		public static readonly ADPropertyDefinition LastFailedTime = OfflineAddressBookSchema.LastFailedTime;

		public static readonly ADPropertyDefinition LastNumberOfRecords = OfflineAddressBookSchema.LastNumberOfRecords;

		public static readonly ADPropertyDefinition LastGeneratingData = OfflineAddressBookSchema.LastGeneratingData;

		public static readonly ADPropertyDefinition DiffRetentionPeriod = OfflineAddressBookSchema.DiffRetentionPeriod;

		public static readonly ADPropertyDefinition Versions = OfflineAddressBookSchema.Versions;

		public static readonly ADPropertyDefinition Schedule = OfflineAddressBookSchema.Schedule;

		public static readonly ADPropertyDefinition PublicFolderDistributionEnabled = OfflineAddressBookSchema.PublicFolderDistributionEnabled;

		public static readonly ADPropertyDefinition GlobalWebDistributionEnabled = OfflineAddressBookSchema.GlobalWebDistributionEnabled;

		public static readonly ADPropertyDefinition WebDistributionEnabled = OfflineAddressBookSchema.WebDistributionEnabled;

		public static readonly ADPropertyDefinition ShadowMailboxDistributionEnabled = OfflineAddressBookSchema.ShadowMailboxDistributionEnabled;

		public static readonly ADPropertyDefinition ConfiguredAttributes = OfflineAddressBookSchema.ConfiguredAttributes;

		public static readonly ADPropertyDefinition AdminDisplayName = ADConfigurationObjectSchema.AdminDisplayName;

		public static readonly ADPropertyDefinition GeneratingMailbox = OfflineAddressBookSchema.GeneratingMailbox;
	}
}
