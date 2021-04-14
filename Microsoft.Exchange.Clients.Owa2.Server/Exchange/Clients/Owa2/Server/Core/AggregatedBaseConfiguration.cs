using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Configuration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class AggregatedBaseConfiguration : ConfigurationBase
	{
		public static AggregatedBaseConfiguration ConfigurationFromData(OwaConfigurationBaseData data)
		{
			AllowOfflineOnEnum allowOfflineOn;
			Enum.TryParse<AllowOfflineOnEnum>(data.AllowOfflineOn, out allowOfflineOn);
			InstantMessagingTypeOptions instantMessagingType;
			Enum.TryParse<InstantMessagingTypeOptions>(data.InstantMessagingType, out instantMessagingType);
			OutboundCharsetOptions outboundCharset;
			Enum.TryParse<OutboundCharsetOptions>(data.OutboundCharset, out outboundCharset);
			return new AggregatedBaseConfiguration
			{
				AllowCopyContactsToDeviceAddressBook = data.AllowCopyContactsToDeviceAddressBook,
				AllowOfflineOn = allowOfflineOn,
				AttachmentPolicy = AggregatedBaseConfiguration.AttachmentPolicyFromData(data.AttachmentPolicy),
				DefaultTheme = data.DefaultTheme,
				InstantMessagingEnabled = data.InstantMessagingEnabled,
				InstantMessagingType = instantMessagingType,
				OutboundCharset = outboundCharset,
				PlacesEnabled = data.PlacesEnabled,
				WeatherEnabled = data.WeatherEnabled,
				RecoverDeletedItemsEnabled = data.RecoverDeletedItemsEnabled,
				SegmentationFlags = data.SegmentationFlags,
				UseGB18030 = data.UseGB18030,
				UseISO885915 = data.UseISO885915
			};
		}

		public static OwaConfigurationBaseData DataFromConfiguration(ConfigurationBase config)
		{
			return new OwaConfigurationBaseData
			{
				AttachmentPolicy = config.AttachmentPolicy.PolicyData,
				AllowCopyContactsToDeviceAddressBook = config.AllowCopyContactsToDeviceAddressBook,
				AllowOfflineOn = config.AllowOfflineOn.ToString(),
				DefaultTheme = config.DefaultTheme,
				InstantMessagingEnabled = config.InstantMessagingEnabled,
				InstantMessagingType = config.InstantMessagingType.ToString(),
				OutboundCharset = config.OutboundCharset.ToString(),
				PlacesEnabled = config.PlacesEnabled,
				WeatherEnabled = config.WeatherEnabled,
				RecoverDeletedItemsEnabled = config.RecoverDeletedItemsEnabled,
				SegmentationFlags = config.SegmentationFlags,
				UseGB18030 = config.UseGB18030,
				UseISO885915 = config.UseISO885915
			};
		}

		public static AttachmentPolicy AttachmentPolicyFromData(OwaAttachmentPolicyData data)
		{
			AttachmentPolicyLevel treatUnknownTypeAs;
			Enum.TryParse<AttachmentPolicyLevel>(data.TreatUnknownTypeAs, out treatUnknownTypeAs);
			return new AttachmentPolicy(data.BlockFileTypes, data.BlockMimeTypes, data.ForceSaveFileTypes, data.ForceSaveMimeTypes, data.AllowFileTypes, data.AllowMimeTypes, treatUnknownTypeAs, data.DirectFileAccessOnPublicComputersEnabled, data.DirectFileAccessOnPrivateComputersEnabled, data.ForceWacViewingFirstOnPublicComputers, data.ForceWacViewingFirstOnPrivateComputers, data.WacViewingOnPublicComputersEnabled, data.WacViewingOnPrivateComputersEnabled, data.ForceWebReadyDocumentViewingFirstOnPublicComputers, data.ForceWebReadyDocumentViewingFirstOnPrivateComputers, data.WebReadyDocumentViewingOnPublicComputersEnabled, data.WebReadyDocumentViewingOnPrivateComputersEnabled, data.WebReadyFileTypes, data.WebReadyMimeTypes, data.WebReadyDocumentViewingSupportedFileTypes, data.WebReadyDocumentViewingSupportedMimeTypes, data.WebReadyDocumentViewingForAllSupportedTypes);
		}
	}
}
