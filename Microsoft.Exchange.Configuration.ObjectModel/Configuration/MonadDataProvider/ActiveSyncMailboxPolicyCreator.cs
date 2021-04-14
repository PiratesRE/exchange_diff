using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class ActiveSyncMailboxPolicyCreator : ConfigurableObjectCreator
	{
		internal override IList<string> GetProperties(string fullName)
		{
			return new string[]
			{
				"Identity",
				"Name",
				"WhenChanged",
				"DevicePolicyRefreshInterval",
				"AllowNonProvisionableDevices",
				"IsDefaultPolicy",
				"DevicePasswordEnabled",
				"AlphanumericDevicePasswordRequired",
				"MinDevicePasswordComplexCharacters",
				"PasswordRecoveryEnabled",
				"RequireStorageCardEncryption",
				"RequireDeviceEncryption",
				"MaxInactivityTimeDeviceLock",
				"MaxDevicePasswordFailedAttempts",
				"MinDevicePasswordLength",
				"AllowSimpleDevicePassword",
				"DevicePasswordExpiration",
				"DevicePasswordHistory",
				"MaxCalendarAgeFilter",
				"MaxEmailAgeFilter",
				"MaxEmailBodyTruncationSize",
				"RequireManualSyncWhenRoaming",
				"AllowHTMLEmail",
				"AttachmentsEnabled",
				"MaxAttachmentSize",
				"AllowStorageCard",
				"AllowCamera",
				"AllowWiFi",
				"AllowIrDA",
				"AllowInternetSharing",
				"AllowRemoteDesktop",
				"AllowDesktopSync",
				"AllowBluetooth",
				"AllowBrowser",
				"AllowConsumerEmail",
				"AllowUnsignedApplications",
				"AllowUnsignedInstallationPackages",
				"ApprovedApplicationList",
				"UnapprovedInROMApplicationList"
			};
		}

		protected override void FillProperty(Type type, PSObject psObject, ConfigurableObject configObject, string propertyName)
		{
			if (propertyName == "ApprovedApplicationList")
			{
				ApprovedApplicationCollection approvedApplicationCollection = new ApprovedApplicationCollection();
				IList list = ((PSObject)psObject.Members[propertyName].Value).BaseObject as IList;
				foreach (object prop in list)
				{
					approvedApplicationCollection.Add(MockObjectCreator.GetSingleProperty(prop, typeof(ApprovedApplication)));
				}
				configObject.propertyBag[MobileMailboxPolicySchema.ADApprovedApplicationList] = approvedApplicationCollection;
				return;
			}
			base.FillProperty(type, psObject, configObject, propertyName);
		}
	}
}
