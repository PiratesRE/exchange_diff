using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class MailboxCreator : ConfigurableObjectCreator
	{
		internal override IList<string> GetProperties(string fullName)
		{
			return new string[]
			{
				"Identity",
				"DisplayName",
				"Alias",
				"HiddenFromAddressListsEnabled",
				"ServerLegacyDN",
				"Database",
				"ArchiveDatabase",
				"WhenChanged",
				"CustomAttribute1",
				"CustomAttribute2",
				"CustomAttribute3",
				"CustomAttribute4",
				"CustomAttribute5",
				"CustomAttribute6",
				"CustomAttribute7",
				"CustomAttribute8",
				"CustomAttribute9",
				"CustomAttribute10",
				"CustomAttribute11",
				"CustomAttribute12",
				"CustomAttribute13",
				"CustomAttribute14",
				"CustomAttribute15",
				"EmailAddressPolicyEnabled",
				"EmailAddresses",
				"ManagedFolderMailboxPolicy",
				"RetentionPolicy",
				"RetentionUrl",
				"RetentionComment",
				"LitigationHoldEnabled",
				"RetentionHoldEnabled",
				"StartDateForRetentionHold",
				"EndDateForRetentionHold",
				"UseDatabaseQuotaDefaults",
				"ArchiveQuota",
				"IssueWarningQuota",
				"ProhibitSendQuota",
				"ProhibitSendReceiveQuota",
				"ArchiveGuid",
				"ArchiveName",
				"ArchiveWarningQuota",
				"UseDatabaseRetentionDefaults",
				"RetainDeletedItemsUntilBackup",
				"RetainDeletedItemsFor",
				"SharingPolicy",
				"RoleAssignmentPolicy",
				"MailboxPlan",
				"GrantSendOnBehalfTo",
				"ForwardingAddress",
				"DeliverToMailboxAndForward",
				"RecipientLimits",
				"MaxSendSize",
				"MaxReceiveSize",
				"RecipientTypeDetails",
				"AcceptMessagesOnlyFromSendersOrMembers",
				"RejectMessagesFromSendersOrMembers",
				"RequireSenderAuthenticationEnabled",
				"UMEnabled",
				"ResourceCapacity",
				"ResourceCustom",
				"ResourceType"
			};
		}

		protected override void FillProperty(Type type, PSObject psObject, ConfigurableObject configObject, string propertyName)
		{
			if (propertyName == "RequireSenderAuthenticationEnabled")
			{
				configObject.propertyBag[MailEnabledRecipientSchema.RequireSenderAuthenticationEnabled] = MockObjectCreator.GetSingleProperty(psObject.Members[propertyName].Value, MailEnabledRecipientSchema.RequireSenderAuthenticationEnabled.Type);
				return;
			}
			if (propertyName == "RetentionHoldEnabled")
			{
				configObject.propertyBag[MailboxSchema.ElcExpirationSuspensionEnabled] = MockObjectCreator.GetSingleProperty(psObject.Members[propertyName].Value, MailboxSchema.ElcExpirationSuspensionEnabled.Type);
				return;
			}
			if (propertyName == "StartDateForRetentionHold")
			{
				configObject.propertyBag[MailboxSchema.ElcExpirationSuspensionStartDate] = MockObjectCreator.GetSingleProperty(psObject.Members[propertyName].Value, MailboxSchema.ElcExpirationSuspensionStartDate.Type);
				return;
			}
			if (propertyName == "EndDateForRetentionHold")
			{
				configObject.propertyBag[MailboxSchema.ElcExpirationSuspensionEndDate] = MockObjectCreator.GetSingleProperty(psObject.Members[propertyName].Value, MailboxSchema.ElcExpirationSuspensionEndDate.Type);
				return;
			}
			base.FillProperty(type, psObject, configObject, propertyName);
		}
	}
}
