using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class ExchangeProvisioningDataFactory
	{
		internal static ProvisioningData GetProvisioningUpdateData(IMigrationDataProvider provider, MigrationJob exchangeJob, MigrationJobItem exchangeJobItem)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(exchangeJob, "exchangeJob");
			MigrationUtil.ThrowOnNullArgument(exchangeJobItem, "exchangeJobItem");
			switch (exchangeJobItem.RecipientType)
			{
			case MigrationUserRecipientType.Mailbox:
				return ExchangeProvisioningDataFactory.GetMailboxUserUpdate(exchangeJob, exchangeJobItem);
			case MigrationUserRecipientType.Contact:
				return ExchangeProvisioningDataFactory.GetMailContactUpdate(exchangeJob, exchangeJobItem);
			case MigrationUserRecipientType.Group:
				return ExchangeProvisioningDataFactory.GetGroupMembers(provider, exchangeJob, exchangeJobItem);
			case MigrationUserRecipientType.Mailuser:
				return ExchangeProvisioningDataFactory.GetMailEnabledUserUpdate(exchangeJob, exchangeJobItem);
			}
			throw new MigrationDataCorruptionException("Unknown or unsupported Recipient Type: " + exchangeJobItem.RecipientType);
		}

		internal static ProvisioningData GetProvisioningData(IMigrationDataProvider provider, MigrationJob exchangeJob, MigrationJobItem exchangeJobItem)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(exchangeJob, "exchangeJob");
			MigrationUtil.ThrowOnNullArgument(exchangeJobItem, "exchangeJobItem");
			switch (exchangeJobItem.RecipientType)
			{
			case MigrationUserRecipientType.Mailbox:
				return ExchangeProvisioningDataFactory.GetMailboxUser(provider, exchangeJob, exchangeJobItem);
			case MigrationUserRecipientType.Contact:
				return ExchangeProvisioningDataFactory.GetMailContact(provider, exchangeJob, exchangeJobItem);
			case MigrationUserRecipientType.Group:
				return ExchangeProvisioningDataFactory.GetGroup(provider, exchangeJob, exchangeJobItem);
			case MigrationUserRecipientType.Mailuser:
				return ExchangeProvisioningDataFactory.GetMailEnabledUser(provider, exchangeJob, exchangeJobItem);
			}
			throw new MigrationDataCorruptionException("Unknown or unsupported Recipient Type: " + exchangeJobItem.RecipientType);
		}

		private static ContactUpdateProvisioningData GetMailContactUpdate(MigrationJob exchangeJob, MigrationJobItem exchangeJobItem)
		{
			ExchangeProvisioningDataStorage exchangeProvisioningDataStorage = exchangeJobItem.ProvisioningData as ExchangeProvisioningDataStorage;
			MigrationUtil.ThrowOnNullArgument(exchangeProvisioningDataStorage, "provisioningData");
			ExchangeMigrationMailContactRecipient exchangeMigrationMailContactRecipient = (ExchangeMigrationMailContactRecipient)exchangeProvisioningDataStorage.ExchangeRecipient;
			ContactUpdateProvisioningData contactUpdateProvisioningData = ContactUpdateProvisioningData.Create(exchangeJobItem.Identifier);
			contactUpdateProvisioningData.Component = ProvisioningComponent.ExchangeMigration;
			string manager;
			if (exchangeMigrationMailContactRecipient.TryGetPropertyValue<string>((PropTag)2147811359U, out manager))
			{
				contactUpdateProvisioningData.Manager = manager;
			}
			string[] grantSendOnBehalfTo;
			if (exchangeMigrationMailContactRecipient.TryGetPropertyValue<string[]>((PropTag)2148864031U, out grantSendOnBehalfTo))
			{
				contactUpdateProvisioningData.GrantSendOnBehalfTo = grantSendOnBehalfTo;
			}
			if (!contactUpdateProvisioningData.IsEmpty())
			{
				return contactUpdateProvisioningData;
			}
			return null;
		}

		private static MailEnabledUserUpdateProvisioningData GetMailEnabledUserUpdate(MigrationJob exchangeJob, MigrationJobItem exchangeJobItem)
		{
			ExchangeProvisioningDataStorage exchangeProvisioningDataStorage = exchangeJobItem.ProvisioningData as ExchangeProvisioningDataStorage;
			MigrationUtil.ThrowOnNullArgument(exchangeProvisioningDataStorage, "provisioningData");
			ExchangeMigrationMailEnabledUserRecipient exchangeMigrationMailEnabledUserRecipient = (ExchangeMigrationMailEnabledUserRecipient)exchangeProvisioningDataStorage.ExchangeRecipient;
			MailEnabledUserUpdateProvisioningData mailEnabledUserUpdateProvisioningData = MailEnabledUserUpdateProvisioningData.Create(exchangeJobItem.Identifier);
			mailEnabledUserUpdateProvisioningData.Component = ProvisioningComponent.ExchangeMigration;
			string manager;
			if (exchangeMigrationMailEnabledUserRecipient.TryGetPropertyValue<string>((PropTag)2147811359U, out manager))
			{
				mailEnabledUserUpdateProvisioningData.Manager = manager;
			}
			string[] grantSendOnBehalfTo;
			if (exchangeMigrationMailEnabledUserRecipient.TryGetPropertyValue<string[]>((PropTag)2148864031U, out grantSendOnBehalfTo))
			{
				mailEnabledUserUpdateProvisioningData.GrantSendOnBehalfTo = grantSendOnBehalfTo;
			}
			if (!mailEnabledUserUpdateProvisioningData.IsEmpty())
			{
				return mailEnabledUserUpdateProvisioningData;
			}
			return null;
		}

		private static UserUpdateProvisioningData GetMailboxUserUpdate(MigrationJob exchangeJob, MigrationJobItem exchangeJobItem)
		{
			if (exchangeJob.IsStaged)
			{
				return null;
			}
			ExchangeProvisioningDataStorage exchangeProvisioningDataStorage = exchangeJobItem.ProvisioningData as ExchangeProvisioningDataStorage;
			MigrationUtil.ThrowOnNullArgument(exchangeProvisioningDataStorage, "provisioningData");
			ExchangeMigrationMailUserRecipient exchangeMigrationMailUserRecipient = (ExchangeMigrationMailUserRecipient)exchangeProvisioningDataStorage.ExchangeRecipient;
			UserUpdateProvisioningData userUpdateProvisioningData = UserUpdateProvisioningData.Create(exchangeJobItem.Identifier);
			userUpdateProvisioningData.Component = ProvisioningComponent.ExchangeMigration;
			string manager;
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<string>((PropTag)2147811359U, out manager))
			{
				userUpdateProvisioningData.Manager = manager;
			}
			string[] grantSendOnBehalfTo;
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<string[]>((PropTag)2148864031U, out grantSendOnBehalfTo))
			{
				userUpdateProvisioningData.GrantSendOnBehalfTo = grantSendOnBehalfTo;
			}
			if (!userUpdateProvisioningData.IsEmpty())
			{
				return userUpdateProvisioningData;
			}
			return null;
		}

		private static MemberProvisioningData GetGroupMembers(IMigrationDataProvider provider, MigrationJob exchangeJob, MigrationJobItem exchangeJobItem)
		{
			ExchangeProvisioningDataStorage exchangeProvisioningDataStorage = exchangeJobItem.ProvisioningData as ExchangeProvisioningDataStorage;
			MigrationUtil.ThrowOnNullArgument(exchangeProvisioningDataStorage, "provisioningData");
			MemberProvisioningData memberProvisioningData = MemberProvisioningData.Create(exchangeJobItem.Identifier);
			memberProvisioningData.Component = ProvisioningComponent.ExchangeMigration;
			if (exchangeJobItem.IsPAW)
			{
				ExchangeMigrationGroupRecipient exchangeMigrationGroupRecipient = (ExchangeMigrationGroupRecipient)exchangeProvisioningDataStorage.ExchangeRecipient;
				string[] grantSendOnBehalfTo;
				if (exchangeMigrationGroupRecipient.TryGetPropertyValue<string[]>((PropTag)2148864031U, out grantSendOnBehalfTo))
				{
					memberProvisioningData.GrantSendOnBehalfTo = grantSendOnBehalfTo;
				}
				string managedBy;
				if (exchangeMigrationGroupRecipient.TryGetPropertyValue<string>((PropTag)2148270111U, out managedBy))
				{
					memberProvisioningData.ManagedBy = managedBy;
				}
				if (exchangeMigrationGroupRecipient.Members != null)
				{
					int num = 0;
					GroupProvisioningSnapshot groupProvisioningSnapshot = exchangeJobItem.ProvisioningStatistics as GroupProvisioningSnapshot;
					if (groupProvisioningSnapshot != null)
					{
						num = groupProvisioningSnapshot.CountOfProvisionedMembers + groupProvisioningSnapshot.CountOfSkippedMembers;
					}
					if (exchangeMigrationGroupRecipient.Members.Length > num)
					{
						int config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("MigrationGroupMembersBatchSize");
						memberProvisioningData.Members = exchangeMigrationGroupRecipient.Members.Skip(num).Take(config).ToArray<string>();
					}
				}
			}
			else
			{
				LegacyExchangeMigrationGroupRecipient legacyExchangeMigrationGroupRecipient = (LegacyExchangeMigrationGroupRecipient)exchangeProvisioningDataStorage.ExchangeRecipient;
				string[] grantSendOnBehalfTo2;
				if (legacyExchangeMigrationGroupRecipient.TryGetPropertyValue<string[]>((PropTag)2148864031U, out grantSendOnBehalfTo2))
				{
					memberProvisioningData.GrantSendOnBehalfTo = grantSendOnBehalfTo2;
				}
				string managedBy2;
				if (legacyExchangeMigrationGroupRecipient.TryGetPropertyValue<string>((PropTag)2148270111U, out managedBy2))
				{
					memberProvisioningData.ManagedBy = managedBy2;
				}
				List<string> groupMembersInfo = exchangeJobItem.GetGroupMembersInfo(provider);
				if (groupMembersInfo != null)
				{
					memberProvisioningData.Members = groupMembersInfo.ToArray();
				}
			}
			if (!memberProvisioningData.IsEmpty())
			{
				return memberProvisioningData;
			}
			return null;
		}

		private static GroupProvisioningData GetGroup(IMigrationDataProvider provider, MigrationJob exchangeJob, MigrationJobItem exchangeJobItem)
		{
			ExchangeProvisioningDataStorage exchangeProvisioningDataStorage = exchangeJobItem.ProvisioningData as ExchangeProvisioningDataStorage;
			MigrationUtil.ThrowOnNullArgument(exchangeProvisioningDataStorage, "provisioningData");
			ExchangeMigrationRecipient exchangeRecipient = exchangeProvisioningDataStorage.ExchangeRecipient;
			string propertyValue = exchangeRecipient.GetPropertyValue<string>(PropTag.DisplayName);
			string name = propertyValue;
			string text;
			if (exchangeRecipient.TryGetPropertyValue<string>(PropTag.Account, out text))
			{
				name = text;
			}
			GroupProvisioningData groupProvisioningData = GroupProvisioningData.Create(name);
			groupProvisioningData.DisplayName = propertyValue;
			groupProvisioningData.IsSmtpAddressCheckWithAcceptedDomain = provider.ADProvider.IsSmtpAddressCheckWithAcceptedDomain;
			string[] emailAddresses;
			if (exchangeRecipient.TryGetProxyAddresses(out emailAddresses))
			{
				groupProvisioningData.EmailAddresses = emailAddresses;
			}
			groupProvisioningData.Action = (exchangeRecipient.DoesADObjectExist ? ProvisioningAction.UpdateExisting : ProvisioningAction.CreateNewOrUpdateExisting);
			groupProvisioningData.Component = ProvisioningComponent.ExchangeMigration;
			return groupProvisioningData;
		}

		private static ProvisioningData GetMailContact(IMigrationDataProvider provider, MigrationJob exchangeJob, MigrationJobItem exchangeJobItem)
		{
			ExchangeProvisioningDataStorage exchangeProvisioningDataStorage = exchangeJobItem.ProvisioningData as ExchangeProvisioningDataStorage;
			MigrationUtil.ThrowOnNullArgument(exchangeProvisioningDataStorage, "provisioningData");
			ExchangeMigrationMailContactRecipient exchangeMigrationMailContactRecipient = (ExchangeMigrationMailContactRecipient)exchangeProvisioningDataStorage.ExchangeRecipient;
			string propertyValue = exchangeMigrationMailContactRecipient.GetPropertyValue<string>(PropTag.DisplayName);
			string name = propertyValue;
			string text;
			if (exchangeMigrationMailContactRecipient.TryGetPropertyValue<string>(PropTag.Account, out text))
			{
				name = text;
			}
			ContactProvisioningData contactProvisioningData = ContactProvisioningData.Create(name, exchangeJobItem.Identifier);
			contactProvisioningData.DisplayName = propertyValue;
			contactProvisioningData.IsSmtpAddressCheckWithAcceptedDomain = provider.ADProvider.IsSmtpAddressCheckWithAcceptedDomain;
			if (exchangeMigrationMailContactRecipient.TryGetPropertyValue<string>(PropTag.GivenName, out text))
			{
				contactProvisioningData.FirstName = text;
			}
			if (exchangeMigrationMailContactRecipient.TryGetPropertyValue<string>(PropTag.Surname, out text))
			{
				contactProvisioningData.LastName = text;
			}
			if (exchangeMigrationMailContactRecipient.TryGetPropertyValue<string>(PropTag.PrimaryFaxNumber, out text))
			{
				contactProvisioningData.Fax = text;
			}
			if (exchangeMigrationMailContactRecipient.TryGetPropertyValue<string>(PropTag.BusinessTelephoneNumber, out text))
			{
				contactProvisioningData.Phone = text;
			}
			if (exchangeMigrationMailContactRecipient.TryGetPropertyValue<string>(PropTag.CompanyName, out text))
			{
				contactProvisioningData.Company = text;
			}
			if (exchangeMigrationMailContactRecipient.TryGetPropertyValue<string>(PropTag.DepartmentName, out text))
			{
				contactProvisioningData.Department = text;
			}
			if (exchangeMigrationMailContactRecipient.TryGetPropertyValue<string>(PropTag.Initials, out text))
			{
				contactProvisioningData.Initials = text;
			}
			if (exchangeMigrationMailContactRecipient.TryGetPropertyValue<string>(PropTag.MobileTelephoneNumber, out text))
			{
				contactProvisioningData.MobilePhone = text;
			}
			if (exchangeMigrationMailContactRecipient.TryGetPropertyValue<string>(PropTag.OfficeLocation, out text))
			{
				contactProvisioningData.Office = text;
			}
			if (exchangeMigrationMailContactRecipient.TryGetPropertyValue<string>(PropTag.Title, out text))
			{
				contactProvisioningData.Title = text;
			}
			if (exchangeMigrationMailContactRecipient.TryGetPropertyValue<string>(PropTag.HomeTelephoneNumber, out text))
			{
				contactProvisioningData.HomePhone = text;
			}
			if (exchangeMigrationMailContactRecipient.TryGetPropertyValue<string>(PropTag.StreetAddress, out text))
			{
				contactProvisioningData.StreetAddress = text;
			}
			if (exchangeMigrationMailContactRecipient.TryGetPropertyValue<string>(PropTag.Locality, out text))
			{
				contactProvisioningData.City = text;
			}
			if (exchangeMigrationMailContactRecipient.TryGetPropertyValue<string>(PropTag.StateOrProvince, out text))
			{
				contactProvisioningData.StateOrProvince = text;
			}
			if (exchangeMigrationMailContactRecipient.TryGetPropertyValue<string>(PropTag.PostalCode, out text))
			{
				contactProvisioningData.PostalCode = text;
			}
			if (exchangeMigrationMailContactRecipient.TryGetPropertyValue<string>((PropTag)2154364959U, out text))
			{
				contactProvisioningData.CountryOrRegion = text;
			}
			if (exchangeMigrationMailContactRecipient.TryGetPropertyValue<string>(PropTag.Comment, out text))
			{
				contactProvisioningData.Notes = text;
			}
			string[] emailAddresses;
			if (exchangeMigrationMailContactRecipient.TryGetProxyAddresses(out emailAddresses))
			{
				contactProvisioningData.EmailAddresses = emailAddresses;
			}
			contactProvisioningData.Action = (exchangeMigrationMailContactRecipient.DoesADObjectExist ? ProvisioningAction.UpdateExisting : ProvisioningAction.CreateNewOrUpdateExisting);
			contactProvisioningData.Component = ProvisioningComponent.ExchangeMigration;
			return contactProvisioningData;
		}

		private static ProvisioningData GetMailEnabledUser(IMigrationDataProvider provider, MigrationJob exchangeJob, MigrationJobItem exchangeJobItem)
		{
			ExchangeProvisioningDataStorage exchangeProvisioningDataStorage = exchangeJobItem.ProvisioningData as ExchangeProvisioningDataStorage;
			MigrationUtil.ThrowOnNullArgument(exchangeProvisioningDataStorage, "provisioningData");
			ExchangeMigrationMailEnabledUserRecipient exchangeMigrationMailEnabledUserRecipient = (ExchangeMigrationMailEnabledUserRecipient)exchangeProvisioningDataStorage.ExchangeRecipient;
			string propertyValue = exchangeMigrationMailEnabledUserRecipient.GetPropertyValue<string>(PropTag.DisplayName);
			string password = MigrationUtil.EncryptedStringToClearText(exchangeProvisioningDataStorage.EncryptedPassword);
			string name = propertyValue;
			string text;
			if (exchangeMigrationMailEnabledUserRecipient.TryGetPropertyValue<string>(PropTag.Account, out text))
			{
				name = text;
			}
			MailEnabledUserProvisioningData mailEnabledUserProvisioningData = MailEnabledUserProvisioningData.Create(name, exchangeJobItem.Identifier, password, provider.ADProvider.IsMSOSyncEnabled);
			mailEnabledUserProvisioningData.DisplayName = propertyValue;
			mailEnabledUserProvisioningData.IsSmtpAddressCheckWithAcceptedDomain = provider.ADProvider.IsSmtpAddressCheckWithAcceptedDomain;
			if (exchangeMigrationMailEnabledUserRecipient.TryGetPropertyValue<string>(PropTag.GivenName, out text))
			{
				mailEnabledUserProvisioningData.FirstName = text;
			}
			if (exchangeMigrationMailEnabledUserRecipient.TryGetPropertyValue<string>(PropTag.Surname, out text))
			{
				mailEnabledUserProvisioningData.LastName = text;
			}
			if (exchangeMigrationMailEnabledUserRecipient.TryGetPropertyValue<string>(PropTag.PrimaryFaxNumber, out text))
			{
				mailEnabledUserProvisioningData.Fax = text;
			}
			if (exchangeMigrationMailEnabledUserRecipient.TryGetPropertyValue<string>(PropTag.BusinessTelephoneNumber, out text))
			{
				mailEnabledUserProvisioningData.Phone = text;
			}
			if (exchangeMigrationMailEnabledUserRecipient.TryGetPropertyValue<string>(PropTag.CompanyName, out text))
			{
				mailEnabledUserProvisioningData.Company = text;
			}
			if (exchangeMigrationMailEnabledUserRecipient.TryGetPropertyValue<string>(PropTag.DepartmentName, out text))
			{
				mailEnabledUserProvisioningData.Department = text;
			}
			if (exchangeMigrationMailEnabledUserRecipient.TryGetPropertyValue<string>(PropTag.Initials, out text))
			{
				mailEnabledUserProvisioningData.Initials = text;
			}
			if (exchangeMigrationMailEnabledUserRecipient.TryGetPropertyValue<string>(PropTag.MobileTelephoneNumber, out text))
			{
				mailEnabledUserProvisioningData.MobilePhone = text;
			}
			if (exchangeMigrationMailEnabledUserRecipient.TryGetPropertyValue<string>(PropTag.OfficeLocation, out text))
			{
				mailEnabledUserProvisioningData.Office = text;
			}
			if (exchangeMigrationMailEnabledUserRecipient.TryGetPropertyValue<string>(PropTag.Title, out text))
			{
				mailEnabledUserProvisioningData.Title = text;
			}
			if (exchangeMigrationMailEnabledUserRecipient.TryGetPropertyValue<string>(PropTag.HomeTelephoneNumber, out text))
			{
				mailEnabledUserProvisioningData.HomePhone = text;
			}
			if (exchangeMigrationMailEnabledUserRecipient.TryGetPropertyValue<string>(PropTag.StreetAddress, out text))
			{
				mailEnabledUserProvisioningData.StreetAddress = text;
			}
			if (exchangeMigrationMailEnabledUserRecipient.TryGetPropertyValue<string>(PropTag.Locality, out text))
			{
				mailEnabledUserProvisioningData.City = text;
			}
			if (exchangeMigrationMailEnabledUserRecipient.TryGetPropertyValue<string>(PropTag.StateOrProvince, out text))
			{
				mailEnabledUserProvisioningData.StateOrProvince = text;
			}
			if (exchangeMigrationMailEnabledUserRecipient.TryGetPropertyValue<string>(PropTag.PostalCode, out text))
			{
				mailEnabledUserProvisioningData.PostalCode = text;
			}
			if (exchangeMigrationMailEnabledUserRecipient.TryGetPropertyValue<string>((PropTag)2154364959U, out text))
			{
				mailEnabledUserProvisioningData.CountryOrRegion = text;
			}
			if (exchangeMigrationMailEnabledUserRecipient.TryGetPropertyValue<string>(PropTag.Comment, out text))
			{
				mailEnabledUserProvisioningData.Notes = text;
			}
			string[] emailAddresses;
			if (exchangeMigrationMailEnabledUserRecipient.TryGetProxyAddresses(out emailAddresses))
			{
				mailEnabledUserProvisioningData.EmailAddresses = emailAddresses;
			}
			mailEnabledUserProvisioningData.Action = (exchangeMigrationMailEnabledUserRecipient.DoesADObjectExist ? ProvisioningAction.UpdateExisting : ProvisioningAction.CreateNewOrUpdateExisting);
			mailEnabledUserProvisioningData.Component = ProvisioningComponent.ExchangeMigration;
			return mailEnabledUserProvisioningData;
		}

		private static ProvisioningData GetMailboxUser(IMigrationDataProvider provider, MigrationJob exchangeJob, MigrationJobItem exchangeJobItem)
		{
			ExchangeProvisioningDataStorage exchangeProvisioningDataStorage = exchangeJobItem.ProvisioningData as ExchangeProvisioningDataStorage;
			MigrationUtil.ThrowOnNullArgument(exchangeProvisioningDataStorage, "provisioningData");
			ExchangeMigrationMailUserRecipient exchangeMigrationMailUserRecipient = (ExchangeMigrationMailUserRecipient)exchangeProvisioningDataStorage.ExchangeRecipient;
			string text = exchangeMigrationMailUserRecipient.GetPropertyValue<string>(PropTag.DisplayName);
			string text2 = MigrationUtil.EncryptedStringToClearText(exchangeProvisioningDataStorage.EncryptedPassword);
			ExchangeOutlookAnywhereEndpoint exchangeOutlookAnywhereEndpoint = (ExchangeOutlookAnywhereEndpoint)exchangeJob.SourceEndpoint;
			string name = text;
			string text3;
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<string>(PropTag.Account, out text3))
			{
				name = text3;
			}
			if (string.Equals(text, "Administrator", StringComparison.OrdinalIgnoreCase))
			{
				text = "Administrator (" + exchangeOutlookAnywhereEndpoint.NspiServer + ")";
				name = "Administrator-" + exchangeOutlookAnywhereEndpoint.NspiServer;
			}
			bool useExistingId = string.IsNullOrEmpty(text2);
			UserProvisioningData userProvisioningData = UserProvisioningData.Create(name, exchangeJobItem.Identifier, useExistingId, text2, provider.ADProvider.IsMSOSyncEnabled);
			userProvisioningData.DisplayName = text;
			userProvisioningData.ResetPasswordOnNextLogon = (!exchangeJob.IsStaged || exchangeProvisioningDataStorage.ForceChangePassword);
			userProvisioningData.IsSmtpAddressCheckWithAcceptedDomain = provider.ADProvider.IsSmtpAddressCheckWithAcceptedDomain;
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<string>(PropTag.GivenName, out text3))
			{
				userProvisioningData.FirstName = text3;
			}
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<string>(PropTag.Surname, out text3))
			{
				userProvisioningData.LastName = text3;
			}
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<string>(PropTag.PrimaryFaxNumber, out text3))
			{
				userProvisioningData.Fax = text3;
			}
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<string>(PropTag.BusinessTelephoneNumber, out text3))
			{
				userProvisioningData.Phone = text3;
			}
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<string>(PropTag.CompanyName, out text3))
			{
				userProvisioningData.Company = text3;
			}
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<string>(PropTag.DepartmentName, out text3))
			{
				userProvisioningData.Department = text3;
			}
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<string>(PropTag.Initials, out text3))
			{
				userProvisioningData.Initials = text3;
			}
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<string>(PropTag.MobileTelephoneNumber, out text3))
			{
				userProvisioningData.MobilePhone = text3;
			}
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<string>(PropTag.OfficeLocation, out text3))
			{
				userProvisioningData.Office = text3;
			}
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<string>(PropTag.Title, out text3))
			{
				userProvisioningData.Title = text3;
			}
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<string>((PropTag)2359230495U, out text3) && !string.IsNullOrEmpty(text3))
			{
				userProvisioningData.Languages = text3.Split(ExchangeProvisioningDataFactory.UserCultureDelimiters, StringSplitOptions.RemoveEmptyEntries);
			}
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<string>(PropTag.DisplayTypeEx, out text3))
			{
				userProvisioningData.ResourceType = text3;
			}
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<string>(PropTag.HomeTelephoneNumber, out text3))
			{
				userProvisioningData.HomePhone = text3;
			}
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<string>(PropTag.StreetAddress, out text3))
			{
				userProvisioningData.StreetAddress = text3;
			}
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<string>(PropTag.Locality, out text3))
			{
				userProvisioningData.City = text3;
			}
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<string>(PropTag.StateOrProvince, out text3))
			{
				userProvisioningData.StateOrProvince = text3;
			}
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<string>(PropTag.PostalCode, out text3))
			{
				userProvisioningData.PostalCode = text3;
			}
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<string>((PropTag)2154364959U, out text3))
			{
				userProvisioningData.CountryOrRegion = text3;
			}
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<string>(PropTag.Comment, out text3))
			{
				userProvisioningData.Notes = text3;
			}
			int resourceCapacity;
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<int>((PropTag)134676483U, out resourceCapacity))
			{
				userProvisioningData.ResourceCapacity = resourceCapacity;
			}
			string[] emailAddresses;
			if (exchangeMigrationMailUserRecipient.TryGetProxyAddresses(out emailAddresses))
			{
				userProvisioningData.EmailAddresses = emailAddresses;
			}
			byte[] umspokenName;
			if (exchangeMigrationMailUserRecipient.TryGetPropertyValue<byte[]>((PropTag)2361524482U, out umspokenName))
			{
				userProvisioningData.UMSpokenName = umspokenName;
			}
			userProvisioningData.Action = (exchangeMigrationMailUserRecipient.DoesADObjectExist ? ProvisioningAction.UpdateExisting : ProvisioningAction.CreateNewOrUpdateExisting);
			userProvisioningData.Component = (exchangeJob.IsStaged ? ProvisioningComponent.StagedExchangeMigration : ProvisioningComponent.ExchangeMigration);
			return userProvisioningData;
		}

		private static readonly char[] UserCultureDelimiters = new char[]
		{
			','
		};
	}
}
